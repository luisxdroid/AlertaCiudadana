using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AlertaCiudadana.Resources;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;

namespace AlertaCiudadana
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        //void llamadaEmergencia()
        //{
        //    PhoneCallTask phoneCallTask = new PhoneCallTask();
        //    phoneCallTask.PhoneNumber = "+56965695941";
        //    phoneCallTask.DisplayName = "Carlos Vallejos";
        //    phoneCallTask.Show();
        //}

        private void btnCarabineros_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resus = MessageBox.Show("\"PuntoDeAcceso\" deseas acceder a tu posicion actual . \n\n Permitir??", "Advertencia", MessageBoxButton.OKCancel); 
            if (resus == MessageBoxResult.OK) { NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative)); }

        }

        private void btnTerminosUso_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Legal.xaml", UriKind.Relative));
        }
       

        void appBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Contactos.xaml", UriKind.Relative));
        }

        private void btnHotLine_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings contacto = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                SmsComposeTask smsComposeTask = new SmsComposeTask();
                if (contacto["numTelefonos"].ToString().Trim() != null || contacto["numTelefonos"].ToString().Trim() != "")
                {
                    smsComposeTask.Body = "¡ALERTA!\nMe encuentro en una situación de peligro.";
                    smsComposeTask.To = contacto["numTelefonos"].ToString().Trim();
                    smsComposeTask.Show();
                }
                else { MessageBox.Show("¡ALERTA!\nNo existen contactos agregados!!"); }
            }
            catch { MessageBox.Show("¡ALERTA!\nNo existen contactos agregados!!"); }
        }

        
    }
}