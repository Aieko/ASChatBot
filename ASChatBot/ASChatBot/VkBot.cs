using System;
using VkApi = VkNet.VkApi;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ASChatBot
{
    public class VkBot
    {
        public VkApi VkApiClient;

        private static LongPollServerResponse vkLongPollServer = null;

        private MessageKeyboard keyboard;

        private string vkAppToken;
        private ulong vkGroupId;

        public bool VkBotOn = false;

        public VkBot(string MyAppToken, ulong MyGroupId  )
        {
            vkAppToken = MyAppToken;
            vkGroupId = MyGroupId;

            VkApiClient = new VkApi();

            CreateKeyboard();
        }

        private async Task<bool> ReconnectVkGroup()
        {
            if (VkApiClient.IsAuthorized) return true;

            while (!VkApiClient.IsAuthorized)
            {
                try
                {
                    await VkApiClient.AuthorizeAsync(new ApiAuthParams() { AccessToken = vkAppToken });

                    return true;
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }

            return false;
        }

        public async Task Run()
        {
            VkBotOn = true;

            await ReconnectVkGroup();

            while (VkBotOn)
            {
                try
                {
                    var poll = CheckForNewMesseges();

                    if (poll != null) ChatBotAnswer(poll);
                }
                catch (Exception)
                {
                    await ReconnectVkGroup();

                    Thread.Sleep(2500);
                }
            }
        }

        public void Dispose()
        {
            VkBotOn = false;

            VkApiClient.Dispose();
        }
        
        private BotsLongPollHistoryResponse CheckForNewMesseges()
        {
            vkLongPollServer = VkApiClient.Groups.GetLongPollServer(vkGroupId);

            while(VkBotOn)
            {
                var poll = VkApiClient.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams()
                  { Server = vkLongPollServer.Server, Ts = vkLongPollServer.Ts, Key = vkLongPollServer.Key, Wait = 25 });

                if (poll?.Updates != null && VkBotOn) return poll;
            }

            return null;

        }

        private void CreateKeyboard()
        {
            var keyboardBuilder = new KeyboardBuilder(false);
            keyboardBuilder.SetInline(true);
            keyboardBuilder.AddButton("Заказать обувь на пошив", "");
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Узнать какие пары есть в наличии", "");
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Узнать о статусе готовности заказа", "");
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Подробнее о сервисе AnimaShoes", "");
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Связаться с сотрудником", "");

            keyboard = keyboardBuilder.Build();
        }

        private void SendMessageBack(Message msg, string text, bool withKeyboard = false)
        {
            if (withKeyboard)
            {
                VkApiClient.Messages.Send(new MessagesSendParams
                {
                    Keyboard = keyboard,
                    RandomId = new DateTime().Millisecond,
                    PeerId = msg.PeerId.Value,
                    Message = text
                });

                return;
            }


            VkApiClient.Messages.Send(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = msg.PeerId.Value,
                Message = text
            });
        }

        private Task SaveId(long id)
        {
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "savedIds.txt");
            using (var writer = File.AppendText(backingFile))
            {
                return writer.WriteLineAsync(id.ToString());
            }
        }

        private async Task<bool> ReadIdAsync(long id)
        {
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "savedIds.txt");

            if (backingFile == null || !File.Exists(backingFile))
            {
                return false;
            }


            using (var reader = new StreamReader(backingFile, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (long.TryParse(line, out var newcount))
                    {
                        if (id == newcount)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void ChatBotAnswer(BotsLongPollHistoryResponse poll)
        {
            foreach (var a in poll.Updates)
            {
                if (a.Type == GroupUpdateType.MessageNew && a.MessageNew.Message.UserId != VkApiClient.UserId)
                {
                    var msg = a.MessageNew.Message;
                    var text = "";
                    var withKeyboard = false;
                    var massageText = msg.Text.ToLower();

                    var userId = a.MessageNew.Message.FromId;
                    //Check user's ID in file, responds with message of existance and info how to triggers him then saves user's ID
                    //otherwise ignore new message 
                    if (userId.Value != 0 && !ReadIdAsync(userId.Value).Result)
                    {
                        SaveId(userId.Value);
                        text = "У нас в сообществе есть чат-бот! Для быстрого ответа вы можете обратиться к нему, введя в окошко с сообщением \"чатбот\" без ковычек.";
                        SendMessageBack(a.MessageNew.Message, text);
                        break;
                    }
                    
                    if (massageText.Contains("чатбот") && massageText.Length < 9)
                    {
                        text = "Вас приветствует чат-бот :) Какую операцию Вы бы хотели совершить?";
                        withKeyboard = true;
                        SendMessageBack(msg, text, withKeyboard);
                        break;
                    }

                    switch (massageText)
                    {
                        case "заказать обувь на пошив":
                            text = AnswersLibrary.VkBotAnswers[0];
                            break;
                        case "узнать какие пары есть в наличии":
                            text =  AnswersLibrary.VkBotAnswers[1];
                            break;
                        case "узнать о статусе готовности заказа":
                            text = AnswersLibrary.VkBotAnswers[2];
                            break;
                        case "подробнее о сервисе animashoes":
                            text = AnswersLibrary.VkBotAnswers[3];
                            break;
                        case "связаться с сотрудником":
                            text = AnswersLibrary.VkBotAnswers[4];
                            break;
                    }

                    if (text == "") break;

                    SendMessageBack(msg, text, withKeyboard);
                }
            }
        }

    }

    
}
