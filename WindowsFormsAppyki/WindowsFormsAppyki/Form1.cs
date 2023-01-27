using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NPOI.SS.Formula;

namespace WindowsFormsAppyki
{
    public partial class YKİ : MaterialForm
    {
        public readonly MaterialSkinManager materialSkinManager;

        public List<string> ListView { get; set; }
        public object Optimization { get; private set; }

        public YKİ()
        {
            MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();


            // our target sysid
            byte sysid;
            // our target compid
            byte compid;


            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

        }

        public void btnbaglan_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
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
            TxtDurum.Text = "Bağlantı Kesildi ";

        }

        public void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // TxtDurum.Text = "KONUM VERİLERİ YÜKLENİYOR";
            List<ListViewItem> veriler = new List<ListViewItem>(); // veriler ana listesi

            SerialPort gonderen = (SerialPort)sender;
            string veri = gonderen.ReadExisting();
            List<string> enl = StringFuncs.String.StringBetween(veri, "en"); // enl listesi oluşturuldu.(ham data)
            PointLatLng point = new PointLatLng(-1, 1);

            if (enl != null)
            {
                ListViewItem item = new ListViewItem("ENLEM"); // item listiew ı oluştu. ( işlenmiş dataların bulunacağı liste)
                try
                {
                    string lat = enl.LastOrDefault();
                    byte dd = Convert.ToByte(lat.Substring(0, 2));  // Gelen veri kalıbınının ilk kısmı(dd)
                    double mm_mmmm = Convert.ToDouble(lat.Substring(2)); // Gelen veri kalıbının geri kalan kısmı
                    double a = dd + (mm_mmmm / 60);
                    point.Lat = a;
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
                    byte ddd = Convert.ToByte(lon.Substring(0, 3));  // Gelen veri kalıbınının ilk kısmı(ddd)
                    double mm_mmmm = Convert.ToDouble(lon.Substring(3)); // Gelen veri kalıbının geri kalan kısmı
                    double b = ddd + (mm_mmmm / 60);
                    point.Lng = b;
                    item.SubItems.Add(b.ToString("G"));
                    textBox2.Text = Convert.ToDouble(b).ToString();
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
                veriler.Add(item);

            }
            listView1.Items.AddRange(veriler.ToArray());
            // MAP
            if (point.Lat != -1 && point.Lng != -1)
            {
                map.MapProvider = GMapProviders.GoogleMap;
                map.MinZoom = 5; // minimum zoom
                map.MaxZoom = 150; // maksimum zoom
                map.Zoom = 10; // default zoom 

                map.Position = new PointLatLng(point.Lat, point.Lng);

                PointLatLng nokta = new PointLatLng(point.Lat, point.Lng);
                GMapMarker marker = new GMarkerGoogle(nokta, GMarkerGoogleType.red_dot); // işaretçinin niteliğini belirleyen satır.

                GMapOverlay markers = new GMapOverlay("markers");
                markers.Markers.Add(marker);
                map.Overlays.Add(markers);

                if (map.Overlays.Count > 1)  // diğer işaretçileri harita üzerinden kaldırmak için bir koşul oluşturdum.
                {
                    map.Overlays.RemoveAt(0);

                }
            }

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
            //TxtDurum.Text = "DURUM BİLGİSİ ALINIYOR.";

            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            listView1.View = View.Details;

            // Harita

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            map.DragButton = MouseButtons.Left;
            map.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;
            map.MaxZoom = 200;

            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var dot = map.FromLocalToLatLng(e.X, e.Y);
                double lat = dot.Lat;
                double lon = dot.Lng;
                GMapMarker marker = new GMarkerGoogle(dot, GMarkerGoogleType.red_dot);
                GMapOverlay markers = new GMapOverlay("markers");
                markers.Markers.Add(marker);
                map.Overlays.Add(markers);

            }

        }
        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            SerialVeriGonder("<iptal/>");
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
            //İNİŞ
            MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();
            bool armed = false;

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;

            req.param1 = armed ? 0 : 1;
            armed = !armed;

            MAVLink.mavlink_mission_item_int_t req2 = new MAVLink.mavlink_mission_item_int_t();
            req2.command = (byte)MAVLink.MAV_CMD.WAYPOINT;

            //req2.y =(dot.Lat);
            //req2.x = dot.Lng;
            //req2.z = (float)(2.34);
            req2.param1 = 0;
            req2.param2 = 0;
            req2.param3 = 0;
            req2.param4 = 0;
            req2.seq = 0;
            byte[] packet = mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);
            serialPort1.Write(packet, 0, packet.Length);
        }
        private List<XElement> KoordinatlarıGetir()
        {
            List<XElement> waypoints = new List<XElement>();
            waypoints.Add(new XElement("one", 1, 40.23005160, 29.00995460));
            waypoints.Add(new XElement("two", 2, 40.23010640, 29.00984470));
            waypoints.Add(new XElement("three", 40.23017500, 29.00980710));
            waypoints.Add(new XElement("four", 40.23025380, 29.00983330));
            waypoints.Add(new XElement("five", 40.23030450, 29.00989290));
            waypoints.Add(new XElement("six", 6, 40.23032450, 29.01001570));
            waypoints.Add(new XElement("sev", 7, 40.23025950, 29.01013300));
            waypoints.Add(new XElement("ei", 8, 40.23017910, 29.01016590));
            waypoints.Add(new XElement("ni", 9, 40.23009720, 29.01012230));
            waypoints.Add(new XElement("ten", 10, 40.23005160, 29.01000690));
            return waypoints;
        }
        private XDocument NoktalariGetir()// XML DOSYASI OLUŞTURAN KOD
        {
            XDocument doc = XDocument.Parse("<?xml version = \"1.0\" encoding=\"UTF-8\" ?>\n<noktalar></noktalar>");
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

        private void BtnHaritaYenile_Click(object sender, EventArgs e)
        {

            if (map.Overlays.Count > 1)  // diğer işaretçileri harita üzerinden kaldırmak için bir koşul oluşturdum.
            {
                map.Overlays.RemoveAt(0);
                map.Refresh();

            }
        }

      
    }
}
