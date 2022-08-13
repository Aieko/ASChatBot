using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace ASChatBot.Droid
{
    [Activity(Label = "Ответы ВК-бота")]
    public class VKBotAnswersInfoActivity : Activity
    {
        private EditText firstButtonEntry;
        private EditText secondButtonEntry;
        private EditText thirdButtonEntry;
        private EditText fourthButtonEntry;
        private EditText fithButtonEntry;

        private Button saveAnswersButton;
        private Button restartAnswersButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.VKBotAnswersInfo_Layout);

            firstButtonEntry = (EditText)FindViewById(Resource.Id.firstButtonEntry);
            secondButtonEntry = (EditText)FindViewById(Resource.Id.secondButtonEntry);
            thirdButtonEntry = (EditText)FindViewById(Resource.Id.thirdButtonEntry);
            fourthButtonEntry = (EditText)FindViewById(Resource.Id.fourthButtonEntry);
            fithButtonEntry = (EditText)FindViewById(Resource.Id.fithButtonEntry);

            saveAnswersButton = (Button)FindViewById(Resource.Id.saveAnswersButton);
            restartAnswersButton = (Button)FindViewById(Resource.Id.restartAnswersButton);

            saveAnswersButton.Click += SaveAnswersButton_Click;
            restartAnswersButton.Click += RestartAnswersButton_Click;


            ReadAnswersInfo();
        }


        private void RestartAnswersButton_Click(object sender, EventArgs e)
        {
            AnswersLibrary.ResetVkBotAnswers();

            firstButtonEntry.Text = AnswersLibrary.VkBotAnswers[0];
            secondButtonEntry.Text = AnswersLibrary.VkBotAnswers[1];
            thirdButtonEntry.Text = AnswersLibrary.VkBotAnswers[2];
            fourthButtonEntry.Text = AnswersLibrary.VkBotAnswers[3];
            fithButtonEntry.Text = AnswersLibrary.VkBotAnswers[4];

            Helper.DisplayAlert("Внимание", "Ответы Вк-бота были сброшены до дефолтных", "Ок", this);
        }

        private void SaveAnswersButton_Click(object sender, EventArgs e)
        {
            bool allEntriesFilled = true;

            if (firstButtonEntry.Text == "" ||
            secondButtonEntry.Text == "" ||
            thirdButtonEntry.Text == "" ||
            fourthButtonEntry.Text == "" ||
            fithButtonEntry.Text == "") allEntriesFilled = false;

            if (!allEntriesFilled)
            {
                Helper.DisplayAlert("Что-то пошло не так", "Заполните все поля c ответами, чтобы сохранить информацию", "Ок", this);
                return;
            }

            SaveAnswersInfo();
        }


        private void SaveAnswersInfo()
        {
            var answers = new string[5];
            answers[0] = firstButtonEntry.Text;
            answers[1] = secondButtonEntry.Text;
            answers[2] = thirdButtonEntry.Text;
            answers[3] = fourthButtonEntry.Text;
            answers[4] = fithButtonEntry.Text;

            AnswersLibrary.SaveVkBotAnswers(answers);

            Helper.DisplayAlert("Внимание", "Ответы Вк-бота были сохранены", "Ок", this);
        }

        public void ReadAnswersInfo()
        {
            firstButtonEntry.Text = AnswersLibrary.VkBotAnswers[0];
            secondButtonEntry.Text = AnswersLibrary.VkBotAnswers[1];
            thirdButtonEntry.Text = AnswersLibrary.VkBotAnswers[2];
            fourthButtonEntry.Text = AnswersLibrary.VkBotAnswers[3];
            fithButtonEntry.Text = AnswersLibrary.VkBotAnswers[4];

        }
    }
}