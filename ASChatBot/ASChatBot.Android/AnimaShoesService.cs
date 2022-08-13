using Android.OS;
using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Plugin.Connectivity;
using System;
using System.IO;
using Path = System.IO.Path;
using Newtonsoft.Json;
using Android.Graphics;

namespace ASChatBot.Droid
{
    public class StartServiceAndroid : IStartService
    {
        public void StartForegroundServiceCompat()
        {
            var intent = new Intent(MainActivity.Instance, typeof(AnimaShoesService));


            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                MainActivity.Instance.StartForegroundService(intent);
            }
            else
            {
                MainActivity.Instance.StartService(intent);
            }
            
        }
    }

    [Service(Enabled = true, Exported = false)]
    [IntentFilter(new[] {
        "ru.animashoesapp.service.action.stopvkbot",
        "ru.animashoesapp.service.action.stopmailbot",
        "ru.animashoesapp.service.action.runvkbot",
        "ru.animashoesapp.service.action.runmailbot",
        "ru.animashoesapp.service.action.stopservice",
        "ru.animashoesapp.service.action.refreshinfo"},

       Categories = new[] { Intent.CategoryDefault })]
    class AnimaShoesService : Service
    {
        private bool isStarted;

        private const string NOTIFICATION_CHANNEL_ID = "1003";
        private const string NOTIFICATION_CHANNEL_NAME = "MyChannel";
        private const int NOTIFICATION_SERVICE_ID = 1001;

        private string myVkGroupToken = "";
        private ulong myVkGroupId = 0;

        private string myEmail;
        private string myEmailSecurityKey;

        private static Task chatBotTask;
        private static Task emailBotTask;

        public static VkBot VkBotClient { get; private set; }
        public static MailBot MailBotClient { get; private set; }

        public bool VkBotOn { get; private set; }
        public bool MailBotOn { get; private set; }

        public static AnimaShoesService Instance { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();

            if (Instance == null)
            {
                Instance = this;
            }
        }

        public bool ConnectionCheck()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            return false;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isStarted) //Service is already started
            {

                switch (intent.Action)
                {
                    case "ru.animashoesapp.service.action.stopvkbot":
                        VkBotOn = false;
                        VkBotClient?.Dispose();
                        DispatchNotificationThatServiceIsRunning();

                        MainActivity.Instance?.VkBotToggleButtonSwitch(false);

                        break;
                    case "ru.animashoesapp.service.action.stopmailbot":
                        MailBotClient?.done?.Cancel();
                        MailBotClient?.Cancel?.Cancel();
                        MailBotOn = false;
                        DispatchNotificationThatServiceIsRunning();
                        MainActivity.Instance?.MailBotToggleButtonSwitch(false);
                        break;
                    case "ru.animashoesapp.service.action.runvkbot":

                        if (!VkGroupInfoCheck())
                        {
                            MainActivity.Instance?.VkBotToggleButtonSwitch(false);
                            break;
                        }
                        
                        ChatBotStart();
                        VkBotOn = true;
                        MainActivity.Instance?.VkBotToggleButtonSwitch(true);
                        DispatchNotificationThatServiceIsRunning();
                        break;
                    case "ru.animashoesapp.service.action.runmailbot":
                        if (!EmailInfoCheck())
                        {
                            MainActivity.Instance?.MailBotToggleButtonSwitch(false);
                            break;
                        }
                        EmailBotStart();
                        MailBotOn = true;
                        MainActivity.Instance?.MailBotToggleButtonSwitch(true);
                        DispatchNotificationThatServiceIsRunning();
                        break;
                    case "ru.animashoesapp.service.action.stopservice":
                        VkBotClient?.Dispose();
                        MainActivity.Instance?.VkBotToggleButtonSwitch(true);
                        MailBotClient?.Dispose();
                        MainActivity.Instance?.MailBotToggleButtonSwitch(false);
                        StopService(intent);
                        break;
                    case "ru.animashoesapp.service.action.refreshinfo":
                        RefreshInfo();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    CreateNotificationChannel();

                    DispatchNotificationThatServiceIsRunning(out NotificationCompat.Builder builder);

                    StartForeground(NOTIFICATION_SERVICE_ID, builder.Build());

                    RefreshInfo();

                    isStarted = true;
                }
            }
            return StartCommandResult.Sticky;
        }

        private void RefreshInfo()
        {
            ReadEmailBotInfo();
            ReadVkBotInfo();
        }

        private void ReadVkBotInfo()
        {
            string fileName = "GroupInfo.json";
            var groupInfo = new App.GroupInfo();
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            if (backingFile == null || !File.Exists(backingFile))
            {
                var fs = File.Create(backingFile);
                fs.Dispose();
                return;
            }
            else
            {
                using (StreamReader file = File.OpenText(backingFile))
                {
                    if (file != null)
                    {
                        try
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            groupInfo = (App.GroupInfo)serializer.Deserialize(file, typeof(App.GroupInfo));
                        }
                        catch
                        {
                            DisplayAlert("Первый запуск?", "Если это Ваш первый запуск приложения, то зайдите в настройки соединения группы и электронной почты и введите актуальную информацию", "Понимаю..");
                            file.Dispose();
                            return;

                        }

                    }

                    if (groupInfo == null)
                    {
                        DisplayAlert("Что-то пошло не так!", "Файл был найден, но информацию о VK группе не удалось найти или она была неполная", "Ок");
                        return;
                    }

                    if ((groupInfo.Token != null && groupInfo.GroupID != null)
                        || (groupInfo.Token != "" && groupInfo.GroupID != ""))
                    {
                        myVkGroupToken = groupInfo.Token;
                        myVkGroupId = ulong.Parse(groupInfo.GroupID);
                    }
                    else
                    {
                        DisplayAlert("Что-то пошло не так!", "Файл был найден, но информацию о VK группе не удалось найти или она была неполная", "Ок");
                    }

                }
            }
        }

        private void ReadEmailBotInfo()
        {
            string fileName = "EmailInfo.json";
            var emailInfo = new App.EmailInfo();
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            if (backingFile == null || !File.Exists(backingFile))
            {
                var fs = File.Create(backingFile);
                fs.Dispose();
                return;
            }
            else
            {
                using (StreamReader file = File.OpenText(backingFile))
                {
                    if (file != null)
                    {
                        try
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            emailInfo = (App.EmailInfo)serializer.Deserialize(file, typeof(App.EmailInfo));
                        }
                        catch
                        {
                            DisplayAlert("Первый запуск?", "Если это Ваш первый запуск приложения, то зайдите в настройки соединения группы и электронной почты и введите актуальную информацию", "Понимаю..");
                            file.Dispose();
                            return;

                        }

                    }

                    if (emailInfo == null)
                    {
                        DisplayAlert("Что-то пошло не так!", "Файл был найден, но информацию об электронной почте не удалось найти или она была неполная", "Ок");
                        file.Dispose();
                        return;
                    }

                    if ((emailInfo.Email != null && emailInfo.SecurityKey != null)
                        || (emailInfo.Email != "" && emailInfo.SecurityKey != ""))
                    {
                        myEmail = emailInfo.Email;
                        myEmailSecurityKey = emailInfo.SecurityKey;

                    }
                    else
                    {
                        DisplayAlert("Что-то пошло не так!", "Файл был найден, но информацию об электронной почте не удалось найти или она была неполная", "Ок");

                    }

                }
            }
        }

        private void EmailBotStart()
        {
            try
            {
                if (!ConnectionCheck()) throw new Exception("Устройство не подключено к интернету.");

                MailBotClient = new MailBot(myEmail, myEmailSecurityKey);

                emailBotTask = Task.Factory.StartNew(() => MailBotClient.Run());

            }
            catch (Exception exception)
            {
                MailBotClient.Dispose();

                DisplayAlert("Something went wrong!", exception.Message);
            }
        }

        //ChatBot triggers and responds on text requests
        private void ChatBotStart()
        {
            try
            {
                if (!ConnectionCheck()) throw new Exception("Устройство не подключено к интернету.");

                VkBotClient = new VkBot(myVkGroupToken, myVkGroupId);

                chatBotTask = Task.Factory.StartNew(() => VkBotClient.Run());

            }
            catch (Exception exception)
            {
                VkBotClient.Dispose();

                DisplayAlert("Something went wrong!", exception.Message);
            }
        }

        private void DisplayAlert(string title, string message, string buttonText = null)
        {
            if (buttonText == null) buttonText = "Ok";

            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(MainActivity.Instance);
            AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton(buttonText, (c, ev) =>
            {
                alert.Cancel();
            });
            alert.Show();
        }

        private void DispatchNotificationThatServiceIsRunning(out NotificationCompat.Builder builder)
        {
            var vkBotStatus = VkBotOn ? "ВКЛЮЧЕН" : "ОТКЛЮЧЕН";

            var mailBotStatus = MailBotOn ? "ВКЛЮЧЕН" : "ОТКЛЮЧЕН";

            builder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                   .SetDefaults((int)NotificationDefaults.All)
                   .SetSmallIcon(Resource.Drawable.as_notification_icon)
                   .SetSound(null)
                   .SetChannelId(NOTIFICATION_CHANNEL_ID)
                   .SetPriority(NotificationCompat.PriorityDefault)
                   .SetAutoCancel(false)
                   .SetContentTitle("Сервис готов к работе!")
                   .SetContentText($"ВК чат-бот {vkBotStatus} \nМейл-бот {mailBotStatus}")
                   .SetOngoing(true)
                   .SetContentIntent(MakePendingIntent(""))
                   .AddAction(Resource.Drawable.notify_panel_notification_icon_bg, "Остановить сервис", MakePendingIntent("ru.animashoesapp.service.action.stopservice"));

            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
        }

        private void DispatchNotificationThatServiceIsRunning()
        {
            var vkBotStatus = VkBotOn ? "ВКЛЮЧЕН" : "ОТКЛЮЧЕН";

            var mailBotStatus = MailBotOn ? "ВКЛЮЧЕН" : "ОТКЛЮЧЕН";

            var titleText = "";
            var builder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
            if (VkBotOn || MailBotOn) titleText = "Сервис запущен и работает";
            else
            {
                titleText = "Сервис готов к работе!";
                builder.AddAction(Resource.Drawable.notify_panel_notification_icon_bg, "Остановить сервис", MakePendingIntent("ru.animashoesapp.service.action.stopservice"));
            }

            builder.SetDefaults((int)NotificationDefaults.All)
                   .SetSmallIcon(Resource.Drawable.as_notification_icon)
                   .SetSound(null)
                   .SetChannelId(NOTIFICATION_CHANNEL_ID)
                   .SetPriority(NotificationCompat.PriorityDefault)
                   .SetAutoCancel(false)
                   .SetContentTitle(titleText)
                   .SetContentText($"ВК чат-бот {vkBotStatus} \nМейл-бот {mailBotStatus}")
                   .SetOngoing(true)
                   .SetContentIntent(MakePendingIntent(""));



            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(NOTIFICATION_SERVICE_ID,builder.Build());
        }

        private void CreateNotificationChannel()
        {
            //Notification Channel
            NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.Max);


            NotificationManager notificationManager = (NotificationManager)this.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(notificationChannel);
        }

        public PendingIntent MakePendingIntent(string name)
        {
            Intent intent = new Intent(this, typeof(AnimaShoesService));
            if (name != "") intent.SetAction(name);
            PendingIntent pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.OneShot);
            return pendingIntent;
        }

        private bool VkGroupInfoCheck()
        {
            if (myVkGroupToken == null || myVkGroupToken == "")
            {
                DisplayAlert("Что-то пошло не так", "Ключ группы не был найден, необходимо его ввести на странице информации группы.", "Ок");
                return false;
            }
            else if (myVkGroupId == 0)
            {
                DisplayAlert("Что-то пошло не так", "ID группы не было найдено, необходимо его ввести на странице информации группы.", "Ок");
                return false;
            }

            return true;
        }

        private bool EmailInfoCheck()
        {
            if (myEmail == null || myEmail == "")
            {
                DisplayAlert("Что-то пошло не так", "Email адрес не был найден, его необходимо ввести на странице информации Email.", "Ок");
                return false;
            }
            else if (myEmailSecurityKey == null || myEmailSecurityKey == "")
            {
                DisplayAlert("Что-то пошло не так", "Ключ безопаности(пароль) приложения не был найден, необходимо его ввести на странице информации Email.", "Ок");
                return false;
            }

            return true;
        }
    } 
}