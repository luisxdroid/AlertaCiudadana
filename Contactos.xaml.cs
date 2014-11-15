using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Microsoft.Phone.UserData;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;

namespace AlertaCiudadana
{
    public partial class Contactos : PhoneApplicationPage
    {
        public Contactos()
        {
            InitializeComponent();

            Loaded += Contactos_Loaded;
        }

        IsolatedStorageSettings contacto = IsolatedStorageSettings.ApplicationSettings; //base de datos local
        Contacts contactos = new Contacts();

        void Contactos_Loaded(object sender, RoutedEventArgs e)
        {
            contactos.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(Contacts_SearchCompleted);
            contactos.SearchAsync(String.Empty, FilterKind.None, null);
        }

        void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            var ContactsData =(from m in e.Results
                               where (m.PhoneNumbers.FirstOrDefault() != null) 
                               select new MyContacts
                               {
                                   Name = m.DisplayName,
                                   Phone = m.PhoneNumbers.FirstOrDefault().ToString(),
                               }).ToArray();


            List<MyContacts> final = new List<MyContacts>();
            try { contacto.Add("numTelefonos", ""); }
            catch { }

            foreach (MyContacts aux in ContactsData)
            {
                string[] mov = aux.Phone.Split('(');
                string telefono = mov[0].Replace(" ", "");
                string telefono2 = telefono + ";";

                if (contacto["numTelefonos"].ToString().Contains(telefono2))
                {
                    final.Add(new MyContacts { Name = aux.Name, Phone = telefono, Check = true });
                }
                else
                {
                    final.Add(new MyContacts { Name = aux.Name, Phone = telefono, Check = false });
                }
            }


            lbContacts.ItemsSource = final;



        }

        private void CheckBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CheckBox aux = (CheckBox)sender;
           
            
            
            if (aux.IsChecked == true)
            {
                contacto["numTelefonos"] = contacto["numTelefonos"].ToString() + aux.Tag.ToString() + "; "; 
            }

            if (aux.IsChecked == false)
            {
                string fil = aux.Tag.ToString()+"; ";
                string[] filtro = new string[] { fil };
                var delete = contacto["numTelefonos"].ToString().Split(filtro, StringSplitOptions.None);

                contacto["numTelefonos"] = delete[0] + delete[1];
            }

        }


    }

    public class MyContacts
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool Check { get; set; }
    }
}