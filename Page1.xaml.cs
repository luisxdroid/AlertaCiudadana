using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Microsoft.Phone.Maps.Services;
using System.IO.IsolatedStorage;
using System.IO;

namespace AlertaCiudadana
{
    public partial class Page1 : PhoneApplicationPage
    {
        string mypos;
        public int datos = 0;
        public float x1 = 0, x2 = 0;
        public string auxMensajeCalle = "", auxMensajeAltura = "", auxMensajeComuna = "", auxMensajeCiudad = "", comunaPC = "", numeroPC = "";
       
        public Page1()
        {
            InitializeComponent();
            ShowMyLocationOnTheMap();
            GetCurrentCoordinate();
            busquedaComisarias();
            busquedaPlanCuadrante();
        }

        private GeoCoordinate MyCoordinate = null;
        private ReverseGeocodeQuery MyReverseGeocodeQuery = null;
        private double _accuracy = 0.0;
        public GeoCoordinate reCenterPoss = null;

        //mi posicion en el mapa
        private async void ShowMyLocationOnTheMap()
        {
            
            try
            {
                
                Geolocator myGeolocator = new Geolocator();
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);

                //mypos obtiene mi posicion
                mypos = Convert.ToString(myGeocoordinate);
                reCenterPoss = myGeoCoordinate;
                
                //Centrar el mapa en mi posición
                this.myMapa.Center = myGeoCoordinate;
                this.myMapa.ZoomLevel = 15;

#region
                // Creando un mardito circulo
                //Ellipse myCircle = new Ellipse();
                //myCircle.Fill = new SolidColorBrush(Colors.Red);
                //myCircle.Height = 40;
                //myCircle.Width = 40;
                //myCircle.Opacity = 50;
                //myCircle.Tap += myCircle_Tap;
#endregion
                
                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myPoss();
                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                myLocationOverlay.GeoCoordinate = myGeoCoordinate;

         
                //Create a MapLayer to contain the MapOverlay.       
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);    
         
                //Add the MapLayer to the Map.        
                myMapa.Layers.Add(myLocationLayer);
            
            }       
            catch(Exception ex) 
            {           
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        public Image myPoss()
        {
            Image myMapPoss = new Image();
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri("/Imagenes/Boton/myPoss2.png", UriKind.Relative);
            myMapPoss.Source = bi;
            myMapPoss.Tap += myCircle_Tap;
            return myMapPoss;
        }

        void busquedaPlanCuadrante()
        {
            XDocument objXML = XDocument.Load("datosCuadrante.xml");

            Cuadrante[] resultado = (
                from obj in objXML.Descendants("plan")
                where obj.Element("Tipo").Value == "cuadrante" // && (float)obj.Element("Cory") >= y1 && (float)obj.Element("Cory") <= y2 && (float)obj.Element("Corx") >= x1 && (float)obj.Element("Corx") <= x2
                select new Cuadrante
                {
                    //Id = obj.Element("Id").Value,
                    //Tipo = obj.Element("Tipo").Value,
                    Fono = obj.Element("Fono").Value,
                    Comuna = obj.Element("Comuna").Value,

                    
                }).ToArray();

            #region
            //    comunaPC = resultado[1].Comuna;
            //numeroPC = resultado[1].Fono;

            //foreach(Cuadrante auxC in resultado)
            //{
            //    Image imgCuadra = new Image();

            //    imgCuadra.Tag = auxC.Comuna + "|" + auxC.Fono;
            //    comunaPC = auxC.Comuna;
            //    numeroPC = auxC.Fono;
            //}
            #endregion
        }

        void busquedaComisarias()
        {
            XDocument objXML = XDocument.Load("datosComisaria.xml");

            Comisaria[] resultado = (
                from obj in objXML.Descendants("comisaria")
                where obj.Element("Tipo").Value == "carabinero" // && (float)obj.Element("Cory") >= y1 && (float)obj.Element("Cory") <= y2 && (float)obj.Element("Corx") >= x1 && (float)obj.Element("Corx") <= x2
                select new Comisaria
                    {
                        Id = obj.Element("Id").Value, 
                        Tipo = obj.Element("Tipo").Value,
                        Fono = obj.Element("Fono").Value,
                        Nombre = obj.Element("Nombre").Value,
                        corX = obj.Element("corX").Value,
                        corY = obj.Element("corY").Value,
                        Direccion = obj.Element("Direccion").Value
                    }).ToArray();
            #region
            //Comuna = obj.Element("Comuna").Value,
            //fonoCua = obj.Element("fonoCuadrante").Value,
            //Hacia = obj.Element("Hacia").Value,
            #endregion
            
            //Se recore el arreglo 1 x 1 y se agregan todos al mapa
            foreach (Comisaria aux in resultado) //en aux estaran los datos de cada elemento
            {
                Image imgPacos = new Image();
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri("/Assets/1.png", UriKind.Relative);
                imgPacos.Source = bi;
                imgPacos.Tag = aux.Nombre + "|" + aux.Fono + "|" + aux.Direccion; //tag es una propiedas como un auxiliar, en el puedes colocar cualquier valor
                imgPacos.Tap += callPacos_Tap;
                //comunaPC = aux.Comuna;
                //numeroPC = aux.fonoCua;

                MapOverlay overlay = new MapOverlay
                {
                    //CultureInfo.InvariantCulture , se coloca porque en algunos telefonos las coordenadas con con ',' o con '.'
                    GeoCoordinate = new GeoCoordinate(Convert.ToDouble(aux.corX, CultureInfo.InvariantCulture), Convert.ToDouble(aux.corY, CultureInfo.InvariantCulture)),
                    Content = imgPacos
                };

                MapLayer layer = new MapLayer();
                layer.Add(overlay);
                myMapa.Layers.Add(layer);
            }
        }

        void callPacos_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image imgAux = (Image)sender; //le pasas los datos de quien esta ejecutanto el evento
            string[] datos = Convert.ToString(imgAux.Tag).Split('|');

            MessageBox.Show("Direccion: "+ datos[2]);

            PhoneCallTask phoneCallTask = new PhoneCallTask();
            phoneCallTask.DisplayName = datos[0]; 
            phoneCallTask.PhoneNumber = datos[1];
            phoneCallTask.Show();
            
        }
        
        void myCircle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (auxMensajeCalle != "")
            {
                IsolatedStorageSettings contacto = IsolatedStorageSettings.ApplicationSettings;

                SmsComposeTask smsComposeTask = new SmsComposeTask();
                smsComposeTask.Body = "¡ALERTA!\nMe encuentro en una situación de peligro en " + auxMensajeCalle + " altura del #" + auxMensajeAltura + ", comuna de " + auxMensajeComuna + ".";
                smsComposeTask.To = contacto["numTelefonos"].ToString();
                smsComposeTask.Show();
            } 
        }

        private async void GetCurrentCoordinate()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {
                Geoposition currentPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10));
                _accuracy = currentPosition.Coordinate.Accuracy;

                MyCoordinate = new GeoCoordinate(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);

                if (MyReverseGeocodeQuery == null || !MyReverseGeocodeQuery.IsBusy)
                {
                    MyReverseGeocodeQuery = new ReverseGeocodeQuery();
                    MyReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(MyCoordinate.Latitude, MyCoordinate.Longitude);
                    MyReverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
                    MyReverseGeocodeQuery.QueryAsync();
                }
            }
            catch{ }
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MapAddress address = e.Result[0].Information.Address;
                    if(address.District=="")
                    {
                         ubicacion.Text = ""+address.Street+" #"+address.HouseNumber+", \n"+ address.City+", "+address.City;
                         auxMensajeCalle = address.Street;
                         auxMensajeAltura = address.HouseNumber;
                         auxMensajeComuna = address.City;
                    }
                    else
                    {
                        ubicacion.Text = "" + address.Street + " #" + address.HouseNumber + ", \n" + address.District + ", " + address.City;
                        auxMensajeCalle = address.Street;
                        auxMensajeAltura = address.HouseNumber;
                        auxMensajeComuna = address.District;
                        //auxMensajeCiudad = address.City;
                    }

                   
                    #region
                    /*"Street: "+ address.Street + 
                    "\n House Number: "+address.HouseNumber +
                    "\n District: " +address.District +
                    "\n BuildingFloor:  " + address.BuildingFloor +
                    "\n BuildingName: " + address.BuildingName +
                    "\n BuildingRoom: " + address.BuildingRoom +
                    "\n BuildingZone: " + address.BuildingZone +
                    "\n City: " + address.City +
                    "\n Continent: " + address.Continent +
                    "\n Country: " + address.Country +
                    "\n CountryCode: " + address.CountryCode +
                    "\n County: " + address.County +
                    "\n District: " + address.District +
                    "\n houseNumber: " + address.HouseNumber +
                    "\n Neighborhood: " + address.Neighborhood +
                    "\n PostalCode: " + address.PostalCode +
                    "\n Province: " + address.Province +
                    "\n State: " + address.State +
                    "\n StateCode: " + address.StateCode +
                    "\n Street: " + address.Street +
                    "\n TownShip: " + address.Township;
                    "\n  " +address. +*/
                    #endregion
                    //if (address.District==null)
                    //{
                    //    auxMensajeComuna = address.City;
                    //    return;
                    //}
                    
                    //crearDoc();
                }
            }
        }

        #region
        //public void crearDoc()
        //{
        //    XDocument xDoc = XDocument.Load("file.xml");
        //        var contactsElement = new XElement("root", 
        //                                new XElement("prueba",
        //                                    new XElement("caso1", "hola"),
        //                                    new XElement("caso2", "1234"),
        //                                    new XElement("caso3", "abc@abc.com")
                                      
        //                                    ));

        //        xDoc.Add(contactsElement);


        //        var fileStream = File.OpenWrite("file.xml");
                
        //        xDoc.Save(fileStream);
        //}
        #endregion

        //app button
        private void btnLlamar_Click(object sender, EventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();
            phoneCallTask.PhoneNumber = "133";
            phoneCallTask.DisplayName = "Central de Carabineros de Chile";
            phoneCallTask.Show();
        }

        private void btnPlanCuadrante_Click(object sender, EventArgs e)
        {
            XDocument objXML = XDocument.Load("datosCuadrante.xml");

            Cuadrante[] resultado = (
                from obj in objXML.Descendants("plan")
                where obj.Element("Tipo").Value == "cuadrante" // && (float)obj.Element("Cory") >= y1 && (float)obj.Element("Cory") <= y2 && (float)obj.Element("Corx") >= x1 && (float)obj.Element("Corx") <= x2
                select new Cuadrante
                {
                    //Id = obj.Element("Id").Value,
                    //Tipo = obj.Element("Tipo").Value,
                    Fono = obj.Element("Fono").Value,
                    Comuna = obj.Element("Comuna").Value,

                }).ToArray();

            for (int dato = 0; dato < resultado.ToArray().Length; dato++)
            {
                //MessageBox.Show("" + auxMensajeCiudad);
                comunaPC = resultado[dato].Comuna;
                numeroPC = resultado[dato].Fono;

                //PhoneCallTask phoneCallTask = new PhoneCallTask();
                if (auxMensajeComuna == comunaPC)
                {
                    PhoneCallTask phoneCallTask = new PhoneCallTask();
                    phoneCallTask.PhoneNumber = numeroPC;
                    phoneCallTask.DisplayName = "P. Cuadrante "+ auxMensajeComuna;
                    phoneCallTask.Show();
                }
                else
                {
                    //if(auxMensajeCiudad == comunaPC)
                    //{
                    //    MessageBox.Show("Comuna: " + comunaPC + ", " + numeroPC);
                    //}
                    
                }
                //{
                //    comunaPC = auxMensajeCiudad;
                //    numeroPC = resultado[dato].Fono;
                //    auxMensajeComuna = comunaPC;
                //    PhoneCallTask phoneCallTask = new PhoneCallTask();
                //    phoneCallTask.PhoneNumber = numeroPC;
                //    phoneCallTask.DisplayName = "Plan cuadrante de " + auxMensajeComuna;
                //    phoneCallTask.Show();
                //}

                    //if (auxMensajeComuna == "Santiago")
                    //{
                    //    auxMensajeComuna = "Santiago";
                    //    //PhoneCallTask phoneCallTask = new PhoneCallTask();
                    //    phoneCallTask.PhoneNumber = numeroPC;
                    //    phoneCallTask.DisplayName = "Plan cuadrante de " + auxMensajeComuna;
                    //    phoneCallTask.Show();

                    //}
                    //#endregion
                }
            }
        

        private void btnReCalcular_Click(object sender, EventArgs e)
        {
            ubicacion.Text = "Recalculando...\n esperando nueva ubicación";
            GetCurrentCoordinate();
            myMapa.Center = reCenterPoss;
            myMapa.ZoomLevel = 15;
        }

        #region
        //private void bGuardar_Click(object sender, EventArgs e)
        //{
        //    XmlWriter w = XmlWriter.Create("C:holamundo.xml");
        //    //w.WriteStartElement("MiInfo");
        //    //w.WriteElementString("TextBox", textBox1.Text);
        //    //w.WriteElementString("CheckBox", checkBox1.Checked.ToString());
        //    //w.WriteEndElement();
        //    //w.Close();
        //}
        #endregion

        private void btnAyuda_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Simbolos.xaml", UriKind.Relative));
        }
       
    }
}