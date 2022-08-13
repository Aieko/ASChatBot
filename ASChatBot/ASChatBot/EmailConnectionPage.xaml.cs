using System;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ASChatBot
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmailConnectionPage : ContentPage
	{
        private StackLayout layout;
        private Entry emailEntry;
        private Entry securityKeyEntry;

        private Button resetButton;
        private Button saveButton;

        private bool emailInfoFound = false;

        public EmailConnectionPage ()
		{
            InitializeComponent();
            CreateEntries();
            ReadInfo();
            CreateButtons();
        }

        private void CreateEntries()
        {
            Label TokenText = new Label
            {
                Text = "Адрес электронный почты",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            emailEntry = new Entry { Text = "", Placeholder = "Пример : example@gmail.com" };

            Label GroupIdText = new Label
            {
                Text = "Ключ безопаности(пароль) приложения",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            securityKeyEntry = new Entry { Text = "", Placeholder = "Пример : xqsdvhfpwxgojlft" };

            layout = new StackLayout { Padding = new Thickness(10, 25), Children = { TokenText, emailEntry, GroupIdText, securityKeyEntry } };


            this.Content = layout;

        }

        private void CreateButtons()
        {
            saveButton = new Button { Text = "Сохранить", Margin = new Thickness(0, 60) };

            saveButton.Clicked += SaveButton_Clicked;

            resetButton = new Button { Text = "Сбросить информацию", Margin = new Thickness(0, 60) };

            resetButton.Clicked += ResetButton_Clicked;

            resetButton.IsVisible = false;

            if (emailInfoFound)
            {
                resetButton.IsVisible = true;
                saveButton.IsVisible = false;
            }

            layout.Children.Add(saveButton);
            layout.Children.Add(resetButton);
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            emailEntry.IsReadOnly = false;
            securityKeyEntry.IsReadOnly = false;

            resetButton.IsVisible = false;
            saveButton.IsVisible = true;

            emailEntry.Text = "";
            securityKeyEntry.Text = "";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (emailEntry.Text == "" || securityKeyEntry.Text == "")
            {
                DisplayAlert("Что-то пошло не так", "Заполните все поля, чтобы сохранить информацию", "Ок");
                return;
            }

            SaveInfo();
        }

        private void SaveInfo()
        {
            string fileName = "EmailInfo.json";
            var emailInfo = new App.EmailInfo(emailEntry.Text, securityKeyEntry.Text);
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, emailInfo);
            };

            DisplayAlert("Информация была добавлена", "Перейдите на основную страницу приложения и нажмите кнопку *Обновить информацию* для того, чтобы изменения вступили в силу", "Понимаю");
        }

        public void ReadInfo()
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
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        emailInfo = (App.EmailInfo)serializer.Deserialize(file, typeof(App.EmailInfo));

                        emailEntry.Text = emailInfo.Email;
                        emailEntry.IsReadOnly = true;
                        securityKeyEntry.Text = emailInfo.SecurityKey;
                        securityKeyEntry.IsReadOnly = true;

                        emailInfoFound = true;
                        file.Dispose();
                    }
                    catch
                    {
                        DisplayAlert("Первый запуск?", "Введите информацию в поля.\nВаш email на который будут приходить письма и откуда они будут исходить\nЕсли у вас нет ключа безопаности(пароля приложения), то обратитесь по этому поводу к своему системному администратору. ", "Понимаю..");
                    }
                }


            }
        }
    }
}