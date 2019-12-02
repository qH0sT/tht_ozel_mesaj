using Gecko;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace tht_ozel_mesaj
{
    public partial class Form1 : Form
    {
        /*
          
          
  _________                                     ____  __.           __              __________ 
 /   _____/____     ____   ____ ___________    |    |/ _|____      |__| _____   ____\______   \
 \_____  \\__  \   / ___\ /  _ \\____ \__  \   |      < \__  \     |  |/     \_/ __ \|       _/
 /        \/ __ \_/ /_/  >  <_> )  |_> > __ \_ |    |  \ / __ \_   |  |  Y Y  \  ___/|    |   \
/_______  (____  /\___  / \____/|   __(____  / |____|__ (____  /\__|  |__|_|  /\___  >____|_  /
        \/     \//_____/        |__|       \/          \/    \/\______|     \/     \/       \/ 


         * */
        public Form1()
        {
            InitializeComponent();
            Xpcom.Initialize("Firefox");
            PromptFactory.PromptServiceCreator = () => new FilteredPromptService(); // javascript uyarı mesajlarını kapamak için.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (Properties.Settings.Default.BeniHatirla != "")
            {
                checkBox1.Checked = true;
                textBox1.Text = Properties.Settings.Default.BeniHatirla.Split('|')[0];
                textBox1.Text = Properties.Settings.Default.BeniHatirla.Split('|')[1];
            }
            if(Properties.Settings.Default.Timer != "")
            {
                numericUpDown1.Value = int.Parse(Properties.Settings.Default.Timer);
            }
            if (Properties.Settings.Default.ses_dosyasi != string.Empty)
            {
                textBox3.Text = Properties.Settings.Default.ses_dosyasi;
                checkBox3.Checked = true;
            }
            else
            {
                textBox3.Text = Environment.CurrentDirectory + "\\notify.wav";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                geckoWebBrowser1.Navigate("https://www.turkhackteam.org/#login");
                label1.Text = textBox1.Text;
                //geckoWebBrowser1.Navigate("https://www.turkhackteam.org/");
            }
        }
        int mesajsayisi = 0;
        List<GeckoElement> tempo = new List<GeckoElement>();
        private void geckoWebBrowser1_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
           
            if (e.Uri.ToString().Contains("https://www.turkhackteam.org/private.php"))
            {
                mesajsayisi = 0;
                tempo.Clear();
                listView1.Items.Clear();
                //listBox1.Items.Add("Sayfa doldu: " + e.Uri.ToString());
                foreach (GeckoHtmlElement bugun in geckoWebBrowser1.Document.GetElementsByTagName("tbody"))
                {
                    if (bugun.GetAttribute("id") == "collapseobj_pmf0_today")
                    {
                        var icerik = bugun.GetElementsByTagName("tr");
                        foreach(var childnode in icerik)
                        {
                            var icerik2 = childnode.GetElementsByTagName("td");
                            
                            foreach(var child2 in icerik2)
                            {
                                if(child2.GetAttribute("class") == "alt1 alt1Active")
                                {
                                    tempo.Add(child2);
                                   
                                    mesajsayisi += 1;                                  
                                    ListViewItem lvi = new ListViewItem(child2.TextContent);
                                    lvi.SubItems.Add(child2.GetAttribute("id"));
                                    listView1.Items.Add(lvi);
                                  
                                }
                            }
                        }
                    }
                }
                label4.Text = mesajsayisi.ToString();
                if (timer1.Enabled == false)
                {
                    timer1.Interval = (int)numericUpDown1.Value * 1000;
                    timer1.Enabled = true; timer1.Start();
                }
                acildi = true;
            }
            else if (e.Uri.ToString().Contains("#login"))
            {
                listBox1.Items.Add("Sayfa doldu: " + e.Uri.ToString());
                GeckoElement username = geckoWebBrowser1.Document.GetElementById("navbar_username");
                username.SetAttribute("value",textBox1.Text);

                GeckoElement password = geckoWebBrowser1.Document.GetElementById("navbar_password");
                password.SetAttribute("value" , textBox2.Text);
                listBox1.Items.Add("Üyelik bilgileri aktarıldı: " + textBox1.Text + "  " +Convert.ToBase64String(Encoding.UTF8.GetBytes(textBox2.Text)));

                foreach(GeckoHtmlElement girisButonu in geckoWebBrowser1.Document.GetElementsByTagName("input"))
                {
                    if(girisButonu.GetAttribute("class") == "spe-button4")
                    {
                        girisButonu.Click();
                        listBox1.Items.Add("Giriş Yap butonuna tıklandı.");
                        break;
                    }
                }
            }
            else if (e.Uri.ToString().Contains("https://www.turkhackteam.org/login.php?do=login"))
            {
                listBox1.Items.Add("Sayfa doldu: " + e.Uri.ToString());
                GeckoHtmlElement element = null;
                GeckoElement geckoDomElement = geckoWebBrowser1.Document.DocumentElement;
                if (geckoDomElement is GeckoHtmlElement)
                {
                    element = (GeckoHtmlElement)geckoDomElement;
                    string icerik = element.InnerHtml;
                    if (icerik.Contains("Yanlış kullanıcı adı veya şifre girildi."))
                    {
                        listBox1.Items.Add("Yanlış kullanıcı adı veya şifre girildi.");
                        MessageBox.Show("Yanlış kullanıcı adı veya şifre","Giriş",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        return;
                    }
                }

            }
            else if(e.Uri.ToString() == "https://www.turkhackteam.org/")
            {
                try
                {
                    foreach (GeckoHtmlElement pp in geckoWebBrowser1.Document.GetElementsByTagName("img"))
                    {
                        if (pp.GetAttribute("title") == "Avatar")
                        {

                            using (WebClient baytar = new WebClient())
                            {
                                baytar.Headers.Add("User-Agent: Other");
                                using (Stream sago_go_go = baytar.OpenRead("https://www.turkhackteam.org/" + pp.GetAttribute("src")))
                                {
                                    pictureBox1.Image = Image.FromStream(sago_go_go);
                                }
                            }
                            break;
                        }
                    }
                }catch(Exception ex) { listBox1.Items.Add("Profil fotosu hatası: "+ex.Message); }
                listBox1.Items.Add("Sayfa doldu: " + e.Uri.ToString());
                geckoWebBrowser1.Navigate("https://www.turkhackteam.org/private.php");
            }
            
        }
     
        bool acildi = false;
        private void geckoWebBrowser1_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        //https://www.youtube.com/watch?v=01n1S9b7QUE
      
        private void label4_TextChanged(object sender, EventArgs e) //coded by sagopa k | melankolia 2 0 1 9 kuzen.
        {
            //MessageBox.Show("changed"); test için eklemiştim.
            if (acildi == true)
            {
                if (tempo.Count != 0)
                {
                    if (checkBox3.Checked && textBox3.Text != string.Empty)
                    {
                        SoundPlayer bildiri = new SoundPlayer(@textBox3.Text);
                        bildiri.Play();
                    }
                    notifyIcon1.BalloonTipTitle = "Yeni Mesaj!";
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipText = tempo[0].TextContent;
                    notifyIcon1.ShowBalloonTip(2000);
                }
            }
          
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown1.Value * 1000;
            Properties.Settings.Default.Timer = numericUpDown1.Value.ToString();
            Properties.Settings.Default.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            geckoWebBrowser1.Reload();
        }

        private void seçiliMesajıOkuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                geckoWebBrowser2.Navigate("https://www.turkhackteam.org/private.php?do=showpm&pmid=" + listView1.SelectedItems[0].SubItems[1].Text.Replace("m", ""));
            }
            catch (Exception) { }
        }

        private void geckoWebBrowser2_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            if(e.Uri.ToString() != "about:blank")
            tabControl1.SelectedTab= tabControl1.TabPages[3];
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                if (checkBox1.Checked)
                {
                    Properties.Settings.Default.BeniHatirla = textBox1.Text + "|" + textBox2.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.BeniHatirla = "";
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                textBox2.PasswordChar = '*';
            }
            else
            {
                textBox2.PasswordChar = '\0';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog op = new OpenFileDialog())
            {
                op.Title = "Bir .wav dosyası seçin.";
                op.Filter = "(*.wav)|*.wav";
                if(op.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = op.FileName;
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                if (textBox3.Text != string.Empty)
                {
                    Properties.Settings.Default.ses_dosyasi = textBox3.Text;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                Properties.Settings.Default.ses_dosyasi = "";
                Properties.Settings.Default.Save();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedIndex != 3 && geckoWebBrowser2.Url.ToString().Contains("https"))
                {
                    geckoWebBrowser2.Navigate("about:blank");
                }
            }
            catch (Exception) { }
        }

        private void gösterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Opacity = 100;
            ShowInTaskbar = true;
        }

        private void gizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Opacity = 0;
            ShowInTaskbar = false;
        }
    }
}
