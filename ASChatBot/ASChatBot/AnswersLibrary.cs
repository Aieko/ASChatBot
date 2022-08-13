using System;
using Newtonsoft.Json;
using System.IO;

namespace ASChatBot
{
    public class AnswersLibrary
    {
        private const string EMAIL_BOT_ANSWERS_FILE_NAME = "EmailBotAnswersInfo.json";
        private const string VK_BOT_ANSWERS_FILE_NAME = "VkBotAnswersInfo.json";
        private const int NUMBER_OF_VK_BOT_ANSWERS = 5;
        private const int NUMBER_OF_EMAIL_BOT_ANSWERS = 9;

        public static string[] VkBotAnswers;

        public static string[] EmailBotAnswers;

        internal static readonly string[] VkBotDefaultAnswers = new string[NUMBER_OF_VK_BOT_ANSWERS];

        internal static readonly string[] EmailBotDefaultAnswers = new string[9];

       

        static AnswersLibrary()
        {
            //DEFAULT Answers for VK BOT
            VkBotDefaultAnswers[0] = "Мы можем сшить Вам как стандартный размер модели, так и размер по Вашим меркам :) инструкцию по снятию мерок Вы можете найти по данной ссылке: https://vk.com/topic-106741070_32852566"
                        + "\nЕсли у Вас есть такая возможность, то просим отправить нам фото язычка Вашей спортивной обуви." + "\n\nВсе модели, которые мы можем сшить, представлены по данной ссылке : https://vk.com/market-106741070?section=album_2"
                        + "\n\nПалитру Вы можете посмотреть по данной ссылке: https://vk.com/anima.shoes?z=album-106741070_240968816"
                        + "\n\nМы так же можем утеплить Вашу обувь: У нас есть 3 типа утепления: байка(-10), иск. мех(-20) и тинсулейт(-30).";
            VkBotDefaultAnswers[1] = "Пары в наличии Вы можете посмотреть по данной ссылке : https://vk.com/market-106741070?section=album_3 или на сайте https://animashoes.com/presence"
                        + "\nНа сайте есть специальные фильтры для поиска по размеру/цвету/сезону и т.д, так что советуем смотреть наличие на сайте, это намного удобнее!" +
                        "\nЕсли же Вам комфортнее смотреть модели в ВК, то при переходе по ссылке в информации товара, при нажатии на модель, находится раздел *В НАЛИЧИИ*. В нем записаны все размеры, которые уже отшиты и присутствуют на нашем складе.";
            VkBotDefaultAnswers[2] = "Мы отправили заявку на статус заказа, наш сотрудник отпишется Вам как только производство ответит нам. Спасибо за ожидание! :)";
            VkBotDefaultAnswers[3] = "Более подробную информацию о нашем магазине Вы можете найти по данной ссылке : https://vk.com/topic-106741070_32852558";
            VkBotDefaultAnswers[4] = "Заявка отправлена! Примерное время ожидания ответа в рабочее время 5-30 минут. Мы работаем каждый день, кроме понедельника. Понедельник - выходной."
                        + "\nВремя работы магазина с 12:00 до 20:00.";
            //DEFAULT Answers for EMAIL BOT
            EmailBotDefaultAnswers[0] = "Здравствуйте! Спасибо за заказ. Сошьем для Вас {modelName} в {size} размере, цвет - {color}, подклад - {insides}."
                + "\n\rДля пошива модели на заказ необходимо, как минимум, внести аванс - половину стоимости заказа.\nСумма перевода для аванса - {price}р."
                + "\nВы также можете перевести всю сумму сразу.\nСумма пошива модели {price}р + сумма доставки {deliveryPrice}р.\nИтого: {deliveryPrice + price}р."
                + "\nПереведите аванс на сумму {price / 2}р или полную стоимость {price + deliveryPrice}р по номеру: 8(981)856-22-14\nСбербанк или Тинькофф, Анна Анатольевна К., чтобы подтвердить заказ.\n\r"
                + "ВАЖНО. После оплаты обязательно отправьте скриншот экрана с информацией о переводе.";

            EmailBotDefaultAnswers[1] = "Здравствуйте! Спасибо за заказ. Сошьем для Вас {modelName} в {size} размере, цвет - {color}, подклад - {insides}."
                        + "\nПримерные сроки готовности 10-14 дней. По готовности отпишемся и пригласим Вас на примерку в шоурум.";

            EmailBotDefaultAnswers[2] = "Здравствуйте! Спасибо за заказ. Сошьем для Вас {modelName} в {size} размере, цвет - {color}, подклад - {insides}."
                        + "\nПримерные сроки готовности 10-14 дней. По готовности свяжемся с Вами, чтобы договориться о дате и времени доставки.";

            EmailBotDefaultAnswers[3] = "Здравствуйте! Спасибо за заказ. Сошьем для Вас {modelName} в {size} размере, цвет - {color}, подклад - {insides}."
                        + "\nПримерные сроки готовности 10-14 дней. По готовности свяжемся с Вами и скинем трек-номер доставки.";

            EmailBotDefaultAnswers[4] = "Здравствуйте! Спасибо за заказ {modelName} в {size} размере, цвет - {color}, подклад - {insides}." +
                "\n\rДля бронирования модели из наличия необходимо внести полную стоимость.\nСумма перевода - {price}р"
                + " + сумма доставки {deliveryPrice}р.\nИтого: {deliveryPrice + price}р."
                + "\nПереведите оплату на сумму {price + deliveryPrice}р по номеру: 8(981)856-22-14\nСбербанк или Тинькофф, Анна Анатольевна К., чтобы подтвердить заказ.\n\r"
                + "ВАЖНО. После оплаты обязательно отправьте скриншот экрана с информацией о переводе.";

            EmailBotDefaultAnswers[5] = "Здравствуйте! Спасибо за заказ {modelName} в {size} размере, цвет - {color}, подклад - {insides}.\n\rТак как вы выбрали тип оплаты - {paymentType}, то время бесплатного бронирования - 1 день."
               + "\nВы можете оплатить бронь модели через мобильный банк и прийти на примерку в любое удобное для Вас время. Если модель не подойдет, то мы вернем всю сумму."
               + "\n\rСумма перевода - {price}р + сумма доставки {deliveryPrice}р.\nИтого: {deliveryPrice + price}р."
               + "\nПереведите оплату на сумму {price + deliveryPrice}р по номеру: 8(981)856-22-14\nСбербанк или Тинькофф, Анна Анатольевна К., чтобы подтвердить заказ.\n\r"
               + "После оплаты обязательно отправьте скриншот экрана с информацией о переводе.";

            EmailBotDefaultAnswers[6] = "Здравствуйте! Спасибо за заказ {modelName} в {size} размере, цвет - {color}, подклад - {insides}." +
                "\n\rПриходите на примерку в наш магазин по адресу - Лиговский проспект, д. 5. Рабочее время 12:00-20:00, понедельник - выходной.";

            EmailBotDefaultAnswers[7] = "Здравствуйте! Спасибо за заказ {modelName} в {size} размере, цвет - {color}, подклад - {insides}." +
                "\n\rВ течение 24 часов мы свяжемся с Вами, чтобы договориться о дате и времени доставки.";

            EmailBotDefaultAnswers[8] = "Здравствуйте! Спасибо за заказ {modelName} в {size} размере, цвет - {color}, подклад - {insides}." +
                "\n\rВ течение 24 часов мы оформим и скинем трек-номер доставки.";


            VkBotAnswers = new string[NUMBER_OF_VK_BOT_ANSWERS];

            EmailBotAnswers = new string[NUMBER_OF_EMAIL_BOT_ANSWERS];

            VKBotSetDefaultOrGetCustomAnswers();

            EmailBotSetDefaultOrGetCustomAnswers();

        }

        private static void EmailBotSetDefaultOrGetCustomAnswers()
        {
            var answersInfo = new App.EmailBotAnswers(NUMBER_OF_EMAIL_BOT_ANSWERS);
            string fileName = EMAIL_BOT_ANSWERS_FILE_NAME;
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            if (backingFile == null || !File.Exists(backingFile))
            {
                for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
                {
                    EmailBotAnswers[i] = EmailBotDefaultAnswers[i];
                }

                for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
                {
                    answersInfo.answer[i] = EmailBotAnswers[i];
                }

                using (StreamWriter file = File.CreateText(backingFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, answersInfo);
                };
            }
            else
            {
                using (StreamReader file = File.OpenText(backingFile))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        answersInfo = (App.EmailBotAnswers)serializer.Deserialize(file, typeof(App.EmailBotAnswers));

                        for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
                        {
                            EmailBotAnswers[i] = answersInfo.answer[i];
                        }

                        file.Dispose();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        public static void SaveEmailBotAnswers(string[] answers)
        {
            if (NUMBER_OF_EMAIL_BOT_ANSWERS != answers.Length)
            {
                throw new IndexOutOfRangeException();
            }

            string fileName = EMAIL_BOT_ANSWERS_FILE_NAME;
            var answersInfo = new App.EmailBotAnswers(NUMBER_OF_EMAIL_BOT_ANSWERS);
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
            {
                answersInfo.answer[i] = answers[i];
            }

            using (StreamWriter file = File.CreateText(backingFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, answersInfo);
            };

            for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
            {
                EmailBotAnswers[i] = answers[i];
            }
        }

        public static void ResetEmailBotAnswers()
        {
            var answersInfo = new App.EmailBotAnswers(NUMBER_OF_EMAIL_BOT_ANSWERS);
            string fileName = EMAIL_BOT_ANSWERS_FILE_NAME;
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            if (!File.Exists(backingFile))
            {
                throw new NotImplementedException();
            }

            File.Delete(backingFile);

            for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
            {
                EmailBotAnswers[i] = EmailBotDefaultAnswers[i];
            }

            for (int i = 0; i < NUMBER_OF_EMAIL_BOT_ANSWERS; i++)
            {
                answersInfo.answer[i] = EmailBotAnswers[i];
            }

            using (StreamWriter file = File.CreateText(backingFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, answersInfo);
            };
        }

        public static string MessageFormat(string text, ref OrderInformation orderInfo)
        {

            var answer = text.Replace("{modelName}", orderInfo.modelName).Replace("{size}", orderInfo.size).Replace("{color}", orderInfo.color)
                .Replace("{insides}", orderInfo.insides).Replace("{name}", orderInfo.name).Replace("{email}", orderInfo.email)
                .Replace("{phone}", orderInfo.phone).Replace("{delivery}", orderInfo.delivery).Replace("{adress}", orderInfo.adress)
                .Replace("{price}", orderInfo.price.ToString()).Replace("{deliveryPrice}", orderInfo.deliveryPrice.ToString())
                .Replace("{price / 2}", (orderInfo.price / 2).ToString()).Replace("{deliveryPrice + price}", (orderInfo.deliveryPrice + orderInfo.price).ToString())
                .Replace("{price + deliveryPrice}", (orderInfo.deliveryPrice + orderInfo.price).ToString());

            return answer;
        }

        private static void VKBotSetDefaultOrGetCustomAnswers()
        {
            var answersInfo = new App.VkBotAnswers(NUMBER_OF_VK_BOT_ANSWERS);
            string fileName = VK_BOT_ANSWERS_FILE_NAME;
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            if (backingFile == null || !File.Exists(backingFile))
            {
                for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
                {
                    VkBotAnswers[i] = VkBotDefaultAnswers[i];
                }

                for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
                {
                    answersInfo.answer[i] = VkBotAnswers[i];
                }

                using (StreamWriter file = File.CreateText(backingFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, answersInfo);
                };
            }
            else
            {
                using (StreamReader file = File.OpenText(backingFile))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        answersInfo = (App.VkBotAnswers)serializer.Deserialize(file, typeof(App.VkBotAnswers));

                        for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
                        {
                            VkBotAnswers[i] = answersInfo.answer[i];
                        }

                        file.Dispose();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        public static void SaveVkBotAnswers(string[] answers)
        {

            if (NUMBER_OF_VK_BOT_ANSWERS != answers.Length)
            {
                throw new IndexOutOfRangeException();
            }

            string fileName = VK_BOT_ANSWERS_FILE_NAME;
            var answersInfo = new App.VkBotAnswers(VkBotAnswers.Length);

            for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
            {
                answersInfo.answer[i] = answers[i];
            }

            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, answersInfo);
            };

            for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
            {
                VkBotAnswers[i] = answers[i];
            }
        }

        public static void ResetVkBotAnswers()
        {
            var answersInfo = new App.VkBotAnswers(5);
            string fileName = VK_BOT_ANSWERS_FILE_NAME;
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            File.Delete(backingFile);

            for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
            {
                VkBotAnswers[i] = VkBotDefaultAnswers[i];
            }

            for (int i = 0; i < NUMBER_OF_VK_BOT_ANSWERS; i++)
            {
                answersInfo.answer[i] = VkBotAnswers[i];
            }

            using (StreamWriter file = File.CreateText(backingFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, answersInfo);
            };
        }

   
    }
}