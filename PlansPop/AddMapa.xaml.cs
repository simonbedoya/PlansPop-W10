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
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddMapa : Page
    {
        private Frame rootFrame;
        private Plan plan;
        private string direccion;

        public AddMapa()
        {
            this.InitializeComponent();

            rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                    = AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().BackRequested += AddMapa_BackRequested;

            mapControl.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            mapControl.TiltInteractionMode = MapInteractionMode.GestureAndControl;
            mapControl.MapServiceToken = "ift37edNFjWtwMkOrquL~7gzuj1f0EWpVbWWsBaVKtA~Amh-DIg5R0ZPETQo7V-k2m785NG8XugPBOh52EElcvxaioPYSYWXf96wL6E_0W1g";

            // Specify a known location

            BasicGeoposition posicion = new BasicGeoposition() { Latitude = 2.4448143, Longitude = -76.6147395 };
            Geopoint point = new Geopoint(posicion);

            // Set map location
            mapControl.Center = point;
            mapControl.ZoomLevel = 13.0f;
            mapControl.LandmarksVisible = true;

            AddIcons();

            //GridMapa.Children.Add(mapControl);

        }

        private void AddMapa_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void AddIcons()
        {

            //Query

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

                //Añadiendo el Icono
                mapControl.MapElements.Add(mapIcon);

            }

        }

        private async void MapTapped(MapControl sender, MapInputEventArgs args)
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

                mapControl.MapElements.Add(mapIcon);
            }

        }

        private async void ElementClick(MapControl sender, MapElementClickEventArgs args)
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
                progressRing.IsActive = true;

                StorageFile file = plan.ImagenPlan;
                var bytes = await GetBtyeFromFile(file);
                ParseFile parseFile = new ParseFile(plan.NombrePlan + ".jpg", bytes, "image/jpeg");

                ParseObject parseObject = new ParseObject("Plan");
                parseObject.Add("nombre", plan.NombrePlan);
                parseObject.Add("descripcion", plan.DescripcionPlan);
                parseObject.Add("fecha", plan.FechaPlan + " " + plan.HoraPlan);
                parseObject.Add("id_user", ParseUser.CurrentUser);
                parseObject.Add("imagen", parseFile);

                ParseGeoPoint geoLugar = new ParseGeoPoint(ArgsLat, ArgsLon);
                parseObject.Add("lugar", geoLugar);

                if (direccion == null)
                {
                    parseObject.Add("direccion", dir);

                }
                else
                {
                    parseObject.Add("direccion", direccion);

                    ParseObject objectLugar = new ParseObject("Lugares");
                    objectLugar.Add("nombre", direccion);
                    objectLugar.Add("direccion", direccion);
                    objectLugar.Add("ubicacion", geoLugar);
                    await objectLugar.SaveAsync();
                }

                try
                {
                    await parseObject.SaveAsync();

                    progressRing.IsActive = false;
                    rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage));

                }
                catch
                {
                    var dialog2 = new Windows.UI.Popups.MessageDialog("Error al crear el plan");
                    dialog2.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                    var result2 = await dialog.ShowAsync();
                }


            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            plan = e.Parameter as Plan;

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