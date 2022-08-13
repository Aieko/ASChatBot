using System;

using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;


using Plugin.Connectivity;

namespace ASChatBot
{
    public partial class MainPage : ContentPage
    {

        public Switch switcher1;
        public Switch switcher3;

        private Button groupInfoButton;
        private Button emailInfoButton;

        public MainPage()
        {
            InitializeComponent();

            //CreateUserInterface();

        }

        private void CreateUserInterface()
        {

            var image = new Xamarin.Forms.Image
            { 
                Source = "logo.png",
                HorizontalOptions = LayoutOptions.Center
            };

            Label bot1Text = new Label
            {
                Text = "ВК бот - автоответчик",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            switcher1 = new Switch
            {
                Margin = new Thickness(0, -30),
                IsToggled = false,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };

            groupInfoButton = new Xamarin.Forms.Button { Text = "Подключение для ВК бота", Margin = new Thickness(0, 30) };

            groupInfoButton.Clicked += OpenGroupConnectionInfoPage;

            Label bot3Text = new Label
            {
                Text = "Email - автоответчик",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            switcher3 = new Switch
            {
                Margin = new Thickness(0, -30),
                IsToggled = false,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };

            emailInfoButton = new Xamarin.Forms.Button { Text = "Подключение для почты-автоответчика", Margin = new Thickness(0, 30) };

            emailInfoButton.Clicked += OpenEmailConenctionInfoPage;

            var refreshButton = new Xamarin.Forms.Button { Text = "Обновить информацию", Margin = new Thickness(0, 30) };

            this.Content = new StackLayout { Padding = new Thickness(10, 25), Children = { bot1Text, switcher1, groupInfoButton, bot3Text, switcher3, emailInfoButton, refreshButton, image } };

        }

        private void OpenGroupConnectionInfoPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GroupConnectionPage());
        }

        private void OpenEmailConenctionInfoPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EmailConnectionPage());
        }



    }
}
