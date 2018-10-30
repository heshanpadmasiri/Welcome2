using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Welcome
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Person> personMap;

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

        public void updateFrom(String indexNo)
        {
            string realIndex = new string(indexNo.Take(7).ToArray());

            if (personMap.ContainsKey(realIndex))
            {
                Person person = personMap[realIndex];
                lblName.Text = person.name;
                lblTeam.Text = person.team;
                Image profilePic = null;

                //todo: based on the group pick image
                /*
                pctBox.Image = Properties.Resources.anm_im;
                pctBox.Visible = true;
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 300;
                timer.Tick += (source, e) => { pctBox.Visible = f1alse; timer.Stop(); };
                timer.Start();
                */
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
                        bitmap.Save($"C:\\Night_out\\{person.indexNo}_out.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
                        Image newBg = Image.FromFile($"C:\\Night_out\\{person.indexNo}_out.jpg");
                        this.BackgroundImage = newBg;
                    }
                    
                }
                
            }
        }

        public void showPopup()
        {
            Dialog dialog = new Dialog(this);
            dialog.Show(this);
        }
        public Form1()
        {
            InitializeComponent();
            
            buildMap();
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
    }
}
