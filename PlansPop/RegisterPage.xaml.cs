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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlansPop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        
        private void back_login(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(LoginPage));
        }

        private async void register(object sender, RoutedEventArgs e)
        {
            string name_c = name.Text;
            string mail = email.Text;
            string day = Fecha_Nacimiento.DayFormat;
            string month = Fecha_Nacimiento.MonthFormat;
            string year = Fecha_Nacimiento.YearFormat;
            string fecha = day + "-" + month + "-" + year;
            int s = sex.SelectedIndex;
            string sexo="";
            switch (s)
            {
                case 0:
                    sexo = "Hombre";
                    break;
                case 1:
                    sexo = "Mujer";
                    break;
            }
            string userr = user.Text;
            string passs = pass.Password;
            string re_passs = re_pass.Password;
            int cont_error = 0;

            if (name_c.Equals("") || userr.Equals("") || passs.Equals("") || re_passs.Equals("") || mail.Equals(""))
            {
                Errors.Text = "Por favor llene todos los campos.";
                Errors.Visibility = Visibility.Visible;
                var margin = panel.Margin;
                margin.Top = -17;
                panel.Margin = margin;
                cont_error = 1;
            }
            if (!passs.Equals(re_passs))
            {
                Errors.Text = "Las contraseñas deben conicidir.";
                Errors.Visibility = Visibility.Visible;
                var margin = panel.Margin;
                margin.Top = -17;
                panel.Margin = margin;
                cont_error = 1;
            }
            if (cont_error == 0)
            {
                try
                {
                    PrgRing.Visibility = Visibility.Visible;
                    panel.Visibility = Visibility.Collapsed;
                    var user = new ParseUser()
                    {
                        Username = userr,
                        Password = passs,
                        Email = mail
                    };

                    // other fields can be set just like with ParseObject
                    user["name"] = name_c;
                    user["sex"] = sexo;
                    user["b_date"] = fecha;

                    await user.SignUpAsync();
                }
                catch (Exception ex)
                {
                    PrgRing.Visibility = Visibility.Collapsed;
                    panel.Visibility = Visibility.Visible;
                    string error = ex.Message;
                    if (error.Contains("username"))
                    {
                        Errors.Text = "El usuario "+userr+" ya se encuentra registrado.";
                        Errors.Visibility = Visibility.Visible;
                        var margin = panel.Margin;
                        margin.Top = -17;
                        panel.Margin = margin;
                        cont_error = 1;
                    }
                    if (error.Contains("email"))
                    {
                        Errors.Text = "El correo electrónico " + mail + " ya se encuentra registrado.";
                        Errors.Visibility = Visibility.Visible;
                        var margin = panel.Margin;
                        margin.Top = -17;
                        panel.Margin = margin;
                        cont_error = 1;
                    }

                }
                
            }
        }

        private void text_change(object sender, TextChangedEventArgs e)
        {
            Errors.Visibility = Visibility.Collapsed;
            var margin = panel.Margin;
            margin.Top = 0;
            panel.Margin = margin;
        }

        private void pass_change(object sender, RoutedEventArgs e)
        {
            Errors.Visibility = Visibility.Collapsed;
            var margin = panel.Margin;
            margin.Top = 0;
            panel.Margin = margin;
        }
    }
    
}
