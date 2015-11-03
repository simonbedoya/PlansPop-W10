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
            try
            {
                userr = user.Text;
                passs = pass.Text;

                if (userr.Equals("") || passs.Equals(""))
                {
                    var dialog2 = new Windows.UI.Popups.MessageDialog("Por favor llene los campos usuario y contraseña");
                    dialog2.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                    var result2 = await dialog2.ShowAsync();
                    vacio = 1;
                }
                else {
                    vacio = 0;
                }

                await ParseUser.LogInAsync(userr, passs);

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
                
                // Login was successful.
            }
            catch (Exception ex)
            {
                if (vacio == 0)
                {

                    string error = ex.Message;
                    if (ex.Message.Equals("invalid login parameters"))
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog("Usuario o contraseña incorrectos", "Informacion");
                        dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                        var result = await dialog.ShowAsync();
                    }
                    else
                    {


                        var dialog = new Windows.UI.Popups.MessageDialog("La conexion se esta tardando demasiado, verifique la conexion a internet e intente de nuevo.", "Alerta");
                        dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                        var result = await dialog.ShowAsync();
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
    }
}
