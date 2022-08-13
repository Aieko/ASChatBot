using ASChatBot;
using DependencyServiceDemos.Droid;
using Android.OS;
using Android.App;
using Android.Content;
using ASChatBot.Droid;
using Android.Support.V4.App;
using Android.Graphics;

[assembly: Xamarin.Forms.Dependency(typeof(StartServiceAndroid))]
namespace DependencyServiceDemos.Droid
{
 

    [Service]
    public class MyLocationService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            string channelId = null;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                channelId = CreateNotificationChannel("kim.hsl", "ForegroundService");
            }
            else
            {
                channelId = "";
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channelId)
             .SetOngoing(true)
             .SetContentTitle("Sample Notification")
             .SetContentText("Hello World! This is my first notification!")
             .SetSmallIcon(Resource.Drawable.as_notification_icon);
            // Build the notification:
            Notification notification = builder.Build();

            StartForeground(1, notification);

            return StartCommandResult.NotSticky;
        }

        private string CreateNotificationChannel(string channelId, string channelName)
        {
            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
            {
                Description = "Hello! I'm a notification :)"
            };
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            return channelId;
        }
    }
}