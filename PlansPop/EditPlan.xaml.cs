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
        string titulo_lugar;

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


        }

        private async void AddIcons(ParseGeoPoint lugarPlanEdit)
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Lugares");
            IEnumerable<ParseObject> results = await query.FindAsync();
            ParseObject lugar;
            ParseGeoPoint ubicacion;
            BasicGeoposition position;
            Geopoint point;
            MapIcon mapIcon;

            //posicion actual
            getPosicionActual();

            // Agregarlos al mapa
            int tamanio = results.Count<ParseObject>();


            for (int i = 0; i < tamanio; i++)
            {

                lugar = results.ElementAt<ParseObject>(i);
                ubicacion = lugar.Get<ParseGeoPoint>("ubicacion");

                position = new BasicGeoposition() { Latitude = ubicacion.Latitude, Longitude = ubicacion.Longitude };
                point = new Geopoint(position);

                // MapIcon
                mapIcon = new MapIcon();
                mapIcon.Location = point;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = lugar.Get<string>("direccion");

                if (ubicacion.Latitude == lugarPlanEdit.Latitude && ubicacion.Longitude == lugarPlanEdit.Longitude)
                {
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/posicionPlan.png"));
                }
                else
                {
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bus.png"));
                }
                mapIcon.ZIndex = 0;

                editMap.MapElements.Add(mapIcon);

            }

        }

        private async void getPosicionActual()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator myGeolocator = new Geolocator();
                    Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                    Geocoordinate myGeocoordinate = myGeoposition.Coordinate;

                    MapIcon posicionActual = new MapIcon();
                    posicionActual.Location = myGeocoordinate.Point;
                    posicionActual.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    posicionActual.Title = "MiPosicion";
                    posicionActual.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/posicion_actual.png"));
                    posicionActual.ZIndex = 0;
                    editMap.MapElements.Add(posicionActual);

                    break;

                case GeolocationAccessStatus.Denied:

                    break;

                case GeolocationAccessStatus.Unspecified:

                    break;
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

            gPoint = plan.Get<ParseGeoPoint>("lugar");

            // Specify a known location

            BasicGeoposition posicion = new BasicGeoposition() { Latitude = gPoint.Latitude, Longitude = gPoint.Longitude };
            Geopoint point = new Geopoint(posicion);

            // Set map location
            editMap.Center = point;
            editMap.ZoomLevel = 17.0f;
            editMap.LandmarksVisible = true;


            AddIcons(gPoint);

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
                var res = await contentDialog.ShowAsync();

                edtProgressRing.IsActive = true;
                BasicGeoposition pos = new BasicGeoposition() { Latitude = args.Location.Position.Latitude, Longitude = args.Location.Position.Longitude }; ;
                Geopoint point = new Geopoint(pos);

                MapLocationFinderResult LocationAdress = await MapLocationFinder.FindLocationsAtAsync(point);
                direccion = LocationAdress.Locations[0].Address.Street + "-" + LocationAdress.Locations[0].Address.StreetNumber + ", "
                               + LocationAdress.Locations[0].Address.Country + ", " + LocationAdress.Locations[0].Address.Town;


                MapIcon mapIcon = new MapIcon();
                mapIcon.Location = point;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = titulo_lugar + " - " + direccion;
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bus.png"));
                mapIcon.ZIndex = 0;

                editMap.MapElements.Add(mapIcon);
                edtProgressRing.IsActive = false;
            }
        }

        private async void editElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            string dir = null;
            double ArgsLat = 0;
            double ArgsLon = 0;

            if (btnEditSiguiente.Visibility == Visibility.Visible)
            {

                foreach (var e in args.MapElements)
                {
                    var icon = e as MapIcon;
                    dir = icon.Title;
                    ArgsLat = icon.Location.Position.Latitude;
                    ArgsLon = icon.Location.Position.Longitude;

                }

                if (!dir.Equals("MiPosicion"))
                {

                    if (gPoint.Latitude == ArgsLat && gPoint.Longitude == ArgsLon) // si el lugar seleccionado es el lugar del plan
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog("Este es el lugar");

                        dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                        //dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });

                        var result = await dialog.ShowAsync();
                    }
                    else
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog("Elegir Lugar");

                        dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                        dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });

                        var result = await dialog.ShowAsync();

                        if (result.Id.Equals(1))
                        {
                            gPoint.Latitude = ArgsLat;
                            gPoint.Longitude = ArgsLon;
                            NombreLugarTxt.Text = dir;
                            if (direccion == null)
                            {
                                nuevo_existente = 1; // lugar existente

                            }
                            else
                            {
                                nuevo_existente = 2; // nuevo lugar

                                direccion = null;
                            }

                        }
                    }
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
            plan["lugar"] = gPoint;
            if (photo != null)
            {
                var bytes = await GetBtyeFromFile(photo);
                ParseFile parseFile = new ParseFile("editdefault" + editHora.Time.Minutes + ".jpg", bytes, "image/jpeg");
                plan["imagen"] = parseFile;

            }
            if (nuevo_existente == 2) // crear nuevo lugar
            {
                ParseObject objectLugar = new ParseObject("Lugares");
                objectLugar.Add("nombre", NombreLugarTxt.Text);
                objectLugar.Add("direccion", NombreLugarTxt.Text);
                objectLugar.Add("ubicacion", gPoint);
                await objectLugar.SaveAsync();

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

        private void contentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            titulo_lugar = tituloLugar.Text;
        }

        private void edit_plan_btn(object sender, RoutedEventArgs e)
        {
            btn_en_edt.Visibility = Visibility.Collapsed;
            btnEditSiguiente.Visibility = Visibility.Visible;
            editNombre.IsEnabled = true;
            editDescripcion.IsEnabled = true;
            editFecha.IsEnabled = true;
            editHora.IsEnabled = true;
        }
    }
}

