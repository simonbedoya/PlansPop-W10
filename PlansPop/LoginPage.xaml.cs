using Parse;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;
using System.Windows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
   
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        string userr;
        string passs;
        int vacio = 0;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            PrgRing.Visibility = Visibility.Visible;
            try
            {
                
                userr = user.Text;
                passs = pass.Password;
                
                if (userr.Equals("") || passs.Equals(""))
                {
                    error.Text = "Por favor ingrese usuario y contraseña";
                    error.Visibility = Visibility.Visible;
                    var margin = login.Margin;
                    margin.Top = 4;
                    login.Margin = margin;
                    PrgRing.Visibility = Visibility.Collapsed;
                    login.Visibility = Visibility.Visible;
                    
                    vacio = 1;
                    
                }
                else {
                    vacio = 0;
                }
                
                await ParseUser.LogInAsync(userr, passs);

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
                PrgRing.Visibility = Visibility.Collapsed;
                login.Visibility = Visibility.Visible;

                // Login was successful.
            }
            catch (Exception ex)
            {
                if (vacio == 0)
                {

                    string errors = ex.Message;
                    if (ex.Message.Equals("invalid login parameters"))
                    {
                        error.Text = "Usuario o contraseña incorrectos.";
                        error.Visibility = Visibility.Visible;
                        var margin = login.Margin;
                        margin.Top = 4;
                        login.Margin = margin;
                        PrgRing.Visibility = Visibility.Collapsed;
                        login.Visibility = Visibility.Visible;
                        
                    }
                    else
                    {
                        error.Text = "La conexion se esta tardando demasiado, verifique la conexion a internet e intente de nuevo.";
                        error.Visibility = Visibility.Visible;
                        var margin = login.Margin;
                        margin.Top = 4;
                        login.Margin = margin;
                        PrgRing.Visibility = Visibility.Collapsed;
                        login.Visibility = Visibility.Visible;
                        
                    }
                    
                }
                /* var dialog = new Windows.UI.Popups.MessageDialog("Hola");
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                var result = await dialog.ShowAsync();
                if (result.Id.Equals("0") ){

                }
                else
                {
                }
                */

            }
        }

        private void RemPass(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(RemPassPage));
        }

        private void register(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(RegisterPage));
        }

        private void text_change(object sender, TextChangedEventArgs e)
        {
            error.Visibility = Visibility.Collapsed;
            var margin = login.Margin;
            margin.Top = 20;
            login.Margin = margin;
        }

        private void pass_change(object sender, RoutedEventArgs e)
        {
            error.Visibility = Visibility.Collapsed;
            var margin = login.Margin;
            margin.Top = 20;
            login.Margin = margin;
        }
    }
}
