using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Welcome
{
    public partial class frmBg : Form
    {
        private Dictionary<string, Person> personMap;
        private HttpListener listener;
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void playAnimation(String indexNo)
        {
            // todo: based on team pick the image
            if (personMap.ContainsKey(indexNo))
            {
                string team = personMap[indexNo].team;
                
            }
            
        }

        private void listen(HttpListener listener)
        {
            while (true)
            {
                IAsyncResult result = listener.BeginGetContext(new AsyncCallback(listnerCallback), listener);
                result.AsyncWaitHandle.WaitOne();
            }


        }

        private void listnerCallback(IAsyncResult result)
        {
            HttpListenerContext context = listener.EndGetContext(result);
            var request = context.Request;
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            string input = reader.ReadToEnd();
            Console.WriteLine(input);


            var temp = input.Split('=');
            string indexNo = temp[1];
            updateFrom(indexNo);
            var responseObj = new JObject();
            responseObj.Add("Success", true);
            string str_obj = responseObj.ToString();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str_obj);
            HttpListenerResponse response = context.Response;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private void startHttpListner()
        {
            var httpListner = new HttpListener();
            this.listener = httpListner;
            httpListner.Prefixes.Add("http://192.168.137.1:8080/");
            httpListner.Start();
            Task.Factory.StartNew(() => { listen(httpListner); });
        }

        public void updateFrom(String indexNo)
        {
            
            if (personMap.ContainsKey(indexNo))
            {
                Person person = personMap[indexNo];


                Image frame0 = Properties.Resources.walking_dead;
                Image frame1 = Properties.Resources.fallen_angles;
                Image frame2 = Properties.Resources.howling_beats;
                Image frame3 = Properties.Resources.blood_seekers;
                Image frame;
                String house = person.team;
                String teamR;
                if (house.Equals("0"))
                {
                    frame = frame0;
                    teamR = "Walking Dead";
                }
                else if (house.Equals("1"))
                {
                    frame = frame1;
                    teamR = "Fallen Angels";
                }
                else if (house.Equals("2"))
                {
                    frame = frame2;
                    teamR = "Howling Beasts";
                }
                else if (house.Equals("3"))
                {
                    frame = frame3;
                    teamR = "Blood Seekers";
                }
                else
                {
                    frame = frame0;
                    teamR = " ";
                }


                lblName.Invoke((MethodInvoker)delegate
                {
                    lblName.Text = person.name;
                });
                lblTeam.Invoke((MethodInvoker)delegate
                {
                    lblTeam.Text = teamR;
                });
                Image profilePic = null;

                //todo: based on the group pick image
                
                pctBox.Image = Properties.Resources.anm_im;
                pctBox.Visible = false;
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 300;
                timer.Tick += (source, e) => { pctBox.Visible = true; timer.Stop(); };
                timer.Start();
                
                


                try
                {
                    String fName = ImageForm_Load();
                    profilePic = Image.FromFile(fName);
                    profilePic = ResizeImage(profilePic, 3105, 4655);
                } catch(Exception ex)
                {
                    MessageBox.Show("Image load filed:" + ex.ToString());
                }

                try
                {
                    using (frame)
                    {
                        using (var bitmap = new Bitmap(frame.Width, frame.Height))
                        {
                            using (var canvas = Graphics.FromImage(bitmap))
                            {
                                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                canvas.DrawImage(profilePic, new Point(((bitmap.Width / 2) - (profilePic.Width / 2)), ((bitmap.Height / 2) - (profilePic.Height / 2))));
                                canvas.DrawImage(frame, new Point(0, 0));
                                canvas.Save();
                            }
                            try
                            {
                                bitmap.Save($"C:\\Night_out\\{person.indexNo}_out.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                Image newBg = Image.FromFile($"C:\\Night_out\\{person.indexNo}_out.jpg");

                                this.BackgroundImage = newBg;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }

                    }
                } catch(Exception ex)
                {

                }
                
                
            }
        }

        public void showPopup()
        {
            Dialog dialog = new Dialog(this);
            dialog.Show(this);
        }
        public frmBg()
        {
            InitializeComponent();
            buildMap();
            startHttpListner();
        }


        private void buildMap()
        {
            personMap = new Dictionary<string, Person>();
            personMap.Add("170182", new Person("170182X", "2", "Madhavi Gayathri "));
            personMap.Add("170535", new Person("170535J", "3", "Shafeek "));
            personMap.Add("170326", new Person("170326U", "2", "Hiruna Kumara "));
            personMap.Add("170144", new Person("170144J", "0", "Hashini Disanayak "));
            personMap.Add("170210", new Person("170210J", "2", "Mithun Wijethunga "));
            personMap.Add("170066", new Person("170066V", "2", "Wageeaha Erangi "));
            personMap.Add("170638", new Person("170638D", "2", "Achintha Isuru "));
            personMap.Add("170214", new Person("170214B", "2", "Missaka Herath "));
            personMap.Add("170431", new Person("170431L", "3", "Thumula Perera "));
            personMap.Add("170346", new Person("170346F", "2", "Ishara Shanuka "));
            personMap.Add("170007", new Person("170007T", "3", "Sachini Abeygunawardhana "));
            personMap.Add("170074", new Person("170074T", "2", "Anju Chamantha "));
            personMap.Add("170645", new Person("170645V", "1", "Yasith Udagedara "));
            personMap.Add("170706", new Person("170706K", "2", "Sepali Wijerathna "));
            personMap.Add("170413", new Person("170413J", "1", "Kashyapa Niyarepola "));
            personMap.Add("170396", new Person("170396G", "0", "Chamika Nandasiri "));
            personMap.Add("170315", new Person("170315K", "3", "Charith Kavinda "));
            personMap.Add("170446", new Person("170446L", "2", "Kavinda Perera "));
            personMap.Add("170016", new Person("170016U", "0", "Jayampathi Adhikari "));
            personMap.Add("170097", new Person("170097P", "1", "Gayal Dassanayake "));
            personMap.Add("170405", new Person("170405L", "1", "NILAAN "));
            personMap.Add("170104", new Person("170104M", "0", "Anutthara de Silva "));
            personMap.Add("170473", new Person("170473P", "1", "Evindu Rajapaksha "));
            personMap.Add("170138", new Person("170138U", "2", "Pasindu Sudesh "));
            personMap.Add("170050", new Person("170050R", "2", "Dineth narada "));
            personMap.Add("170098", new Person("170098U", "2", "Thisun dayarathna "));
            personMap.Add("170417", new Person("170417B", "1", "pakeerathan thukaraka "));
            personMap.Add("170118", new Person("170118J", "2", "Sachintha Denuwan "));
            personMap.Add("170237", new Person("170237X", "1", "Shehana Iqbal "));
            personMap.Add("170512", new Person("170512L", "0", "Dilanka Rathnasiri "));
            personMap.Add("170298", new Person("170298H", "2", "Melanga Kasun "));
            personMap.Add("170176", new Person("170176H", "0", "Damika Gamlath "));
            personMap.Add("170196", new Person("170196T", "0", "Janith Pasindu "));
            personMap.Add("170576", new Person("170576J", "0", "Janaka Shamal "));
            personMap.Add("170034", new Person("170034X", "2", "Shakthi Anjana "));
            personMap.Add("170584", new Person("170584G", "0", "Ashan Silva "));
            personMap.Add("170020", new Person("170020C", "0", "R.Ahrooran "));
            personMap.Add("170337", new Person("170337E", "1", "Rashmika Lakshan "));
            personMap.Add("170251", new Person("170251J", "3", "Gayan Jayakody "));
            personMap.Add("170109", new Person("170109H", "1", "Lahiru Udayanga "));
            personMap.Add("170585", new Person("170585K", "1", "Kaveeaha Silva "));
            personMap.Add("170568", new Person("170568L", "0", "Shashimal Senarath "));
            personMap.Add("170387", new Person("170387f", "3", "Vihan "));
            personMap.Add("170406", new Person("170406P", "2", "kushan chamindu "));
            personMap.Add("170708", new Person("170708T", "0", "Vidura Prasangana "));
            personMap.Add("170681", new Person("170681C", "1", "Lakshan Weerasinghe "));
            personMap.Add("170024", new Person("170024R", "0", "Sasindu Dilshara "));
            personMap.Add("170628", new Person("170628X", "0", "Santhirakumar Thenusan "));
            personMap.Add("170353", new Person("170353A", "1", "Azeem "));
            personMap.Add("170294", new Person("170294R", "2", "imasha "));
            personMap.Add("170025", new Person("170025v", "1", "Thamindu Randil "));
            personMap.Add("170427", new Person("170427F", "3", "Udara Pathum "));
            personMap.Add("170217", new Person("170217L", "1", "Nuwanga Mihiruwan  "));
            personMap.Add("170494", new Person("170494F", "2", "Yoshani Ranaweera "));
            personMap.Add("170580", new Person("170580P", "0", "vimuth "));
            personMap.Add("170517", new Person("170517G", "1", "Yasiru Janith "));
            personMap.Add("170203", new Person("170203P", "3", "Ayesh Sandeepa "));
            personMap.Add("170094", new Person("170094E", "2", "Sasmitha Dasanayaka "));
            personMap.Add("170685", new Person("170685R", "1", "Hasini Weerasooriya "));
            personMap.Add("170482", new Person("170482R", "2", "Asanka Ranasinghe "));
            personMap.Add("170134", new Person("170134E", "2", "Anjala Dilhara "));
            personMap.Add("170342", new Person("170342N", "2", "Minul Lamahewage "));
            personMap.Add("170153", new Person("170153K", "1", "Budvin Chathura "));
            personMap.Add("170479", new Person("170479N", "3", "Rajinthan "));
            personMap.Add("170043", new Person("170043A", "3", "Amanda Ariyaratne "));
            personMap.Add("170313", new Person("170313D", "1", "Arunthavarajah Krishanthan "));
            personMap.Add("170587", new Person("170587T", "3", "Sanju Anupa Silva "));
            personMap.Add("170205", new Person("170205A", "1", "Hemaka Raveen "));
            personMap.Add("170366", new Person("170366P", "2", "Hashan Maduwantha "));
            personMap.Add("170589", new Person("170589C", "1", "Rumesh Madusanka "));
            personMap.Add("170264", new Person("170264C", "0", "Sahan Jayasinghe "));
            personMap.Add("170393", new Person("170393U", "1", "Thamidu Muthukumarana "));
            personMap.Add("170367", new Person("170367U", "3", "Thanuja "));
            personMap.Add("170199", new Person("170199F", "3", "Prabhanu Gunaweera "));
            personMap.Add("170145", new Person("170145M", "1", "Sachini Dissanayaka "));
            personMap.Add("170011", new Person("170011B", "3", "Shashini Abeywickrama "));
            personMap.Add("170588", new Person("170588X", "0", "Sinthujan "));
            personMap.Add("170388", new Person("170388J", "0", "Pathum Mihiranga "));
            personMap.Add("170606", new Person("170606E", "2", "Chamika Sudusinghe "));
            personMap.Add("170612", new Person("170612T", "0", "Thisal Tennakoon "));
            personMap.Add("170031", new Person("170031K", "3", "Ruchin Amaratunga "));
            personMap.Add("170571", new Person("170571N", "3", "Rukmal Senavirathne "));
            personMap.Add("170158", new Person("170158F", "2", "Savindu Ekanayake "));
            personMap.Add("170303", new Person("170303X", "3", "Krishi "));
            personMap.Add("170527", new Person("170527L", "3", "Raveesha Rukshani "));
            personMap.Add("170521", new Person("170521M", "1", "Himashi Rathnayake "));
            personMap.Add("170018", new Person("170018D", "2", "Afra Hussaindeen "));
            personMap.Add("170174", new Person("170174B", "2", "Yogya Gamage "));
            personMap.Add("170504", new Person("170504N", "0", "Jude Ranidu "));
            personMap.Add("170307", new Person("170307M", "3", "Kogul "));
            personMap.Add("170311", new Person("170311U", "3", "Shammi "));
            personMap.Add("170608", new Person("170608L", "0", "Abi "));
            personMap.Add("170478", new Person("170478K", "2", "Dilani "));
            personMap.Add("170046", new Person("170046K", "2", "Uthaya "));
            personMap.Add("170163", new Person("170163P", "3", "Chamodi "));
            personMap.Add("170481", new Person("170481M", "1", "Lakith "));
            personMap.Add("170452", new Person("170452C", "0", "Sammani "));
            personMap.Add("170524", new Person("170524B", "0", "Tharaka Sachintha "));
            personMap.Add("170350", new Person("170350L", "2", "Hiran Lowe "));
            personMap.Add("170454", new Person("170454J", "2", "Kirishnni "));
            personMap.Add("170615", new Person("170615F", "3", "Kasun Tharaka "));
            personMap.Add("170643", new Person("170643M", "3", "Thuvarahan "));
            personMap.Add("170410", new Person("170410X", "2", "V.Nisho "));
            personMap.Add("170425", new Person("170425X", "1", "Primesh "));
            personMap.Add("170096", new Person("170096L", "0", "Dilan dashintha "));
            personMap.Add("170644", new Person("170644R", "0", "Thuvarakan sivagnanam "));
            personMap.Add("170604", new Person("170604V", "0", "Anjalee Sudasinghe "));
            personMap.Add("170320", new Person("170320V", "0", "Nilmani Kulaweera "));
            personMap.Add("170328", new Person("170328D", "0", "Hashara kumarasinghe "));
            personMap.Add("170141", new Person("170141X", "1", "M.Dineskumar "));
            personMap.Add("170610", new Person("170610K", "2", "Janani Sumanapala "));
            personMap.Add("170051", new Person("170051V", "3", "Malith Madushan "));
            personMap.Add("170202", new Person("170202L", "2", "Mohomed shalik "));
            personMap.Add("170531", new Person("170531T", "3", "Sailesh "));
            personMap.Add("170593", new Person("170593H", "1", "Chiran "));
            personMap.Add("170723", new Person("170723J", "3", "ZAMEEL "));
            personMap.Add("170160", new Person("170160E", "0", "Bashana Elikewela "));
            personMap.Add("170032", new Person("170032N", "0", "Anankan "));
            personMap.Add("170081", new Person("170081L", "1", "Sunera Avinash "));
            personMap.Add("170620", new Person("170620P", "0", "Tharmeekan "));
            personMap.Add("170343", new Person("170343T", "3", "S.Lavarthan "));
            personMap.Add("170275", new Person("170275K", "3", "Vihanga Dewmini "));
            personMap.Add("170676", new Person("170676P", "0", "Gihan Ayeshmantha "));
            personMap.Add("170669", new Person("170669X", "1", "Disura Warusawithana "));
            personMap.Add("170243", new Person("170243L", "3", "Isuranga Iniyage "));
            personMap.Add("170414", new Person("170414", "T", "Name"));

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            showPopup();
        }

        private void lblTeam_Click(object sender, EventArgs e)
        {

        }


        private String ImageForm_Load()
        {
            var f1 = GetLastUpdatedFileInDirectory(new DirectoryInfo(@"C:\\Night"));
            return f1[(f1.Count)-1].FullName;

        }


        private List<FileInfo> GetLastUpdatedFileInDirectory(DirectoryInfo directoryInfo)
        {
            FileInfo[] files = directoryInfo.GetFiles();
            List<FileInfo> lastUpdatedFile = new List<FileInfo>();
            DateTime lastUpdate = DateTime.MinValue;
            foreach (FileInfo file in files)
            {
                if (file.LastAccessTime > lastUpdate)
                {
                    lastUpdatedFile.Add(file);
                    lastUpdate = file.LastAccessTime;
                }
            }

            return lastUpdatedFile;
        }

        private void pctBox_Click(object sender, EventArgs e)
        {

        }

    }
}
