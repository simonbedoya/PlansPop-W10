using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class ProfilePage : Page
    {
        ParseUser user = ParseUser.CurrentUser;
        private StorageFile photo;

        public ProfilePage()
        {
            this.InitializeComponent();
            CargarDatos();
        }
        public void CargarDatos() {
            
            
            ParseFile file = user.Get<ParseFile>("photo");
            Uri ur = file.Url;
            BitmapImage img = new BitmapImage(ur);
            imageControl.Source = img;
            name.Text = user.Get<string>("name");
            mail.Text = user.Email;
            int item = 0;
            switch (user.Get<string>("sex")) {
                case "Hombre":
                    item = 1;
                    break;
                case "Mujer":
                    item = 0;
                    break;
            }
            sex.SelectedIndex = item;
            char[] delimeter = { '/', ' ', ':' };
            string[] fechaCom = user.Get<string>("b_date").Split(delimeter);

            string fec = fechaCom[1] + "/" + fechaCom[0] + "/" + fechaCom[2];
            birth_date.Date = DateTime.Parse(fec);
            txt_user.Text = user.Username;
        }

        private void edit(object sender, RoutedEventArgs e)
        {
            name.IsEnabled = true;
            sex.IsEnabled = true;
            birth_date.IsEnabled = true;
            panel_edit.Visibility = Visibility.Collapsed;
            panel_save.Visibility = Visibility.Visible;
        }

        private async void save(object sender, RoutedEventArgs e)
        {
            user["name"] = name.Text;
            int s = sex.SelectedIndex;
            string sexo = "";
            switch (s)
            {
                case 1:
                    sexo = "Hombre";
                    break;
                case 0:
                    sexo = "Mujer";
                    break;
            }
            user["sex"] = sexo;
            string fecha = birth_date.Date.Day + "/" + birth_date.Date.Month + "/" + birth_date.Date.Year;
            user["b_date"] = fecha;
            try
            {
                PrgRing.Visibility = Visibility.Visible;
                await user.SaveAsync();
                PrgRing.Visibility = Visibility.Collapsed;
                accept.Visibility = Visibility.Visible;
                panel_save.Visibility = Visibility.Collapsed;
                panel_edit.Visibility = Visibility.Visible;
                await Task.Delay(1500);
                accept.Visibility = Visibility.Collapsed;
                name.IsEnabled = false;
                sex.IsEnabled = false;
                birth_date.IsEnabled = false;
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage),"1");
                

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                PrgRing.Visibility = Visibility.Collapsed;
                error.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                error.Visibility = Visibility.Collapsed;
            }
            
        }

        private async void btn_tomarfoto(object sender, RoutedEventArgs e)
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
                save_photo(photo);

            }

        }

        private async void btn_elegirfoto(object sender, RoutedEventArgs e)
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
                save_photo(photo);
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
        public async void save_photo(StorageFile file)
        {
            var bytes = await GetBtyeFromFile(photo);
            ParseFile parseFile = new ParseFile(user.Username + ".jpg", bytes, "image/jpeg");
            user["photo"] = parseFile;
            try
            {
                PrgRing2.Visibility = Visibility.Visible;
                await user.SaveAsync();
                PrgRing2.Visibility = Visibility.Collapsed;
                accept_photo.Visibility = Visibility.Visible;
                error_photo.Text = "Actualizado.";                
                error_photo.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                error_photo.Visibility = Visibility.Collapsed;
                accept_photo.Visibility = Visibility.Collapsed;
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage), "1");

            }
            catch (Exception e)
            {
                string msg2 = e.Message;
                PrgRing2.Visibility = Visibility.Collapsed;
                error_photo.Text = "No se actualizo.";
                error_photo.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                error_photo.Visibility = Visibility.Collapsed;
            }
            
        }
        private async Task<byte[]> GetBtyeFromFile(StorageFile storageFile)
        {
            var stream = await storageFile.OpenReadAsync();

            using (var dataReader = new DataReader(stream))
            {
                var bytes = new byte[stream.Size];
                await dataReader.LoadAsync((uint)stream.Size);
                dataReader.ReadBytes(bytes);

                return bytes;
            }
        }
    }
}
