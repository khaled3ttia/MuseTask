/////////////////////////////////////////////////////////////////
// By Khaled Attia
// khaled.3ttia@gmail.com
//
// This sample App demonstrates the use of local notifications
// After it is loaded, a number of notifications (depending on the json file)
// appears on the action bar at the top of your screen
// you can show them and tap them, then details will be passed to the main 
// activity screen
//
////////////////////////////////////////////////////////////////

using Android.App;
using Android.Widget;
using Android.OS;
//to parse JSON
using Newtonsoft.Json;

//to be able to use List<T> 
using System.Collections.Generic;

//to be able to use StreamReader class to loas the JSON file
using System.IO;

//to be able to use Context in the notification manager 
using Android.Content;
namespace MuseTask
{
    [Activity(Label = "MuseTask", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            string title = Intent.GetStringExtra("Title") ?? "No title yet";
            string description = Intent.GetStringExtra("Description") ?? "No description yet";

            //Let the text of the first TextView be equal to the title received
            //from notification
            //at initial state (no notification tapped), it will have the value of
            //"No title yet"
            TextView titleText = FindViewById<TextView>(Resource.Id.nameText);
            titleText.Text = title;

            //Let the text of the second TextView be equal to the description 
            //received from notification
            //at initial state (no notification tapped), it will have the value of
            //"No description yet"
            TextView descText = FindViewById<TextView>(Resource.Id.descText);
            descText.Text = description;


            //load JSON file that contains notifications names and description
            StreamReader sr = new StreamReader(Assets.Open("not.json"));
            string jsonContent = sr.ReadToEnd();

            //Retrieve deserialized Object as "RootObject" that contains List<Notf>
            RootObject notificationData = JsonConvert.DeserializeObject<RootObject>(jsonContent);

            //Initialize a list that contains notification builders (one builder for each notification)
            List<Notification.Builder> builders = new List<Notification.Builder>();

            //Initialize a list that contains notifications
            List<Notification> notifications = new List<Notification>();

            //Create a notification manager
            NotificationManager notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            int notificationid = 0;

            for (int i = 0; i < notificationData.notf.Count; i++)
            {
                string notfi_name = notificationData.notf[i].name;
                string notfi_desc = notificationData.notf[i].description;

                //tapping a notification would bring us back to main activity
                //and insert some additional data (name and description)
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("Title", notfi_name);
                intent.PutExtra("Description", notfi_desc);

                
                int pendingIntentId = i;
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.UpdateCurrent);

                //create a new notification builder and add it to builders list
                builders.Add(new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(notfi_name)
                .SetContentText(notfi_desc)
                .SetSmallIcon(Android.Resource.Drawable.IcNotificationOverlay)
                .SetDefaults(NotificationDefaults.Vibrate));

                //create a new notification and add it to notifications list
                notifications.Add(builders[i].Build());

                //publish the notification using notification manager
                notificationManager.Notify(notificationid, notifications[i]);
                notificationid++;
            }


        }
    }

    //Helper classes to deserialize JSON object
    public class Notf
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class RootObject
    {
        public List<Notf> notf { get; set; }
    }
}

