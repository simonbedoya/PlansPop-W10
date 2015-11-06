using Parse;
using PlansPop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Services.Maps;
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

        StorageFile photo = null;

        ParseGeoPoint gPoint;
        string direccion;

        ParseObject plan;

        int nuevo_existente = -1;

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

            AddIcons();
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

            plan = planitem.obj;

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

        private async void editMapTapped(MapControl sender, MapInputEventArgs args)
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Agregar este lugar");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });
            var result = await dialog.ShowAsync();

            if (result.Id.Equals(1))
            {

                BasicGeoposition pos = new BasicGeoposition() { Latitude = args.Location.Position.Latitude, Longitude = args.Location.Position.Longitude }; ;
                Geopoint point = new Geopoint(pos);

                MapLocationFinderResult LocationAdress = await MapLocationFinder.FindLocationsAtAsync(point);
                direccion = LocationAdress.Locations[0].Address.Street + "--" + LocationAdress.Locations[0].Address.StreetNumber + ", "
                               + LocationAdress.Locations[0].Address.Country + ", " + LocationAdress.Locations[0].Address.Town;


                MapIcon mapIcon = new MapIcon();
                mapIcon.Location = point;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = direccion;
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bus.png"));
                mapIcon.ZIndex = 0;

                editMap.MapElements.Add(mapIcon);
            }
        }

        private async void editElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            string dir = null;
            double ArgsLat = 0;
            double ArgsLon = 0;

            foreach (var e in args.MapElements)
            {
                var icon = e as MapIcon;
                dir = icon.Title;
                ArgsLat = icon.Location.Position.Latitude;
                ArgsLon = icon.Location.Position.Longitude;

            }

            var dialog = new Windows.UI.Popups.MessageDialog("Elegir Lugar");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });

            var result = await dialog.ShowAsync();

            if (result.Id.Equals(1))
            {
                gPoint = new ParseGeoPoint(ArgsLat, ArgsLon);

                if (direccion == null)
                {
                    nuevo_existente = 1; // lugar existente
                    NombreLugarTxt.Text = dir;
                }
                else
                {
                    nuevo_existente = 2; // nuevo lugar
                    NombreLugarTxt.Text = direccion;
                    direccion = null;
                }

            }

        }

        private async void onClickEditSiguiente(object sender, RoutedEventArgs e)
        {
            string organizarFecha = editFecha.Date.Day + "/" + editFecha.Date.Month + "/" + editFecha.Date.Year;
            string organizarHora = configurarHora(editHora.Time.Hours, editHora.Time.Minutes);

            edtProgressRing.IsActive = true;
            plan["nombre"] = editNombre.Text;
            plan["descripcion"] = editDescripcion.Text;

            plan["fecha"] = organizarFecha + " " + organizarHora;
            plan["direccion"] = NombreLugarTxt.Text;
            if (photo != null)
            {
                var bytes = await GetBtyeFromFile(photo);
                ParseFile parseFile = new ParseFile(editNombre.Text + ".jpg", bytes, "image/jpeg");
                plan["imagen"] = parseFile;

            }

            if (!gPoint.Latitude.Equals(0) && !gPoint.Longitude.Equals(0))
            {
                plan["lugar"] = gPoint;
                if (nuevo_existente == 2) // crear nuevo lugar
                {
                    ParseObject objectLugar = new ParseObject("Lugares");
                    objectLugar.Add("nombre", direccion);
                    objectLugar.Add("direccion", direccion);
                    objectLugar.Add("ubicacion", gPoint);
                    await objectLugar.SaveAsync();
                }
            }
            try
            {
                await plan.SaveAsync();
                edtProgressRing.IsActive = false;
                rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            catch
            {
                var dialog2 = new Windows.UI.Popups.MessageDialog("Error al editar el plan");
                dialog2.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                var result2 = await dialog2.ShowAsync();
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