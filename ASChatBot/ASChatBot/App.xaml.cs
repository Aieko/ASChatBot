using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ASChatBot
{
    public partial class App : Application
    {
        public static App Instance;

        public class GroupInfo
        {
            public GroupInfo()
            {
            }

            public GroupInfo(string token, string groupID)
            {
                Token = token;
                GroupID = groupID;
            }

            public string Token { get; set; }
            public string GroupID { get; set; }

        }

        public class EmailInfo
        {
            public EmailInfo()
            {
            }

            public EmailInfo(string email, string securityKey)
            {
                Email = email;
                SecurityKey = securityKey;
            }

            public string Email { get; set; }
            public string SecurityKey { get; set; }

        }

        public struct VkBotAnswers
        {
            public string[] answer;

            public VkBotAnswers(int numOfAnswers)
            {
                answer = new string[numOfAnswers];
            }
        }

        public struct EmailBotAnswers
        {
            public string[] answer;

            public EmailBotAnswers(int numOfAnswers)
            {
                answer = new string[numOfAnswers];
            }
        }

        public App()
        {
            InitializeComponent();
            Instance = this;
            MainPage = new NavigationPage(new MainPage());
        }

        public void OpenEmailConnectionInfoPage()
        {
            MainPage.Navigation.PushAsync(new EmailConnectionPage());
        }

        public void OpenVkGroupConnectionInfoPage()
        {
            MainPage.Navigation.PushAsync(new GroupConnectionPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

   
}
