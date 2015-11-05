using Parse;
using PlansPop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class EditPlan : Page
    {
        Frame rootFrame;
        PlanItem planitem;

        StorageFile photo;

        ParseObject parseObj;


        private string imgURL = "http://files.parsetfss.com/b5b17f71-5029-472e-9a5a-080cf10284b3/tfss-90911fe4-2118-41ee-83ca-04def59d0512-piscina.jpg";
        public string ImgURL
        {
            get { return imgURL; }
            set { imgURL = value; }
        }

        public EditPlan()
        {
            this.InitializeComponent();

            rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += EditPlan_BackRequested;

            editMap.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            editMap.TiltInteractionMode = MapInteractionMode.GestureAndControl;
            editMap.MapServiceToken = "ift37edNFjWtwMkOrquL~7gzuj1f0EWpVbWWsBaVKtA~Amh-DIg5R0ZPETQo7V-k2m785NG8XugPBOh52EElcvxaioPYSYWXf96wL6E_0W1g";

            // Specify a known location

            BasicGeoposition posicion = new BasicGeoposition() { Latitude = 2.4448143, Longitude = -76.6147395 };
            Geopoint point = new Geopoint(posicion);

            // Set map location
            editMap.Center = point;
            editMap.ZoomLevel = 13.0f;
            editMap.LandmarksVisible = true;

        }

        private async void AddIcons()
        {

            ParseQuery<ParseObject> query = ParseObject.GetQuery("Lugares");
            IEnumerable<ParseObject> results = await query.FindAsync();
            ParseObject lugar;
            ParseGeoPoint ubicacion;
            BasicGeoposition position;
            Geopoint point;
            MapIcon mapIcon;

            // Agregarlos al mapa
            int tamanio = results.Count<ParseObject>();


            for (int i = 0; i < tamanio; i++)
            {

                lugar = results.ElementAt<ParseObject>(i);
                ubicacion = lugar.Get<ParseGeoPoint>("ubicacion");
                //Icon
                position = new BasicGeoposition() { Latitude = ubicacion.Latitude, Longitude = ubicacion.Longitude };
                point = new Geopoint(position);
                // MapIcon
                mapIcon = new MapIcon();
                mapIcon.Location = point;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = lugar.Get<string>("direccion");
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bus.png"));
                mapIcon.ZIndex = 0;

                editMap.MapElements.Add(mapIcon);

            }

        }

        private void EditPlan_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            planitem = e.Parameter as PlanItem;

            ParseObject plan = planitem.obj;

            editNombre.Text = plan.Get<string>("nombre");
            editDescripcion.Text = plan.Get<string>("descripcion");
            char[] delimeter = { '/', ' ', ':' };
            string[] fechaCom = plan.Get<string>("fecha").Split(delimeter);

            string fec = fechaCom[1] + "/" + fechaCom[0] + "/" + fechaCom[2];
            editFecha.Date = DateTime.Parse(fec);

            string hor = fechaCom[3];
            string minute = fechaCom[4];
            string am_pm = fechaCom[5];

            string horaOrga = organizarHora(hor, minute, am_pm);

            //editFecha.Date = DateTime.Parse(fechaCom[0]);

            editHora.Time = TimeSpan.Parse(horaOrga);

            ParseFile file = plan.Get<ParseFile>("imagen");
            Uri ur = file.Url;
            BitmapImage img = new BitmapImage(ur);
            editImage.Source = img;

            NombreLugarTxt.Text = plan.Get<string>("direccion");

        }

        private string organizarHora(string hor, string minute, string am_pm)
        {
            int h = Int32.Parse(hor);
            int m = Int32.Parse(minute);

            string horaO;

            if (am_pm.Equals("PM"))
            {
                h = h + 12;

            }
            horaO = h + ":" + m + ":00";

            return horaO;

        }

        private async void onElegirFoto(object sender, RoutedEventArgs e)
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

        private async void onTomarFoto(object sender, RoutedEventArgs e)
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

            editImage.Source = bitmapSource;
        }

        private void editMapTapped(MapControl sender, MapInputEventArgs args)
        {

        }

        private void editElementClick(MapControl sender, MapElementClickEventArgs args)
        {

        }

        private void onClickEditSiguiente(object sender, RoutedEventArgs e)
        {

        }


    }
}

