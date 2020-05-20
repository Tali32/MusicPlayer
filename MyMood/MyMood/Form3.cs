using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MyMood
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                Thread.CurrentThread.CurrentCulture =
                    CultureInfo.CreateSpecificCulture(Properties.Settings.Default.Language);
                Thread.CurrentThread.CurrentUICulture =
                    CultureInfo.CreateSpecificCulture(Properties.Settings.Default.Language);
            }
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Form3));
                resources.ApplyResources(c, c.Name, new CultureInfo(Properties.Settings.Default.Language));
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "Mp3|*.mp3",
                Multiselect = false,
                ValidateNames = true
            };
            file.ShowDialog();

            label5.Text = Convert.ToString(file.FileName);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form2.form3 = null;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            MyAudio audio = new MyAudio();

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || label5.Text == "")
            {
                MessageBox.Show(Localization.Not_added_aud_text, Localization.Incorrect_data_title,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                audio.Name = textBox1.Text;
                audio.Style = textBox2.Text;
                audio.Group = textBox3.Text;
                audio.Path = label5.Text;

                Form2.playlist.Playlist_.Add(audio);
                this.Close();
                Form2.form3 = null;
            }
        }
    }
}