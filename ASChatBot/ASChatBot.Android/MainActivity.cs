using Xamarin.Essentials;
using Android.App;
using Android.Content.PM;
using System.Threading.Tasks;
using Manifest = Android.Manifest;
using Android.OS;
using Xamarin.Forms;
using Button = Android.Widget.Button;
using ToggleButton = Android.Widget.ToggleButton;
using Android.Views;
using Android.Content;

namespace ASChatBot.Droid
{
    [Activity(Label = "Anima Shoes", Icon = "@drawable/logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Views.View.IOnClickListener
    {
        public static MainActivity Instance { get; private set; }
       
        public ToggleButton vkBotToggleButton { get; private set; }
        public ToggleButton emailBotToggleButton { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            Instance = this;


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);


            SetContentView(Resource.Layout.MainActivity_Layout);
            Button vkBotInfoButton = (Button)FindViewById(Resource.Id.VkBotInfoButton);
            Button emailBotInfoButton = (Button)FindViewById(Resource.Id.EmailBotInfoButton);
            Button refreshButton = (Button)FindViewById(Resource.Id.RefreshButton);

            vkBotToggleButton = (ToggleButton)FindViewById(Resource.Id.VkBotToggleButton);
            emailBotToggleButton = (ToggleButton)FindViewById(Resource.Id.EmailBotToggleButton);

            vkBotInfoButton.SetOnClickListener(this);
            emailBotInfoButton.SetOnClickListener(this);
            vkBotToggleButton.SetOnClickListener(this);
            emailBotToggleButton.SetOnClickListener(this);
            refreshButton.SetOnClickListener(this);

            DependencyService.Get<IStartService>().StartForegroundServiceCompat();

            if (AnimaShoesService.Instance != null)
            {
                VkBotToggleButtonSwitch(AnimaShoesService.Instance.VkBotOn);
                MailBotToggleButtonSwitch(AnimaShoesService.Instance.MailBotOn);
            }

            RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.Internet }, 0);

        }

        public async override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            
            await CheckAndRequestReadFilesPermission();

            await CheckAndRequestWriteFilesPermission();

            await CheckAndRequestInternetFilesPermission();

            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task<PermissionStatus> CheckAndRequestReadFilesPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Prompt the user to turn on in settings
                status = await Permissions.RequestAsync<Permissions.StorageRead>();

                return status;
            }

            return status;

        }

        public async Task<PermissionStatus> CheckAndRequestWriteFilesPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Prompt the user to turn on in settings
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                return status;
            }

            return status;

        }

        public async Task<PermissionStatus> CheckAndRequestInternetFilesPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Prompt the user to turn on in settings
                status = await Permissions.RequestAsync<Permissions.NetworkState>();

                return status;
            }

            return status;

        }

        public void OnClick(Android.Views.View v)
        {
            Intent intent = new Intent(this, typeof(AnimaShoesService));
            string action = "";
            switch (v.Id)
            {
                case Resource.Id.RefreshButton:
                    action = "ru.animashoesapp.service.action.refreshinfo";
                    intent.SetAction(action);
                    StartForegroundService(intent);
                    break;
                case Resource.Id.VkBotInfoButton:
                    intent = new Intent(this, typeof(VkBotInfoActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.EmailBotInfoButton:
                    intent = new Intent(this, typeof(EmailBotInfoActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.VkBotToggleButton:
                    action = (!(v as ToggleButton).Checked) ? 
                        "ru.animashoesapp.service.action.stopvkbot" : 
                        "ru.animashoesapp.service.action.runvkbot";
                    intent.SetAction(action);
                    StartForegroundService(intent);
                    break;
                case Resource.Id.EmailBotToggleButton:
                    action = (!(v as ToggleButton).Checked) ? 
                        "ru.animashoesapp.service.action.stopmailbot" :
                        "ru.animashoesapp.service.action.runmailbot";
                    intent.SetAction(action);
                    StartForegroundService(intent);
                    break;
            }  
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (AnimaShoesService.Instance != null)
            {
                vkBotToggleButton.Checked = AnimaShoesService.Instance.VkBotOn;
                emailBotToggleButton.Checked = AnimaShoesService.Instance.MailBotOn;
            }

        }

        public void VkBotToggleButtonSwitch(bool powerOn)
        {
            if(powerOn)
            {
                vkBotToggleButton.Checked = powerOn;
                vkBotToggleButton.SetTextColor(Android.Graphics.Color.Green);
            }
            else
            {
                vkBotToggleButton.Checked = powerOn;
                vkBotToggleButton.SetTextColor(Android.Graphics.Color.DarkRed);
            }
        }
        
        public void MailBotToggleButtonSwitch(bool powerOn)
        {
            if (powerOn)
            {
                emailBotToggleButton.Checked = powerOn;
                emailBotToggleButton.SetTextColor(Android.Graphics.Color.Green);
            }
            else
            {
                emailBotToggleButton.Checked = powerOn;
                emailBotToggleButton.SetTextColor(Android.Graphics.Color.DarkRed);
            }
        }
    }
    
}