using Parse;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class RemPassPage : Page
    {
        public RemPassPage()
        {
            this.InitializeComponent();
        }

        private void back_login(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }

        private async void solicitar(object sender, RoutedEventArgs e)
        {
            string email = re_email.Text;
            if (email.Equals(""))
            {
                warning.Visibility = Visibility.Visible;
                var margin = panel_button.Margin;
                margin.Top = -17;
                panel_button.Margin = margin;
                re_email.Text = "";
            }
            else
            {
                warning.Visibility = Visibility.Collapsed;
                var margin = panel_button.Margin;
                margin.Top = 0;
                panel_button.Margin = margin;
                panel_button.Visibility = Visibility.Collapsed;
                PrgRing2.Visibility = Visibility.Visible;
                try
                {
                    await ParseUser.RequestPasswordResetAsync(email);
                    PrgRing2.Visibility = Visibility.Collapsed;
                    Accept.Visibility = Visibility.Visible;
                    await Task.Delay(2000);                  
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(LoginPage));


                }
                catch (Exception ex)
                {

                  string error =  ex.Message;
                    if (error.Contains("found")) {
                        warning.Text = "No existe el correo "+email;
                        var margin2 = panel_button.Margin;
                        margin2.Top = -17;
                        panel_button.Margin = margin2;
                        warning.Visibility = Visibility.Visible;                        
                        PrgRing2.Visibility = Visibility.Collapsed;
                        panel_button.Visibility = Visibility.Visible;
                        
                                           
                    }
                    switch (error) {
                        case "invalid email address":
                            warning.Text = "Ingrese un correo electrónico valido.";
                            var margin3 = panel_button.Margin;
                            margin3.Top = -17;
                            panel_button.Margin = margin3;
                            panel_button.Visibility = Visibility.Visible;
                            PrgRing2.Visibility = Visibility.Collapsed;
                            warning.Visibility = Visibility.Visible;
                            break;
                        }
                }
                
                
            }
        }

        private void change_text(object sender, TextChangedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            var margin = panel_button.Margin;
            margin.Top = 0;
            panel_button.Margin = margin;
        }

       
    }
}
