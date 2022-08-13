using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace ASChatBot.Droid
{
    [Activity(Label = "Ответ Email-бота")]
    public class EmailBotAnswersInfoActivity : Activity
    {
        private EditText mobileTransferEntry;
        private EditText roboPickupEntry;
        private EditText roboCourierEntry;
        private EditText roboDeliveryEntry;

        private EditText mobileTransferFromStockEntry;
        private EditText cashPaymentFromStockEntry;
        private EditText roboPickupFromStockEntry;
        private EditText roboCourierFromStockEntry;
        private EditText roboDeliveryFromStockEntry;

        private Button saveAnswersButton;
        private Button restartAnswersButton;
        private Button helpButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EmailBotAnswersInfo_Layout);

            mobileTransferEntry = (EditText)FindViewById(Resource.Id.mobileTransferEntry);
            roboPickupEntry = (EditText)FindViewById(Resource.Id.roboPickupEntry);
            roboCourierEntry = (EditText)FindViewById(Resource.Id.roboCourierEntry);
            roboDeliveryEntry = (EditText)FindViewById(Resource.Id.roboDeliveryEntry);

            mobileTransferFromStockEntry = (EditText)FindViewById(Resource.Id.mobileTransferFromStockEntry);
            cashPaymentFromStockEntry = (EditText)FindViewById(Resource.Id.cashPaymentFromStockEntry);
            roboPickupFromStockEntry = (EditText)FindViewById(Resource.Id.roboPickupFromStockEntry);
            roboCourierFromStockEntry = (EditText)FindViewById(Resource.Id.roboCourierFromStockEntry);
            roboDeliveryFromStockEntry = (EditText)FindViewById(Resource.Id.roboDeliveryFromStockEntry);

            saveAnswersButton = (Button)FindViewById(Resource.Id.saveAnswersButton);
            restartAnswersButton = (Button)FindViewById(Resource.Id.restartAnswersButton);
            helpButton = (Button)FindViewById(Resource.Id.helpButton);

            saveAnswersButton.Click += SaveAnswersButton_Click;
            restartAnswersButton.Click += RestartAnswersButton_Click;
            helpButton.Click += HelpButton_Click;

           RefreshAnswers();

            // Create your application here
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            Helper.DisplayAlert("Справка",
                "Внимательно прочитайте данный мануал по написанию своих ответов."
                + "\n Вы можете вставлять в сообщение различную информацию из заказа." +
                "\n Для этого нужно вписать определенный текст в ваше сообщение, этот текст должен быть заключен в скобки - {}" +
                "\n возможные данные из заказа:" +
                "\n{modelName} - название модели, {name} - ФИО клиента," +
                "\n{email} - электронная почта клиента, {phone} - номер телефона клиента" +
                "\n{size} - размер заказанной обуви, {color} - цвет обуви" +
                "\n{insides} - утепление, {delivery} - вид доставки" +
                "\n{adress} - адрес доставки, {payment} - вид оплаты" +
                "\n{price} - стоимость заказа, {deliveryPrice} - стоимость доставки" +
                "\n{price + deliveryPrice} - общая сумма заказа", "Окей", this);
        }

        private void RefreshAnswers()
        {
            mobileTransferEntry.Text = AnswersLibrary.EmailBotAnswers[0];
            roboPickupEntry.Text = AnswersLibrary.EmailBotAnswers[1];
            roboCourierEntry.Text = AnswersLibrary.EmailBotAnswers[2];
            roboDeliveryEntry.Text = AnswersLibrary.EmailBotAnswers[3];
            mobileTransferFromStockEntry.Text = AnswersLibrary.EmailBotAnswers[4];
            cashPaymentFromStockEntry.Text = AnswersLibrary.EmailBotAnswers[5];
            roboPickupFromStockEntry.Text = AnswersLibrary.EmailBotAnswers[6];
            roboCourierFromStockEntry.Text = AnswersLibrary.EmailBotAnswers[7];
            roboDeliveryFromStockEntry.Text = AnswersLibrary.EmailBotAnswers[8];
        }

        private void RestartAnswersButton_Click(object sender, EventArgs e)
        {
            AnswersLibrary.ResetEmailBotAnswers();

            RefreshAnswers();

            Helper.DisplayAlert("Внимание", "Ответы Email-бота были сброшены до дефолтных", "Ок", this);
        }

        private void SaveAnswersButton_Click(object sender, EventArgs e)
        {
            bool allEntriesFilled = true;

            if (mobileTransferEntry.Text == "" ||
            roboPickupEntry.Text == "" ||
            roboCourierEntry.Text == "" ||
            roboDeliveryEntry.Text == "" ||
            mobileTransferFromStockEntry.Text == "" ||
            cashPaymentFromStockEntry.Text == "" ||
            roboPickupFromStockEntry.Text == "" ||
            roboCourierFromStockEntry.Text == "" ||
            roboDeliveryFromStockEntry.Text == "") allEntriesFilled = false;

            if (!allEntriesFilled)
            {
                Helper.DisplayAlert("Что-то пошло не так", "Заполните все поля c ответами, чтобы сохранить информацию", "Ок", this);
                return;
            }

            SaveAnswersInfo();
        }

        private void SaveAnswersInfo()
        {
            var answers = new string[9];
            answers[0] = mobileTransferEntry.Text;
            answers[1] = roboPickupEntry.Text;
            answers[2] = roboCourierEntry.Text;
            answers[3] = roboDeliveryEntry.Text;

            answers[4] = mobileTransferFromStockEntry.Text;
            answers[5] = cashPaymentFromStockEntry.Text;
            answers[6] = roboPickupFromStockEntry.Text;
            answers[7] = roboCourierFromStockEntry.Text;
            answers[8] = roboDeliveryFromStockEntry.Text;

            AnswersLibrary.SaveEmailBotAnswers(answers);

            Helper.DisplayAlert("Внимание", "Ответы Email-бота были сохранены", "Ок", this);
        }
    }
}