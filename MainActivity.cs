
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PhotoLibrary
{
    [Activity(Label = "PhotoLibrary")]
    public class MainActivity : Activity
    {
        int count = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //Start of the photo library application onCreate

            this.StartActivity(typeof(PhotoLibraryActivity));
            SetContentView(Resource.Layout.Main);

            //Get the button from the mainUI and attach it to the event
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click+=delegate {

                button.Text = $"{count++} clicks !";
	};

        }
    }
}
