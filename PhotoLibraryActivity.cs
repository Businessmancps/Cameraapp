
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.CurrentActivity;

namespace PhotoLibrary
{
    [Activity(Label = "PhotoLibraryActivity", MainLauncher =true,Icon ="@mipmap/ic_launcher", Theme="@style/MyCustomTheme")]
    public class PhotoLibraryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //Getting the layout resources from PhotoLibraryUI
            SetContentView(Resource.Layout.PhotoLibraryUI);


            //Getting the chooseGallery Button from the layout and attaching an event to it
            Button useCamera = FindViewById<Button>(Resource.Id.takePicture);
            useCamera.Click += TakePictureButton_Clicked;
            Button chooseFromGallery = FindViewById<Button>(Resource.Id.chooseFromGallery);
            chooseFromGallery.Click +=ChooseFromGalleryButton_Clicked;
        }

        public async void TakePictureButton_Clicked(object sender,System.EventArgs args)
        {
            await CrossMedia.Current.Initialize();
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                ShowMessageDialog("Permission Denied", "Unable to gain access to the camera King.");
                return;
            }
            if(!await Task.Run(()=>CheckCameraAlbumPermissions()))
            {
                ShowMessageDialog("Permission Denied", "Unable to gain access to the camera Daniel.");
                return;
            }

            var imageFilename = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Name = $"{DateTime.UtcNow}.jpg",
                DefaultCamera = CameraDevice.Rear,
                PhotoSize = PhotoSize.Medium,
                SaveToAlbum = true,
            });

            if (imageFilename == null)
                return;

            ImageView photoImageView = FindViewById<ImageView>(Resource.Id.photoImageView);


            photoImageView.SetImageURI(Android.Net.Uri.Parse(imageFilename.Path));
        }

         async void ChooseFromGalleryButton_Clicked(object sender, System.EventArgs args)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                ShowMessageDialog("Not Supported", "Choosing a photo is not supported");
                return;
            }
            if(!await Task.Run(() => CheckCameraAlbumPermissions()))
            {
                ShowMessageDialog("Permision Denied", "Unable to gain access to the Photo Album Daniel.");
                return;
            }

            var imageFilename = await CrossMedia.Current.PickPhotoAsync();
            if (imageFilename != null)
            {
                ImageView photoImageView = FindViewById<ImageView>(Resource.Id.photoImageView);
                photoImageView.SetImageURI(Android.Net.Uri.Parse(imageFilename.Path));
            }
        }
        public async Task<bool> CheckCameraAlbumPermissions()
        {
            var deviceCameraStatus = await
                CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var deviceAlbumStatus = await
                 CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (deviceCameraStatus != PermissionStatus.Granted || 
                deviceAlbumStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] {Permission.Camera , Permission.Storage});
                deviceCameraStatus = results[Permission.Camera];
                deviceAlbumStatus = results[Permission.Storage];
            }

            return (deviceCameraStatus == PermissionStatus.Granted && deviceAlbumStatus == PermissionStatus.Granted);
        }

        public void ShowMessageDialog(string title, string message)
        {
            var dialog = new AlertDialog.Builder(this);
            var alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton("Ok", (c, ev) => CrossPermissions.Current.OpenAppSettings());
            alert.Show();
        }

    }
}
