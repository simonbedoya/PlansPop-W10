using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
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
    public sealed partial class Mapa : Page
    {
        Frame rootFrame;
        private int indicador = -1;
        BasicGeoposition startLocation;
        BasicGeoposition endLocation;

        MapRouteView viewOfRoute;

        public Mapa()
        {
            this.InitializeComponent();
            rootFrame = Window.Current.Content as Frame;

            map.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            map.TiltInteractionMode = MapInteractionMode.GestureAndControl;
            map.MapServiceToken = "ift37edNFjWtwMkOrquL~7gzuj1f0EWpVbWWsBaVKtA~Amh-DIg5R0ZPETQo7V-k2m785NG8XugPBOh52EElcvxaioPYSYWXf96wL6E_0W1g";

            // Specify a known location

            BasicGeoposition posicion = new BasicGeoposition() { Latitude = 2.4448143, Longitude = -76.6147395 };
            Geopoint point = new Geopoint(posicion);

            // Set map location
            map.Center = point;
            map.ZoomLevel = 13.0f;
            map.LandmarksVisible = true;

            AddIcons();

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

            // Get my current location.
            getPosicionActual();

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
                map.MapElements.Add(mapIcon);

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
                    map.MapElements.Add(posicionActual);

                    break;

                case GeolocationAccessStatus.Denied:

                    break;

                case GeolocationAccessStatus.Unspecified:

                    break;
            }

        }


        private async void elementClick(MapControl sender, MapElementClickEventArgs args)
        {
            if (indicador == -1)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Indicaciones desde aqui");
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });
                var result = await dialog.ShowAsync();

                if (result.Id.Equals(1))
                {
                    startLocation = new BasicGeoposition() { Latitude = args.Location.Position.Latitude, Longitude = args.Location.Position.Longitude };
                    indicador = 1;
                }
            }
            else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Indicaciones hasta aqui");
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Aceptar") { Id = 1 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 0 });
                var result = await dialog.ShowAsync();

                if (result.Id.Equals(1))
                {
                    endLocation = new BasicGeoposition() { Latitude = args.Location.Position.Latitude, Longitude = args.Location.Position.Longitude };
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(new Geopoint(startLocation),
                                                        new Geopoint(endLocation),
                                                        MapRouteOptimization.Time,
                                                        MapRouteRestrictions.None);
                    if (routeResult.Status == MapRouteFinderStatus.Success)
                    {

                        viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Colors.Yellow;
                        viewOfRoute.OutlineColor = Colors.Black;

                        map.Routes.Add(viewOfRoute);

                        await map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, MapAnimationKind.None);
                    }

                }
                indicador = -1;

            }

        }

        private void map_MapHolding(MapControl sender, MapInputEventArgs args)
        {
            if (viewOfRoute != null)
            {
                map.Routes.Remove(viewOfRoute);
            }
        }
    }
}
