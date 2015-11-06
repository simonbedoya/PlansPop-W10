using Parse;
using PlansPop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        

        public MainPage()
        {      
                  
            this.InitializeComponent();
           
            MainFrame.Navigate(typeof(Planes));
        }

        private ObservableCollection<MenuItem> menuList;

        public ObservableCollection<MenuItem> MenuList
        {
            get
            {
                if (menuList == null)
                {
                    menuList = new ObservableCollection<MenuItem>();

                    MenuItem item = new MenuItem() { Name = "Planes", Icon = "Home" };
                    MenuItem item1 = new MenuItem() { Name = "Mapa", Icon = "Map" };
                    MenuItem item2 = new MenuItem() { Name = "Mis Planes", Icon = "Favorite" };
                    MenuItem item3 = new MenuItem() { Name = "Agregar Plan", Icon = "Add" };
                   

                    menuList.Add(item);
                    menuList.Add(item1);
                    menuList.Add(item2);
                    menuList.Add(item3);
                    
                    


                }
                CargarDatos();
                return menuList;
                
            }
            set { menuList = value; }
        }

       /* private void logout(object sender, RoutedEventArgs e)
        {
            ParseUser.LogOut();
            var currentUser = ParseUser.CurrentUser; // this will now be null
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }*/
        private void click(object sender, RoutedEventArgs e)
        {
             if (split.IsPaneOpen)
             {
                 split.IsPaneOpen = false;
                 profile.Width = 0;
                 profile.Height = 0;
             }
             else
             {
                 split.IsPaneOpen = true;
                profile.Width = 280.0;
                profile.Height = 100.0;
            }
            
            //MainFrame.Navigate(typeof(LoginPage));

            
        }

        private void selection(object sender, SelectionChangedEventArgs e)
        {
            
            int selection = menu.SelectedIndex;

            switch (selection) {

                case 0:
                    MainFrame.Navigate(typeof(Planes));
                    break;
                case 1:
                    MainFrame.Navigate(typeof(Mapa));
                    break;
                case 2:
                    MainFrame.Navigate(typeof(MisPlanes));
                    break;
                case 3:
                    MainFrame.Navigate(typeof(AgregarPlan));
                    break;
            }
           

            

        }
        

        private void logout(object sender, RoutedEventArgs e)
        {
            ParseUser.LogOut();
            var currentUser = ParseUser.CurrentUser; // this will now be null
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }
        
        public void CargarDatos()
        {
            
                var user = ParseUser.CurrentUser;
                name.Text = user.Get<String>("name");
                email.Text = user.Email;
                ParseFile file = user.Get<ParseFile>("photo");
                Uri ur = file.Url;
                BitmapImage img = new BitmapImage(ur);
                image.ImageSource = img;
        }

        private void btn_profile(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(ProfilePage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string msg = e.Parameter as string;
            if (msg == null)
            {
                MainFrame.Navigate(typeof(Planes));
            }
            else {
                MainFrame.Navigate(typeof(ProfilePage));
            }

        }
    }
}
