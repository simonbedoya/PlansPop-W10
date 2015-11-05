using PlansPop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AgregarPlan : Page
    {
        private string nombre, descripcion, fecha, hora;
        private StorageFile photo;




        public AgregarPlan()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

        }

        private async void onClickSiguiente(object sender, RoutedEventArgs e)
        {
            this.nombre = nombrePlan.Text;
            this.descripcion = descripcionPlan.Text;
            this.fecha = fechaPlan.Date.Day + "/" + fechaPlan.Date.Month + "/" + fechaPlan.Date.Year;
            this.hora = configurarHora(horaPlan.Time.Hours, horaPlan.Time.Minutes);

            if (nombre.Equals("") && descripcion.Equals(""))
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Por favor llene los campos");
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                var result = await dialog.ShowAsync();
            }
            else
            {
                if (photo == null)
                {
                    var packageLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    var assetsFolder = await packageLocation.GetFolderAsync("Assets");
                    photo = await assetsFolder.GetFileAsync("fotoplan.jpg");

                }

                Plan plan = new Plan()
                {
                    NombrePlan = nombre,
                    DescripcionPlan = descripcion,
                    FechaPlan = fecha,
                    HoraPlan = hora,
                    ImagenPlan = photo
                };
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(AddMapa), plan);
            }
        }

        private string configurarHora(int hourOfDay, int minute)
        {
            string am_pm;
            string horaC;

            if (hourOfDay < 12)
            {
                am_pm = "AM";
                if (hourOfDay == 0) { hourOfDay = 12; }
            }
            else
            {
                am_pm = "PM";
                if (hourOfDay != 12) { hourOfDay = hourOfDay - 12; }
            }
            if (hourOfDay < 10)
            {
                if (minute < 10) { horaC = "0" + hourOfDay + ":" + "0" + minute + " " + am_pm; }
                else { horaC = "0" + hourOfDay + ":" + minute + " " + am_pm; }
            }
            else { horaC = hourOfDay + ":" + minute + " " + am_pm; }
            return horaC;
        }

        private async void onClickTomarFoto(object sender, RoutedEventArgs e)
        {
            photo = null;
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                return;
            }
            else
            {
                mostrarFoto(photo);

            }

        }

        private async void onClickElegirFoto(object sender, RoutedEventArgs e)
        {
            photo = null;
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            photo = await picker.PickSingleFileAsync();
            if (photo != null)
            {
                mostrarFoto(photo);
            }
            else
            {

            }
        }

        private async void mostrarFoto(StorageFile file)
        {
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            Image ima = new Image();
            ima.Source = bitmapSource;

            imageControl.Source = bitmapSource;
        }

    }
}
