using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;


namespace FAV
{
    public partial class Form1 : Form
    {
        int iThisImage = 0;
        int iNumImages = 0;
        string sMovieorTV="movie";
        string sLanguage = "sv";
        string[] sImage = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        string[] sThumb = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
         
            sMovieorTV = Properties.Settings.Default.MovieorTV ;
            if (sMovieorTV == "tv")
            {
                pictureBox4.Image = Properties.Resources.tv;
             }
            else
            {
                pictureBox4.Image = Properties.Resources.clapperboard;
            }

                sLanguage = Properties.Settings.Default.Language;
            if (sLanguage == "sv")
            {
                pictureBox3.Image = Properties.Resources.sweden_flag;
            }
            else
            {
                pictureBox3.Image = Properties.Resources.united_kingdom_flag;
            }

            this.Location = Properties.Settings.Default.Location;
            if (Properties.Settings.Default.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Properties.Settings.Default.WindowState;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
           
            PopulateListbox(Properties.Settings.Default.Path);
        }
        private void PopulateListbox(string MyPath)
        {
            if (Directory.Exists(MyPath)!=true)
            {
                return;
            }
            listBox1.Items.Clear();
            this.Text = "FAV - " + Properties.Settings.Default.Path;
            string[] folders = Directory.GetDirectories(MyPath);
            foreach (string folder in folders)
            {
                string MyFolder="";              
                if (folder.Replace(MyPath, "").Substring(0, 1) == "\\")
                {
                    MyFolder=folder.Replace(MyPath + "\\", "");
                }
                else
                {
                    MyFolder = folder.Replace(MyPath, "");
                }
                if (MyFolder.Substring(0,1)!="$")
                {
                    listBox1.Items.Add(MyFolder);
                }
            }
            listBox1.Sorted = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.WindowState != FormWindowState.Maximized )
            {
                Properties.Settings.Default.FormSize = this.Size;
               
            }
            if(this.Location.X>0 && this.Location.Y>0 )
            {
                Properties.Settings.Default.Location = this.Location;
            }
            
            Properties.Settings.Default.WindowState = this.WindowState;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Properties.Settings.Default.Path;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    PopulateListbox(fbd.SelectedPath.ToString());
                    Properties.Settings.Default.Path = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (Directory.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\"))
            {
                System.Diagnostics.Process.Start("explorer.exe", Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\");
            }
           

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.SelectedItem.ToString();
            button2.Enabled = true;
            if(File.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg"))
            {
                pictureBox2.ImageLocation = Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg";
            }
            else
            {
                pictureBox2.Image= Properties.Resources.Bild_saknas;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Trim();
            if(!Directory.Exists(Properties.Settings.Default.Path + "\\" + textBox1.Text) && Directory.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString()))
             {
                int SelectedIndex = listBox1.SelectedIndex;
                Directory.Move(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString(), Properties.Settings.Default.Path + "\\" + textBox1.Text);
                PopulateListbox(Properties.Settings.Default.Path);
                listBox1.SelectedIndex = listBox1.FindStringExact(textBox1.Text);
            }


        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (textBox1.Text.Contains("."))
                {
                    textBox1.Text = textBox1.Text.Replace(".", " ");
                }
               // else
//                {
  //                  textBox1.Text = textBox1.Text.Replace(" ", ".");
    //            }

            }
            textBox1.SelectAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                button3.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
                button2.Enabled = false;
            }

        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (iNumImages > 1)
            {
                pictureBox1.Cursor = Cursors.Hand;
            }
            else
            {
                pictureBox1.Cursor = Cursors.Default;
            }
                  }

        private void SearchForImage(string SearchFor)
        {
           pictureBox1.Image= Properties.Resources.spiral;
            SearchFor = SearchFor.Replace(".", " ");
            //https://www.themoviedb.org/search/movie?language=sv&query=Barbie%20i%20Svansj%C3%B6n
            //https://www.themoviedb.org/search/tv?language=sv&query=Barbie%20i%20Svansj%C3%B6n
            //https://image.tmdb.org/t/p/w185_and_h278_bestv2/q9OtbSsCowJIdYUAbTwEsHe2hjd.jpg
            //https://image.tmdb.org/t/p/w1280/q9OtbSsCowJIdYUAbTwEsHe2hjd.jpg
            SearchFor = "https://www.themoviedb.org/search/" + sMovieorTV + "?language=" + sLanguage + "&query=" + WebUtility.UrlEncode(SearchFor);
            string MyResult = DownloadToString(SearchFor);
            try
            {
      
                int i = 0;
                iThisImage = 0;
                iNumImages = 0;
                do
                {
                    int index = MyResult.IndexOf("poster lazyload fade");

                    MyResult = MyResult.Substring(index + 1);
                    sImage[i] = MyResult.Split(new string[] { "data-srcset" }, StringSplitOptions.None)[0];
                    sImage[i] = sImage[i].Split(new string[] { "bestv2/" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                    pictureBox1.ImageLocation = "https://image.tmdb.org/t/p/w185_and_h278_bestv2/" + sImage[i];
                    pictureBox1.Refresh();
                    sThumb[i] = "https://image.tmdb.org/t/p/w185_and_h278_bestv2/" + sImage[i];
                    sImage[i] = "https://image.tmdb.org/t/p/w1280/" + sImage[i];
                    i++;
                } while (MyResult.Contains("poster lazyload fade") );
                iNumImages = i;
                if (iNumImages==1)
                {
                    toolTip1.SetToolTip(pictureBox1, "Hittade bara en bild.");
                }
                else
                {
                    toolTip1.SetToolTip(pictureBox1, "Hittade " + iNumImages.ToString() + " bilder. Klicka på bilden för att se nästa.");
                }
                




                pictureBox1.ImageLocation = sImage[0];
                button4.Visible = true;
                button4.Enabled = true;
                if (File.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg"))
                {
                    button4.Text = "Ersätt bilden";
                }
                else
                {
                    button4.Text = "Ladda hem bilden";
                }

               
            }
            catch
            {
                pictureBox1.Image = Properties.Resources.Bild_saknas;
                button4.Visible = false;
                button4.Enabled = false;
                toolTip1.SetToolTip(pictureBox1, "");
            }

            

            // webBrowser1.DocumentText = MyResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SearchForImage(textBox1.Text);
        }

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_Click(this, new EventArgs());
            }
        }

        private string DownloadToString(string sURL)
        {
            string htmlCode = "";
            using (WebClient client = new WebClient()) 
            {
                htmlCode = client.DownloadString(sURL);
            }
            return htmlCode;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Image tempImg = pictureBox1.Image;
            pictureBox1.Image = pictureBox2.Image;
            pictureBox2.Image = tempImg;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.bak"))
            {
                File.Delete( Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.bak");
            }
            if (File.Exists(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg"))
            {
                File.Move(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg", Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.bak");
            }
            pictureBox1.Image.Save(Properties.Settings.Default.Path + "\\" + listBox1.SelectedItem.ToString() + "\\folder.jpg", ImageFormat.Jpeg);
            pictureBox2.Image = pictureBox1.Image;
            //button4.Text = "Klart!";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (iNumImages > 1)
            {
                //pictureBox1.Image = Properties.Resources.spiral;
                iThisImage += 1;
                        
                if (iThisImage > iNumImages-1)
                {
                    iThisImage = 0;
                }
                pictureBox1.ImageLocation = sThumb[iThisImage];
                pictureBox1.Refresh();
                pictureBox1.ImageLocation = sImage[iThisImage];
               
                toolTip1.SetToolTip(pictureBox1, "Bild nummer " + (iThisImage + 1).ToString() + " av " + iNumImages.ToString() + " visas nu. Klicka på bilden för att se nästa.");
               
            }
           
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(sLanguage=="sv" )
            {
                pictureBox3.Image = Properties.Resources.united_kingdom_flag;
                sLanguage = "en";
                Properties.Settings.Default.Language = "en";
                Properties.Settings.Default.Save();

            }
            else
            {
                pictureBox3.Image = Properties.Resources.sweden_flag;
                sLanguage = "sv";
                Properties.Settings.Default.Language = "sv";
                Properties.Settings.Default.Save();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (sMovieorTV  == "movie")
            {
                pictureBox4.Image = Properties.Resources.tv;
                sMovieorTV = "tv";
                Properties.Settings.Default.MovieorTV = "tv";
                Properties.Settings.Default.Save();

            }
            else
            {
                pictureBox4.Image = Properties.Resources.clapperboard;
                sMovieorTV = "movie";
                Properties.Settings.Default.MovieorTV = "movie";
                Properties.Settings.Default.Save();

            }
        }
    }
}

