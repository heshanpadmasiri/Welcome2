using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
            

            var jsonObj = JObject.Parse(input);
            string indexNo = (string)jsonObj["indexNo"];
            updateFrom(indexNo);
            Console.WriteLine("Done");
        }

        private void startHttpListner()
        {
            var httpListner = new HttpListener();
            this.listener = httpListner;
            httpListner.Prefixes.Add("http://localhost:8080/");
            httpListner.Start();
            Task.Factory.StartNew(() => { listen(httpListner); });
        }

        public void updateFrom(String indexNo)
        {
            
            if (personMap.ContainsKey(indexNo))
            {
                Person person = personMap[indexNo];

                lblName.Invoke((MethodInvoker)delegate
                {
                    lblName.Text = person.name;
                });
                lblTeam.Invoke((MethodInvoker)delegate
                {
                    lblTeam.Text = person.team;
                });
                Image profilePic = null;

                //todo: based on the group pick image
                
                //pctBox.Image = Properties.Resources.anm_im;
                //pctBox.Visible = false;
                //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                //timer.Interval = 300;
                //timer.Tick += (source, e) => { pctBox.Visible = true; timer.Stop(); };
                //timer.Start();
                
                Image frame = Properties.Resources.bg_red;
                try
                {
                    profilePic = Image.FromFile($"C:\\Night\\pic.jpg");
                    profilePic = ResizeImage(profilePic, 3105, 4655);
                } catch(Exception ex)
                {
                    MessageBox.Show("Image load filed:" + ex.ToString());
                }
                
                
                using (frame)
                {
                    using(var bitmap = new Bitmap(frame.Width, frame.Height))
                    {
                        using(var canvas = Graphics.FromImage(bitmap))
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
            personMap.Add("160451M", new Person("Heshan", "Team1", "160451M"));
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            showPopup();
        }

        private void lblTeam_Click(object sender, EventArgs e)
        {

        }

        private void pctBox_Click(object sender, EventArgs e)
        {

        }
    }
}
