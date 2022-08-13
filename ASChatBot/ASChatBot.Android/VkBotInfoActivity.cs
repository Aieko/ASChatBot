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
    [Activity(Label = "Данные для VK-бота")]
    public class VkBotInfoActivity : Activity
    {
        private EditText tokenEntry;
        private EditText groupIdEntry;

        private Button resetButton;
        private Button saveButton;
        private Button answersButton;


        private bool groupInfoFound = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.VKBotInfo_Layout);

            saveButton = (Button)FindViewById(Resource.Id.saveButton);
            resetButton = (Button)FindViewById(Resource.Id.resetButton);
            answersButton = (Button)FindViewById(Resource.Id.answersButton);

            tokenEntry = (EditText)FindViewById(Resource.Id.vkTokenEntry);
            groupIdEntry = (EditText)FindViewById(Resource.Id.vkIdEntry);
        
            saveButton.Click += SaveButton_Clicked;
            resetButton.Click += ResetButton_Clicked;
            answersButton.Click += AnswersButton_Click;

            ReadGroupInfo();
            

            if (groupInfoFound)
            {
                resetButton.Visibility = Android.Views.ViewStates.Visible;
                saveButton.Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        private void AnswersButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(VKBotAnswersInfoActivity));
            StartActivity(intent);
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            tokenEntry.Focusable = true;
            groupIdEntry.Focusable = true;

            resetButton.Visibility = Android.Views.ViewStates.Invisible;
            saveButton.Visibility = Android.Views.ViewStates.Visible;

            tokenEntry.Text = "";
            groupIdEntry.Text = "";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (tokenEntry.Text == "" || groupIdEntry.Text == "")
            {
                Helper.DisplayAlert("Что-то пошло не так", "Заполните все поля, чтобы сохранить информацию", "Ок", this);
                return;
            }

            SaveGroupInfo();
        }

        private void SaveGroupInfo()
        {
            string fileName = "GroupInfo.json";
            var groupInfo = new App.GroupInfo(tokenEntry.Text, groupIdEntry.Text);
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, groupInfo);
            };

            Helper.DisplayAlert("Информация была добавлена",
                "Перейдите на основную страницу приложения и нажмите кнопку *Обновить информацию* для того, чтобы изменения вступили в силу",
                "Понимаю",
                 this);
        }

        public void ReadGroupInfo()
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
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        groupInfo = (App.GroupInfo)serializer.Deserialize(file, typeof(App.GroupInfo));

                        tokenEntry.Text = groupInfo.Token;
                        tokenEntry.Focusable = false;
                        groupIdEntry.Text = groupInfo.GroupID;
                        groupIdEntry.Focusable = false;

                        groupInfoFound = true;
                        file.Dispose();
                    }
                    catch
                    {
                        Helper.DisplayAlert("Первый запуск?",
                            "Введите информацию в поля. Ключ можно создать в настройках группы, id группы - набор цифр в адресе группы",
                            "Понимаю..",
                             this);
                    }
                }


            }
        }
    }
}