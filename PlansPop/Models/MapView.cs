using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Parse;
using Windows.Storage.Streams;

namespace PlansPop.Models

{

    public class MapView : Grid
    {
        private MapControl mapControl;

        public MapView()
        {

            mapControl = new MapControl();
            mapControl.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            mapControl.TiltInteractionMode = MapInteractionMode.GestureAndControl;
            mapControl.MapServiceToken = "ift37edNFjWtwMkOrquL~7gzuj1f0EWpVbWWsBaVKtA~Amh-DIg5R0ZPETQo7V-k2m785NG8XugPBOh52EElcvxaioPYSYWXf96wL6E_0W1g";

            // Specify a known location
            BasicGeoposition cityPosition;
            Geopoint cityCenter;

            cityPosition = new BasicGeoposition() { Latitude = 2.4448143, Longitude = -76.6147395 };
            cityCenter = new Geopoint(cityPosition);

            // Set map location
            mapControl.Center = cityCenter;
            mapControl.ZoomLevel = 13.0f;
            mapControl.LandmarksVisible = true;

            AddIcons();

            this.Children.Add(mapControl);


            //this.Children.Add(_map);
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
                mapIcon.Title = lugar.Get<string>("nombre") + " " + lugar.Get<string>("direccion");
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bus.jpg"));
                mapIcon.ZIndex = 0;

                //Añadiendo el Icono
                mapControl.MapElements.Add(mapIcon);

            }

        }


    }
}
