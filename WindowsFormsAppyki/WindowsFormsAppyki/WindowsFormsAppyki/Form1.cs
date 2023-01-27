using AForge.Video;
using AForge.Video.DirectShow;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Spatial;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WindowsFormsAppyki
{
    public partial class form1 : MaterialForm
    {
        public readonly MaterialSkinManager materialSkinManager;

        public List<string> ListView { get; set; }
        public object Optimization { get; private set; }

        private FilterInfoCollection webcam; // bilgisayara bağlı kameraların dizisi

        private VideoCaptureDevice cam;



        public form1()
        {
            InitializeComponent();

            materialSkinManager = MaterialSkinManager.Instance;
           
           
            
        }

        public void btnbaglan_Click(object sender, EventArgs e)
        {
            string portismi = comboBox1.Text.ToString();

            string BantGenisligi = comboBox2.Text.ToString();


            serialPort1.PortName = portismi;
            serialPort1.BaudRate = Convert.ToInt32(BantGenisligi);
            serialPort1.Open();
            TxtDurum.Text = "AKB BAĞLANTISI KURULDU ";


        }

        public void btnbaglantıkes_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            MessageBox.Show("Bağlantı kesildi.");


        }

        public void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            List<ListViewItem> veriler = new List<ListViewItem>(); 

            SerialPort gonderen = (SerialPort)sender;
            string veri = gonderen.ReadExisting();
            List<string> enl = StringFuncs.String.StringBetween(veri, "en");
            PointLatLng nokta = new PointLatLng(-1, 1);
            TxtDurum.Text = "KONUM VERİLERİ YÜKLENİYOR";
            if (enl != null)
            {
                ListViewItem item = new ListViewItem("Enlem"); //LitsView' a eklendi.
                try
                {
                    string lat = enl.LastOrDefault();
                    double dd = Convert.ToDouble(lat.Substring(0, 2));  // Gelen veri kalıbınının ilk kısmı(dd)
                    double mm_mmmm = Convert.ToDouble(lat.Substring(2)); // Gelen veri kalıbının geri kalan kısmı
                    double a = dd + (mm_mmmm / 60);
                    nokta.Lat = a;
                    item.SubItems.Add(a.ToString("G"));
                    textBox1.Text = Convert.ToDouble(a).ToString();
                }
                catch
                {
                    item.SubItems.Add("Uydu Bağlantısı Yetersiz");
                }
                finally
                {

                    veriler.Add(item);
                }

            }

            List<string> byl = StringFuncs.String.StringBetween(veri, "boy");
            if (byl != null)
            {
                ListViewItem item = new ListViewItem("BOYLAM");
                try
                {
                    string lon = byl.LastOrDefault();
                    double ddd = Convert.ToDouble(lon.Substring(0, 3));  // Gelen veri kalıbınının ilk kısmı(ddd)
                    double mm_mmmm = Convert.ToDouble(lon.Substring(3)); // Gelen veri kalıbının geri kalan kısmı
                    double b = ddd + (mm_mmmm / 60);
                    nokta.Lng = b;
                    item.SubItems.Add(b.ToString("G"));
                    textBox1.Text = Convert.ToDouble(b).ToString();
                }
                catch
                {
                    item.SubItems.Add("Uydu Bağlantısı Yetersiz");
                }
                finally
                {
                    veriler.Add(item);
                }
            }

            List<string> Irtifa = StringFuncs.String.StringBetween(veri, "irt");
            if (Irtifa != null)
            {
                ListViewItem item = new ListViewItem("İRTİFA");
            }
            if (nokta.Lat != -1 && nokta.Lng != -1)
            {
                map.MapProvider = GMapProviders.GoogleMap;
                double lat = Convert.ToDouble(textBox1.Text);
                double lng = Convert.ToDouble(textBox2.Text);
                map.Position = new PointLatLng(lat, lng);
                map.MinZoom = 5; // minimum zoom
                map.MaxZoom = 100; // maksimum zoom
                map.Zoom = 10; // default zoom

                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot); // işaretçinin niteliğini belirleyen satır.


                GMapOverlay markers = new GMapOverlay("markers");
                markers.Markers.Add(marker);
                map.Overlays.Add(markers);
            }



            KayıtEkle(string.Format("Veri Alındı :{0}", veri));


        }

        private void KayıtEkle(string veri)
        {
            //Datetime dt = Datetime.Now;
            //if (index == -1) ;
            // {
            // kayıtlar
            //}
        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
        }

        public void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            // yenisi gelecek.
        }


        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {

            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? MaterialSkinManager.Themes.LIGHT : MaterialSkinManager.Themes.DARK;

        }

        public void form1_Load(object sender, EventArgs e)
        {   // mod 
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo600, Primary.Indigo900, Primary.Indigo100, Accent.Green100, TextShade.WHITE);
            comboBox1.Text = "Seçiniz";
            TxtDurum.Text = "DURUM BİLGİSİ ALINIYOR.";

            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
         



            // Harita
            map.ShowCenter = false;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            map.DragButton = MouseButtons.Left;
            map.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleSatelliteMap;

            // Kamera
            webcam = new

            FilterInfoCollection(FilterCategory.VideoInputDevice); //webcam dizisine mevcut kameraları dolduruyoruz.

            foreach (FilterInfo item in webcam)
            {


                comboBox3.Items.Add(item.Name); // kameralar combobox a eklendi.

            }

            comboBox3.SelectedIndex = 0;

          

        }


        private void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            cam = new

            VideoCaptureDevice(webcam[comboBox3.SelectedIndex].MonikerString); //başlaya basıldığıdnda yukarda tanımladığımız cam değişkenine comboboxta seçilmş olan kamerayı atıyoruz.

            cam.NewFrame += new NewFrameEventHandler(cam_NewFrame);

            cam.Start(); //kamerayı başlatıyoruz.


        }

        void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)

        {

            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone(); //kısaca bu eventta kameradan alınan görüntüyü picturebox a atıyoruz.

            pcbVideo.Image = bmp;

        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {

            if (cam.IsRunning)
            { 
            
                cam.Stop();

            }

        }

        private void materialRaisedButton5_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog swf = new SaveFileDialog();

            swf.Filter = "(*.jpg)|*.jpg|Bitma*p(*.bmp)|*.bmp";

            DialogResult dialog = swf.ShowDialog();  // ekran görüntüsü aldı.


            if (dialog == DialogResult.OK)
            {

                pcbVideo.Image.Save(swf.FileName);
            }
            
        }

       

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            SerialVeriGonder("<durdur/>");
        }

        private void SerialVeriGonder(string veri)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Bağlantı Açık Değil!");
                return;
            }

            serialPort1.Write(Encoding.UTF8.GetBytes(veri), 0, veri.Length);

        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            SerialVeriGonder("<inis/>");
        }
        private List<XElement> KoordinatlarıGetir() 
        {
            List<XElement> waypoints = new List<XElement>();
            waypoints.Add(new XElement("one",1, 40.23005160, 29.00995460));
            waypoints.Add(new XElement("two" ,2, 40.23010640, 29.00984470));
            waypoints.Add(new XElement("three", 40.23017500, 29.00980710));
            waypoints.Add(new XElement("four", 40.23025380, 29.00983330));
            waypoints.Add(new XElement("five", 40.23030450, 29.00989290));
            waypoints.Add(new XElement("six" ,6, 40.23032450, 29.01001570));
            waypoints.Add(new XElement("seven" ,7, 40.23025950, 29.01013300));
            waypoints.Add(new XElement("eight" ,8, 40.23017910, 29.01016590));
            waypoints.Add(new XElement("nine", 9, 40.23009720, 29.01012230));
            waypoints.Add(new XElement("ten",10, 40.23005160, 29.01000690));
            return waypoints;
        }

      

        private XDocument NoktalariGetir() // XML DOSYASI OLUŞTURAN KOD
        {
            XDocument doc = XDocument.Parse("<noktalar></noktalar>");
            XElement rootElem = doc.Root;
            List<XElement> elementler = KoordinatlarıGetir();

            rootElem.Add(elementler);
            return doc;
        }
        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {

            SerialVeriGonder("<basla/>"); // Göreve başla butonu

         
            SerialVeriGonder(NoktalariGetir().ToString());


        }

        
    }
}
