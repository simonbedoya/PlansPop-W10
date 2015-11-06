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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VerPlan : Page
    {
        Frame rootFrame;
        PlanItem planitem;
        ParseObject plan;

        public VerPlan()
        {
            this.InitializeComponent();
            rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += VerPlan_BackRequested;

            VerMap.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            VerMap.TiltInteractionMode = MapInteractionMode.GestureAndControl;
            VerMap.MapServiceToken = "ift37edNFjWtwMkOrquL~7gzuj1f0EWpVbWWsBaVKtA~Amh-DIg5R0ZPETQo7V-k2m785NG8XugPBOh52EElcvxaioPYSYWXf96wL6E_0W1g";


        }

        private void VerPlan_BackRequested(object sender, BackRequestedEventArgs e)
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

            VerNombre.Text = plan.Get<string>("nombre");
            VerFecha.Text = plan.Get<string>("fecha");
            VerDireccion.Text = plan.Get<string>("direccion");
            VerDescripcion.Text = plan.Get<string>("descripcion");

            Uri ur = plan.Get<ParseFile>("imagen").Url;

            BitmapImage img = new BitmapImage(ur);
            VerImage.Source = img;

            //Para lo de asistentes
            mostrarCreadorAsistentes();

            //Configurar Mapa
            AddMap();


        }

        private void AddMap()
        {
            ParseGeoPoint lugar = plan.Get<ParseGeoPoint>("lugar");
            BasicGeoposition posicion = new BasicGeoposition() { Latitude = lugar.Latitude, Longitude = lugar.Longitude };
            Geopoint point = new Geopoint(posicion);

            // Set map location
            VerMap.Center = point;
            VerMap.ZoomLevel = 17.0f;
            VerMap.LandmarksVisible = true;

            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = point;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = plan.Get<string>("direccion");
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/posicionPlan.png"));
            mapIcon.ZIndex = 0;
            getPosicionActual();
            VerMap.MapElements.Add(mapIcon);

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
                    VerMap.MapElements.Add(posicionActual);

                    break;

                case GeolocationAccessStatus.Denied:

                    break;

                case GeolocationAccessStatus.Unspecified:

                    break;
            }

        }

        private async void mostrarCreadorAsistentes()
        {
            string Id_creador = plan.Get<ParseObject>("id_user").ObjectId;

            var creador = await ParseUser.Query.GetAsync(Id_creador);
            VerCreador.Text = creador.Get<string>("name");

            ParseRelation<ParseUser> relation = plan.GetRelation<ParseUser>("Asistentes");
            IEnumerable<ParseUser> resul = await relation.Query.FindAsync();

            int numero = resul.Count<ParseUser>();
            VerAsistentes.Text = numero.ToString();

            if (numero > 0)
            {
                ParseUser asistente = ParseUser.CurrentUser;
                ParseUser user;
                for (int i = 0; i < numero; i++)
                {
                    user = resul.ElementAt<ParseUser>(i);
                    if (!asistente.ObjectId.Equals(user.ObjectId))
                    { // no es asistente
                        
                        btnAsistir.Background = new SolidColorBrush(Colors.White);

                    }
                    else
                    {// es asistente
                        
                        btnAsistir.Background = new SolidColorBrush(Colors.Cyan);

                    }
                }
            }
            else
            {
                
                btnAsistir.Background = new SolidColorBrush(Colors.White);
            }



        }

        private async void onClickAsistir(object sender, RoutedEventArgs e)
        {
            ParseUser asistente = ParseUser.CurrentUser;

            var addRelation = plan.GetRelation<ParseUser>("Asistentes");
            IEnumerable<ParseUser> resul = await addRelation.Query.FindAsync();

            ParseUser user;
            int count = resul.Count<ParseUser>();
            int numero = 0;
            int noAsistente = -1;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {

                    user = resul.ElementAt<ParseUser>(i);

                    if (asistente.ObjectId.Equals(user.ObjectId)) // es asistente
                    { // Borrarlo de asistentes
                        numero = count - 1;
                        noAsistente = 1;
                        VerAsistentes.Text = numero.ToString();
                        
                        btnAsistir.Background = new SolidColorBrush(Colors.White);
                        addRelation.Remove(asistente);
                        await plan.SaveAsync();
                    }

                }
                if (noAsistente == -1)
                {
                    numero = count + 1;
                    VerAsistentes.Text = numero.ToString();
                    //asistiendo
                    btnAsistir.Background = new SolidColorBrush(Colors.Cyan);
                    addRelation.Add(asistente);
                    await plan.SaveAsync();
                }
                else
                {
                    noAsistente = -1;
                }
            }
            else
            {
                numero = 1;
                VerAsistentes.Text = numero.ToString();
                //assitiendo
                btnAsistir.Background = new SolidColorBrush(Colors.Cyan);
                addRelation.Add(asistente);
                await plan.SaveAsync();
            }

        }
    }
}
