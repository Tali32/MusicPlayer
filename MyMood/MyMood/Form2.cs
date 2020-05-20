using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace MyMood
{
    public partial class Form2 : Form
    {
        public static Form3 form3 = null;
        public static Playlist playlist;

        public Form2()
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

        private void Form2_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Form2));
                resources.ApplyResources(c, c.Name, new CultureInfo(Properties.Settings.Default.Language));
            }

            playlist = new Playlist();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1.form2 = null;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (form3 == null)
            {
                form3 = new Form3();
                form3.ShowDialog();
            }
            else
                form3.ShowDialog();

            if (playlist.Playlist_ != null)
            {
                listBox1.Items.Clear();
                int i = 1;
                foreach (MyAudio elem in playlist.Playlist_)
                {
                    listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                    i++;
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            playlist.Playlist_.RemoveAt(listBox1.SelectedIndex);

            if (playlist.Playlist_ != null)
            {
                listBox1.Items.Clear();
                int i = 1;
                foreach (MyAudio elem in playlist.Playlist_)
                {
                    listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                    i++;
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                MessageBox.Show(Localization.Not_saved_pllist_text, Localization.Incorrect_data_title,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                playlist.Name = textBox1.Text;

                if(Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Playlists"))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream file = new FileStream($"Playlists\\{playlist.Name}.dat", FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(file, playlist);
                    }
                }
                else
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}Playlists");
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream file = new FileStream($"Playlists\\{playlist.Name}.dat", FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(file, playlist);
                    }
                }

                this.Close();
                Form1.form2 = null;
            }
        }
    }
}