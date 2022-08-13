using System;
using Newtonsoft.Json;
using System.IO;
using Button = Android.Widget.Button;
using EditText = Android.Widget.EditText;
using Android.App;
using Android.OS;
using Android.Content;

namespace ASChatBot.Droid
{
    [Activity(Label = "Данные для email-бота")]
    public class EmailBotInfoActivity : Activity
    {
        private Button resetButton;
        private Button saveButton;
        private Button answersButton;

        private EditText emailEntry;
        private EditText securityKeyEntry;

        private bool emailInfoFound = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EmailBotInfo_Layout);

            saveButton = (Button)FindViewById(Resource.Id.saveButton);
            resetButton = (Button)FindViewById(Resource.Id.resetButton);
            answersButton = (Button)FindViewById(Resource.Id.answersButton);

            emailEntry = (EditText)FindViewById(Resource.Id.emailEntry);
            securityKeyEntry = (EditText)FindViewById(Resource.Id.securityKeyEntry);

            saveButton.Click += SaveButton_Clicked;
            resetButton.Click += ResetButton_Clicked;
            answersButton.Click += AnswersButton_Click;

            ReadInfo();

            if (emailInfoFound)
            {
                resetButton.Visibility = Android.Views.ViewStates.Visible;
                saveButton.Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        private void AnswersButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(EmailBotAnswersInfoActivity));
            StartActivity(intent);
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            emailEntry.Focusable = true;
            securityKeyEntry.Focusable = true;

            resetButton.Visibility = Android.Views.ViewStates.Invisible;
            saveButton.Visibility = Android.Views.ViewStates.Visible;

            emailEntry.Text = "";
            securityKeyEntry.Text = "";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (emailEntry.Text == "" || securityKeyEntry.Text == "")
            {
                Helper.DisplayAlert("Что-то пошло не так", "Заполните все поля, чтобы сохранить информацию", "Ок",this);
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

            Helper.DisplayAlert("Информация была добавлена",
                "Перейдите на основную страницу приложения и нажмите кнопку *Обновить информацию* для того, чтобы изменения вступили в силу",
                "Понимаю"
                , this);
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
                        emailEntry.Focusable = false;
                        securityKeyEntry.Text = emailInfo.SecurityKey;
                        securityKeyEntry.Focusable = false;

                        emailInfoFound = true;
                        file.Dispose();
                    }
                    catch
                    {
                        Helper.DisplayAlert("Первый запуск?",
                            "Введите информацию в поля.\nВаш email на который будут приходить письма и откуда они будут исходить\nЕсли у вас нет ключа безопаности(пароля приложения), то обратитесь по этому поводу к своему системному администратору. ",
                            "Понимаю..",
                            this);
                    }
                }


            }
        }
    }
}