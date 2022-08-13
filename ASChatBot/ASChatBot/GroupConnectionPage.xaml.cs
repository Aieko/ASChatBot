using System;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ASChatBot
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupConnectionPage : ContentPage
	{

        StackLayout layout;

        Entry tokenEntry;
        Entry groupIdEntry;

        Button resetButton;
        Button saveButton;

        private bool groupInfoFound = false;

        public GroupConnectionPage ()
		{
			InitializeComponent ();
            CreateEntries();
            ReadInfo();
            CreateButtons();

        }

        private void CreateEntries()
        {
            Label TokenText = new Label
            {
                Text = "Ключ Доступа Группы",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            tokenEntry = new Entry { Text = "", Placeholder = "Пример : 47a232******************a29f" };

            Label GroupIdText = new Label
            {
                Text = "ID Группы",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            groupIdEntry = new Entry { Text = "", Placeholder = "Пример : 214228913" };

            layout = new StackLayout { Padding = new Thickness(10, 25), Children = { TokenText, tokenEntry, GroupIdText, groupIdEntry } };


            this.Content = layout;

        }

        private void CreateButtons()
        {
            saveButton = new Button { Text = "Сохранить", Margin = new Thickness(0, 60) };

            saveButton.Clicked += SaveButton_Clicked;

            resetButton = new Button { Text = "Сбросить информацию", Margin = new Thickness(0, 60) };

            resetButton.Clicked += ResetButton_Clicked;

            resetButton.IsVisible = false;

            if (groupInfoFound)
            {
                resetButton.IsVisible = true;
                saveButton.IsVisible = false;
            }

            layout.Children.Add(saveButton);
            layout.Children.Add(resetButton);
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            tokenEntry.IsReadOnly = false;
            groupIdEntry.IsReadOnly = false;

            resetButton.IsVisible = false;
            saveButton.IsVisible = true;

            tokenEntry.Text = "";
            groupIdEntry.Text = "";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if(tokenEntry.Text == "" || groupIdEntry.Text == "")
            {
                DisplayAlert("Что-то пошло не так", "Заполните все поля, чтобы сохранить информацию", "Ок");
                return;
            }

            SaveInfo(); 
        }

        private void SaveInfo()
        {
            string fileName = "GroupInfo.json";
            var groupInfo = new App.GroupInfo(tokenEntry.Text, groupIdEntry.Text);
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, groupInfo);
            };

            DisplayAlert("Информация была добавлена", "Перейдите на основную страницу приложения и нажмите кнопку *Обновить информацию* для того, чтобы изменения вступили в силу", "Понимаю");
        }

        public void ReadInfo()
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
                        tokenEntry.IsReadOnly = true;
                        groupIdEntry.Text = groupInfo.GroupID;
                        groupIdEntry.IsReadOnly = true;

                        groupInfoFound = true;
                        file.Dispose();
                    }
                    catch
                    {
                       DisplayAlert("Первый запуск?", "Введите информацию в поля. Ключ можно создать в настройках группы, id группы - набор цифр в адресе группы", "Понимаю..");
                    }
                }

               
            } 
        }
    }
}