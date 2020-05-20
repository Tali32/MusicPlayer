using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WMPLib;

namespace MyMood
{
    public partial class Form1 : Form
    {
        public static Form2 form2 = null;
        public static Playlist playlist = new Playlist();

        WindowsMediaPlayer wmp = null; 
        bool audio_playing;

        public Form1()
        {
            if(!String.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                Thread.CurrentThread.CurrentUICulture =
                    CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
                Thread.CurrentThread.CurrentCulture =
                    CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            }
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.ComboBox.BindingContext = this.BindingContext;
            toolStripComboBox1.ComboBox.DataSource = new CultureInfo[] {
                CultureInfo.GetCultureInfo("en"),
                CultureInfo.GetCultureInfo("uk"),
            };
            toolStripComboBox1.ComboBox.DisplayMember = "NativeName";
            toolStripComboBox1.ComboBox.ValueMember = "Name";

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Language))
                toolStripComboBox1.ComboBox.SelectedValue = Properties.Settings.Default.Language;

            wmp = new WindowsMediaPlayer();

            wmp.settings.volume = trackBar1.Value;
            label3.Text = Convert.ToString(trackBar1.Value + "%");

            audio_playing = false;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            wmp.settings.volume = trackBar1.Value;
            label3.Text = Convert.ToString(trackBar1.Value + "%");
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(File.Exists(playlist.Playlist_[listBox1.SelectedIndex].Path))
            {
                wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                timer1.Enabled = true;
                audio_playing = true;
                myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                    playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
            }
            else
            {
                MessageBox.Show(Localization.Wrong_path_text, Localization.Wrong_path_title,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                playlist.Playlist_.RemoveAt(listBox1.SelectedIndex);

                if (playlist.Playlist_.Count != 0)
                {
                    if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Playlists"))
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
                }

                listBox1.Items.Clear();
                int i = 1;
                foreach (MyAudio elem in playlist.Playlist_)
                {
                    listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                    i++;
                }
            }
        }

        private void MyButton1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show(Localization.Incorrect_select_text,
                    Localization.Incorrect_select_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (!audio_playing)
                {
                    wmp.controls.play();
                    audio_playing = true;
                    myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                    label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                        playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                }
                else
                {
                    wmp.controls.pause();
                    audio_playing = false;
                    myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\play.png");
                    label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                        playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Pausing;
                }
            }
        }

        private void MyButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show(Localization.Incorrect_select_text,
                    Localization.Incorrect_select_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (listBox1.SelectedIndex == 0)
                {
                    if (File.Exists(playlist.Playlist_[listBox1.Items.Count - 1].Path))
                    {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                        wmp.controls.play();
                        audio_playing = true;
                        myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                        label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                            playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                    }
                    else
                    {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }
                }
                else
                {
                    if (File.Exists(playlist.Playlist_[listBox1.SelectedIndex - 1].Path))
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                        wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                        wmp.controls.play();
                        audio_playing = true;
                        myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                        label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                            playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                    }
                    else
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                    }
                }
            }
        }

        private void MyButton3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show(Localization.Incorrect_select_text,
                    Localization.Incorrect_select_title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                {
                    if (File.Exists(playlist.Playlist_[0].Path))
                    {
                        listBox1.SelectedIndex = 0;
                        wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                        wmp.controls.play();
                        audio_playing = true;
                        myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                        label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                            playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                    }
                    else
                    {
                        listBox1.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (File.Exists(playlist.Playlist_[listBox1.SelectedIndex + 1].Path))
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                        wmp.controls.play();
                        audio_playing = true;
                        myButton1.BackgroundImage = System.Drawing.Image.FromFile($@"{AppDomain.CurrentDomain.BaseDirectory}resources\pause.png");
                        label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                            playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                    }
                    else
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                    }
                }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if(Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Playlists"))
            {
                OpenFileDialog file = new OpenFileDialog()
                {
                    InitialDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}Playlists",
                    Filter = "Mp3|*.mp3|Data|*.dat",
                    Multiselect = false,
                    ValidateNames = true
                };
                file.ShowDialog();

                Regex regex_mp3 = new Regex(@"\w*mp3$");
                Regex regex_dat = new Regex(@"\w*dat$");
                MatchCollection matches_mp3 = regex_mp3.Matches(file.FileName);
                MatchCollection matches_dat = regex_dat.Matches(file.FileName);

                if (matches_mp3.Count > 0)
                {
                    MyAudio myAudio = new MyAudio($"Audio{playlist.Playlist_.Count+1}", "style", "Group", file.FileName);
                    playlist.Playlist_.Add(myAudio);
                    playlist.Name = "Default";
                    listBox1.Items.Clear();

                    int i = 1;
                    foreach (MyAudio elem in playlist.Playlist_)
                    {
                        listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                        i++;
                    }

                    label6.Text = playlist.Name;
                }
                else if(matches_dat.Count > 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream filest = new FileStream($"{file.FileName}", FileMode.Open))
                    {
                        playlist = (Playlist)formatter.Deserialize(filest);
                    }

                    listBox1.Items.Clear();
                    int i = 1;
                    foreach (MyAudio elem in playlist.Playlist_)
                    {
                        listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                        i++;
                    }

                    label6.Text = playlist.Name;
                }
                else
                {
                    MessageBox.Show(Localization.Not_founded_pllist_text,
                        Localization.Incorrect_select_title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}Playlists");

                OpenFileDialog file = new OpenFileDialog()
                {
                    InitialDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}Playlists",
                    Filter = "Mp3|*.mp3|Data|*.dat",
                    Multiselect = false,
                    ValidateNames = true
                };
                file.ShowDialog();

                Regex regex_mp3 = new Regex(@"\w*mp3$");
                Regex regex_dat = new Regex(@"\w*dat$");
                MatchCollection matches_mp3 = regex_mp3.Matches(file.FileName);
                MatchCollection matches_dat = regex_dat.Matches(file.FileName);

                if (matches_mp3.Count > 0)
                {
                    MyAudio myAudio = new MyAudio($"Audio{playlist.Playlist_.Count + 1}", "style", "Group", file.FileName);
                    playlist.Playlist_.Add(myAudio);
                    playlist.Name = "Default";
                    listBox1.Items.Clear();

                    int i = 1;
                    foreach (MyAudio elem in playlist.Playlist_)
                    {
                        listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                        i++;
                    }

                    label6.Text = playlist.Name;
                }
                else if (matches_dat.Count > 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream filest = new FileStream($"{file.FileName}", FileMode.Open))
                    {
                        playlist = (Playlist)formatter.Deserialize(filest);
                    }

                    listBox1.Items.Clear();
                    int i = 1;
                    foreach (MyAudio elem in playlist.Playlist_)
                    {
                        listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                        i++;
                    }

                    label6.Text = playlist.Name;
                }
                else
                {
                    MessageBox.Show(Localization.Not_founded_pllist_text,
                        Localization.Incorrect_select_title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (form2 == null)
            {
                form2 = new Form2();
                form2.ShowDialog();
            }
            else
                form2.ShowDialog();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            if(audio_playing)
            {
                wmp.controls.pause();
                Thread.Sleep(200);
                audio_playing = false;
                wmp.controls.currentPosition = trackBar2.Value;
                wmp.controls.play();
                audio_playing = true;
            }
            else
            {
                wmp.controls.currentPosition = trackBar2.Value;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            trackBar2.Maximum = Convert.ToInt32(wmp.currentMedia.duration);
            trackBar2.Value = Convert.ToInt32(wmp.controls.currentPosition);
 
            if (wmp != null)
            {
                int s = (int)wmp.currentMedia.duration;
                int m = (s - ((s / 3600) * 3600)) / 60;
                s = s - ((s / 3600) * 3600 + m * 60);
                label4.Text = String.Format("{0:D}:{1:D2}", m, s);
 
                s = (int)wmp.controls.currentPosition;
                m = (s - ((s / 3600) * 3600)) / 60;
                s = s - ((s / 3600) * 3600 + m * 60);
                label7.Text = String.Format("{0:D}:{1:D2}", m, s);

                if ((int)wmp.controls.currentPosition == (int)wmp.currentMedia.duration - 1) 
                {
                    if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                    {
                        if (File.Exists(playlist.Playlist_[0].Path))
                        {
                            listBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            listBox1.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        if (File.Exists(playlist.Playlist_[listBox1.SelectedIndex + 1].Path))
                        {
                            listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                            wmp.URL = playlist.Playlist_[listBox1.SelectedIndex].Path;
                            wmp.controls.play();
                            audio_playing = true;
                            label1.Text = playlist.Playlist_[listBox1.SelectedIndex].Name + " - " +
                                playlist.Playlist_[listBox1.SelectedIndex].Group + " - " + Localization.Playing;
                        }
                        else
                        {
                            MessageBox.Show(Localization.Wrong_path_text, Localization.Wrong_path_title,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            playlist.Playlist_.RemoveAt(listBox1.SelectedIndex + 1);

                            if (playlist.Playlist_.Count != 0)
                            {
                                if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Playlists"))
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
                            }

                            listBox1.Items.Clear();
                            int i = 1;
                            foreach (MyAudio elem in playlist.Playlist_)
                            {
                                listBox1.Items.Add(i + ". " + elem.Name + " - " + elem.Group + "  " + elem.Style + $" ({elem.Path})");
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                label4.Text = "0:00";
                label7.Text = "0:00";
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyResourceToToolStripItemCollection(ToolStripItemCollection col, ComponentResourceManager res, CultureInfo lang)
        {
            for (int i = 0; i < col.Count; i++)
            {
                if (col[i] == toolStripComboBox1)
                { }
                else
                {
                    ToolStripItem item = (ToolStripMenuItem)col[i];

                    if (item.GetType() == typeof(ToolStripMenuItem))
                    {
                        ToolStripMenuItem menuitem = (ToolStripMenuItem)item;
                        ApplyResourceToToolStripItemCollection(menuitem.DropDownItems, res, lang);
                    }

                    res.ApplyResources(item, item.Name, lang);
                }
            }
        }

        private void ToolStripComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = toolStripComboBox1.ComboBox.SelectedValue.ToString();

            Thread.CurrentThread.CurrentCulture = 
                CultureInfo.CreateSpecificCulture(Properties.Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = 
                CultureInfo.CreateSpecificCulture(Properties.Settings.Default.Language);
            ComponentResourceManager resources = new ComponentResourceManager(this.GetType());
            foreach (Control c in this.Controls)
            {
                resources.ApplyResources(c, c.Name, new CultureInfo(toolStripComboBox1.ComboBox.SelectedValue.ToString()));

                if (c.GetType() == typeof(MenuStrip))
                {
                    MenuStrip strip = (MenuStrip)c;
                    ApplyResourceToToolStripItemCollection(strip.Items, resources, new CultureInfo(toolStripComboBox1.ComboBox.SelectedValue.ToString()));
                }
            }

            toolStripMenuItem3.DropDown.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Language = toolStripComboBox1.ComboBox.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }
    }
}