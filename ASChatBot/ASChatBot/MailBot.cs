using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ASChatBot
{
    public struct OrderInformation
    {
        public string modelName;
        public string name;
        public string email;
        public string phone;
        public string size;
        public string color;
        public string insides;
        public string delivery;
        public string adress;
        public string paymentType;
        public string uniqueCartID;
        public int price;
        public int deliveryPrice;
    }

    public class MailBot
    {
        private readonly string email;
        private readonly string securityKey;
        private readonly int port = 993;
        private readonly string host = "imap.gmail.com";

        private int messagesInInbox;

        public ImapClient Client { get; private set; }

        public CancellationTokenSource Cancel { get; private set; }
        public CancellationTokenSource done { get; private set; }

        public MailBot(string email, string securityKey)
        {
            this.email = email;
            this.securityKey = securityKey;
            Client = new ImapClient();
        }

        private async Task Reconnect()
        {
            if (Client.IsConnected && Client.IsAuthenticated) return;

            while(!Client.IsConnected && !Client.IsAuthenticated)
            {
                try
                {
                    if (!Client.IsConnected)
                        await Client.ConnectAsync(host, port);

                    if (!Client.IsAuthenticated)
                    {
                        Client.AuthenticationMechanisms.Remove("XOAUTH2");

                        await Client.AuthenticateAsync(email, securityKey);

                        Client.Inbox.Open(FolderAccess.ReadOnly);
                    }

                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }
            
        }

        public async Task Run()
        {
            await Reconnect();

            if (Client.Inbox.Count > 0)
            {
                messagesInInbox = Client.Inbox.Count;
            }

            Client.Inbox.CountChanged += CountChanged;

            Cancel = new CancellationTokenSource();

            do
            {
                try
                {
                    done = new CancellationTokenSource(new TimeSpan(0, 9, 0));

                    await Reconnect();

                    await Client.IdleAsync(done.Token, Cancel.Token);

                }
                catch (ImapProtocolException)
                {
                    await Reconnect();
                }
                catch (IOException)
                {
                    await Reconnect();
                }
                catch(Exception e)
                {
                    var message = e.Message;
                }

                lock (Client.SyncRoot)
                {
                    done.Dispose();
                    done = null;
                    if (Cancel.IsCancellationRequested) Dispose();
                }

            } while (!Cancel.IsCancellationRequested);

        }

        private void CountChanged(object sender, EventArgs e)
        {
            lock (Client.SyncRoot)
            {
                RespondOnNewMessage();
            }
        }

        public void Dispose()
        {
            lock (Client.SyncRoot)
            {
                Client.Inbox.CountChanged -= CountChanged;
                Client.Disconnect(true);
                Client.Dispose();
                done?.Dispose();
                Cancel.Dispose();
            }
           
        }

        public void RespondOnNewMessage()
        {
            using (var newClient = new ImapClient())
            {
                newClient.Connect(host, port);

                // Remove the XOAUTH2 authentication mechanism since we don't have an OAuth2 token.
                newClient.AuthenticationMechanisms.Remove("XOAUTH2");

                newClient.Authenticate(email, securityKey);

                newClient.Inbox.Open(FolderAccess.ReadOnly);

                var folder = newClient.Inbox;

                if (folder.Count < messagesInInbox)
                {
                    messagesInInbox = folder.Count;
                    return;
                }
                //Get new message
                var newMessage = folder.GetMessage(folder.Count-1);

                string text = GetTextFromHtmlBody(newMessage.HtmlBody);

                if (OrderMessageCheck(text))
                {
                    var orderInfo = new OrderInformation();

                    GetOrderInformation(text, ref orderInfo);

                    var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate(this.email, this.securityKey);

                    var email = new MimeMessage();

                    if (orderInfo.delivery == "Самовывоз") orderInfo.adress = "Лиговский проспект, 5";

                    email = (orderInfo.uniqueCartID == "rec440365314") ? AnswerToOrderFromStock(text, orderInfo) : AnswerToOrderForCustomMade(text, orderInfo);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                messagesInInbox = folder.Count;
            }
        }

        private string GetTextFromHtmlBody(string htmlBody)
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(htmlBody);

            StringBuilder sb = new StringBuilder();

            foreach (var node in doc.DocumentNode.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text1 = node.InnerText;

                    if (!string.IsNullOrEmpty(text1))
                        sb.AppendLine(text1.Trim());
                }
            }

            return sb.ToString();
        }

        private  MimeMessage AnswerToOrderForCustomMade(string text, OrderInformation orderInfo)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(this.email));
            email.To.Add(MailboxAddress.Parse(orderInfo.email));
            email.Subject = $"Anima Shoes Заказ {orderInfo.modelName}";


            if (orderInfo.paymentType == "Безналичная оплата")
            {
                email.Body = new TextPart()
                {
                    Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[0], ref orderInfo)
                };
            }
            else
            {
                if (orderInfo.deliveryPrice == 0)
                {
                    email.Body = new TextPart()
                    {
                        Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[1], ref orderInfo)
                    };
                }
                else if (orderInfo.delivery.Contains("Курьером"))
                {
                    email.Body = new TextPart()
                    {
                        Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[2], ref orderInfo)
                    };
                }
                else email.Body = new TextPart()
                {
                    Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[3], ref orderInfo)
                };

            }

            return email;

        }

        private  MimeMessage AnswerToOrderFromStock(string text, OrderInformation orderInfo)
        {
            var startInfoIndex = text.IndexOf("Размер:");
            var endInfoIndex = text.IndexOf(')') + 1;

            var info = text.Substring(startInfoIndex, endInfoIndex - startInfoIndex);

            string[] lines = info.Split(',');

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Substring(lines[i].IndexOf(':') + 2);

                switch (i)
                {
                    case 0:
                        orderInfo.size = lines[i];
                        break;
                    case 1:
                        orderInfo.color = lines[i];
                        break;
                    case 2:
                        orderInfo.insides = lines[i];
                        break;
                    default:
                        break;
                }
            }

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(this.email));
            email.To.Add(MailboxAddress.Parse(orderInfo.email));
            email.Subject = $"Anima Shoes Заказ {orderInfo.modelName}";

            if (orderInfo.paymentType == "Безналичная оплата")
            {
                email.Body = new TextPart()
                {
                    Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[4], ref orderInfo)
                };
            }
            else if (orderInfo.paymentType == "Наличными при получении")
            {
                email.Body = new TextPart()
                {
                    Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[5], ref orderInfo)
                };
            }
            else
            {
                if (orderInfo.deliveryPrice == 0)
                {
                    email.Body = new TextPart()
                    {
                        Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[6], ref orderInfo)
                    };
                }
                else if (orderInfo.delivery.Contains("Курьером"))
                {
                    email.Body = new TextPart()
                    {
                        Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[7], ref orderInfo)
                    };
                }
                else email.Body = new TextPart()
                {
                    Text = AnswersLibrary.MessageFormat(AnswersLibrary.EmailBotAnswers[8], ref orderInfo)
                };

            }

            return email;
        }

        private  void GetOrderInformation(string text, ref OrderInformation orderInfo)
        {
            orderInfo.uniqueCartID = GetContentFromLine(text, "Уникальный номер блока с формой:");

            orderInfo.modelName = GetModelType(ref text, ref orderInfo);

            GetDeliveryType(ref text, ref orderInfo);

            GetPaymentInfo(ref text, ref orderInfo);

            GetModelInfo(ref text, ref orderInfo);
        }

        private  string GetModelType(ref string text,ref OrderInformation orderInfo)
        {
            var subText = text.Substring(text.IndexOf("Информация о заказе:"));

            int startInfoIndex = subText.IndexOf('1') + 1;
            subText = subText.Substring(startInfoIndex);
            int endInfoIndex = orderInfo.uniqueCartID == "rec440365314" ? subText.IndexOf('(') : subText.IndexOf('1');

            subText = subText.Substring(1, endInfoIndex);

            string withoutNumbers = Regex.Replace(subText, "[0-9]", "");

            withoutNumbers = withoutNumbers.Trim(new Char[] { '\n', 'R', 'U', 'B' });

            withoutNumbers = withoutNumbers.TrimStart(' ');
            withoutNumbers = withoutNumbers.TrimEnd(' ', '\n', '(');

            return withoutNumbers;
        }

        private  void GetModelInfo(ref string text, ref OrderInformation orderInfo)
        {
            var startInfoIndex = text.IndexOf("name:");
            var endInfoIndex = text.IndexOf("policyAccepted:");

            string info = text.Substring(startInfoIndex, endInfoIndex - startInfoIndex);

            string[] lines = info.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (String.Empty != lines[i] && lines[i] != "")
                {
                    lines[i] = lines[i].Substring(lines[i].IndexOf(':') + 2);

                    if (orderInfo.uniqueCartID == "rec440365314")
                    {
                        switch (i)
                        {
                            case 0:
                                orderInfo.name = lines[i];
                                break;
                            case 1:
                                orderInfo.email = lines[i];
                                if(orderInfo.email.Contains('<')) orderInfo.email = orderInfo.email.Substring(0, orderInfo.email.IndexOf('<'));
                                break;
                            case 2:
                                orderInfo.phone = lines[i];
                                break;
                            case 3:
                                orderInfo.adress = lines[i];
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                orderInfo.name = lines[i];
                                break;
                            case 1:
                                orderInfo.email = lines[i];
                                if (orderInfo.email.Contains('<')) orderInfo.email = orderInfo.email.Substring(0, orderInfo.email.IndexOf('<'));
                                break;
                            case 2:
                                orderInfo.phone = lines[i];
                                break;
                            case 3:
                                orderInfo.size = lines[i];
                                break;
                            case 4:
                                orderInfo.color = lines[i];
                                break;
                            case 5:
                                orderInfo.insides = lines[i];
                                break;
                            case 6:
                                orderInfo.adress = lines[i];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private  void GetDeliveryType( ref string text, ref OrderInformation orderInfo)
        {
            orderInfo.delivery = GetContentFromLine(text, "Доставка");
            orderInfo.deliveryPrice = 0;

            if (orderInfo.delivery == "Самовывоз") return;

            orderInfo.delivery = orderInfo.delivery.Substring(0, orderInfo.delivery.IndexOf(':'));
            orderInfo.deliveryPrice = int.Parse(string.Join("", orderInfo.delivery.Where(c => char.IsDigit(c))));
        }

        private  void GetPaymentInfo(ref string text, ref OrderInformation orderInfo)
        {
            int searchableIndex = text.IndexOf("Сумма");
            string resultText = text.Substring(searchableIndex);
            resultText = resultText.Substring(0, resultText.IndexOf(" "));
            orderInfo.price = int.Parse(string.Join("", resultText.Where(c => char.IsDigit(c))));
            orderInfo.paymentType = GetContentFromLine(text, "Платежная система");
        }

        private  string GetContentFromLine(string text, string textToFind)
        {
            int searchableIndex = text.IndexOf(textToFind);
            string resultText = text.Substring(searchableIndex);
            resultText = resultText.Substring(0, resultText.IndexOf("\n"));

            return resultText.Substring(resultText.IndexOf(':') + 2);
        }

        private  bool OrderMessageCheck(string messageContent)
        {
            return messageContent.Contains("Уникальный номер блока с формой:");
        }

    }
}
