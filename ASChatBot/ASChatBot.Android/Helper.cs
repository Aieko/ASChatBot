using Android.App;
using Android.Content;
using System.Collections.Generic;
using System;
namespace ASChatBot.Droid
{
    public static class Helper
    {
       

        public static void DisplayAlert(string title, string message, string buttonText = null, Context context = null)
        {
            if (buttonText == null) buttonText = "Ok";

            if (context == null) context = MainActivity.Instance;

            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(context);
            AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton(buttonText, (c, ev) =>
            {
                alert.Cancel();
            });
            alert.Show();
        }

    }

    public static class Extensions
    {
        public static string Filter(this string str, List<char> charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                str = str.Replace(c.ToString(), String.Empty);
            }

            return str;
        }
    }

}