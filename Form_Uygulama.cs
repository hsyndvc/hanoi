﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;

namespace Hanoi_Towers
{
    public partial class Form_Uygulama : Form
    {
        public Form_Uygulama()
        {
            InitializeComponent();
        }

        static string con2 = "Provider=Microsoft.ACE.Oledb.12.0; Data Source=siralamaa.accdb";
        OleDbConnection connection = new OleDbConnection(con2);
        OleDbCommand komut = new OleDbCommand();

        string ad;
        int disksayi;
        int hamle;
        int süre;
        public static bool formAcikMi = false;
        Random rasgeleSayi = new Random();
        int hamleSayisi = 0;
        int buttonKonumX;
        int buttonKonumY;
        int buttonGenislik;
        bool gidilebilirMi;
        Button b;
        Point buttonKonum;
        bool oyunBitti = false;
        int buttonSayisi;
        int saniye = 0;

        private void tablogoster()
        {
            try
            {
                //"select id AS[ID NO],kullanici_adi AS[KULLANICI ADI],disk_sayi AS[DİSK SAYISI],hamle AS[HAMLE SAYISI],sure AS[SÜRE]from sıralamalar Order By ad ASC", connection
                //"select * from sıralamalar", connection
                connection.Open();
                //OleDbDataAdapter verilistele = new OleDbDataAdapter("select * from sıralamalar", connection);
                OleDbDataAdapter verilistele = new OleDbDataAdapter("select kullanici_adi, disk_sayi, hamle, sure from sıralama", connection);
                DataSet dshafiza = new DataSet();
                verilistele.Fill(dshafiza);
                dataGridView1.DataSource = dshafiza.Tables[0];
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hanoi Kule Oyunu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Close();
            }
        }
        private void veriekle()
        {
            try
            {
                string sqlKomut = "INSERT INTO sıralama(kullanici_adi,disk_sayi,hamle,sure) VALUES('" + ad + "'," + disksayi + "," + hamle + "," + süre + ");";

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                komut.CommandText = sqlKomut;
                komut.Connection = connection;
                komut.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void butonlar_MouseDown(object sender, MouseEventArgs e)
        {
            b = sender as Button;
            buttonKonumX = b.Location.X;
            buttonKonumY = b.Location.Y;
            buttonKonum = b.Location;
            buttonGenislik = b.Width;

            bool gidebilirmi = true;
            foreach (Button panelButton in b.Parent.Controls)
            {
                if (buttonGenislik > panelButton.Width)
                    gidebilirmi = false;
            }
            if (gidebilirmi == true)
            {
                DragDropEffects DDE = DragDropEffects.All;
                panel1.DoDragDrop(b, DDE);
            }

        }
        private void panel_DragDrop(object sender, DragEventArgs e)
        {
            Panel HedefP = sender as Panel;
            if (e.Data.GetData(typeof(Button)) is Button)
            {
                Button SuruklenenButton = (Button)e.Data.GetData(typeof(Button));
                int sayac = 1;
                gidilebilirMi = true;
                foreach (Button pButton in HedefP.Controls)
                {
                    if (pButton.Width < buttonGenislik)
                        gidilebilirMi = false;
                    sayac++;
                }
                if (gidilebilirMi == true && b.Parent != HedefP)
                {
                    listBox1.Items.Add(SuruklenenButton.Name + "  " + b.Parent.Name.ToString() + " -> " + HedefP.Name.ToString());
                    SuruklenenButton.Location = new Point(buttonKonumX, HedefP.Height - sayac * 30);
                    HedefP.Controls.Add(SuruklenenButton);
                    hamleSayisi++;
                    labelHamleSayisi.Text = "Hamle Sayisi : " + hamleSayisi.ToString();
                    int p3BSayisi = 0;
                    foreach (Button pButton in panel3.Controls)
                    {
                        p3BSayisi++;
                    }
                    if (HedefP == panel3 && p3BSayisi == buttonSayisi)
                    {
                        foreach (Button pButton in panel3.Controls)
                        {
                            if (buttonGenislik < pButton.Width)
                            {
                                oyunBitti = true;
                            }
                        }
                    }
                    if (oyunBitti == true && p3BSayisi == buttonSayisi)
                    {
                        timer_sure.Stop();
                        int dakika = saniye / 60;
                        MessageBox.Show(labelKullanıcı.Text + " " + hamleSayisi.ToString() + " Hamle de yaptın. " + "\nSüreniz : " + (saniye > 60 ? dakika + "  Dakika  " + (saniye - (dakika * 60)).ToString() + " Saniye" : saniye.ToString() + " Saniye"), "Tebrikler Oyunu Bitirdiniz.");
                        ad = Convert.ToString(labelKullanıcı.Text);
                        disksayi = Convert.ToInt16(cB1.Text);
                        hamle = hamleSayisi;
                        süre = saniye;
                        veriekle();
                        tablogoster();
                        panelTemizle();
                    }
                }
            }
        }
        private void panel_DragOver(object sender, DragEventArgs e)
        {
            if (e.KeyState == 1)
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void Form_Uygulama_Load(object sender, EventArgs e)
        {
            labelHamleSayisi.Text = "";
            labelSure.Text = "";
            labelKullanıcı.Text = "";
            this.Activate();
            this.Focus();
            cB1.SelectedIndex = 0;
            btnOyna.Enabled = false;
            button1.Enabled = false;
            tablogoster();

        }
        public void panelTemizle()
        {
            panel1.Controls.Clear();
            panel2.Controls.Clear();
            panel3.Controls.Clear();
            listBox1.Items.Clear();
            labelKullanıcı.Text = "";
            labelHamleSayisi.Text = "";
            labelSure.Text = "";
            timer_sure.Stop();
        }
        public void yeniOyun()
        {
            panelTemizle();
            hamleSayisi = 0;
            labelHamleSayisi.Text = "Hamle Sayisi : " + hamleSayisi.ToString();
            labelKullanıcı.Text = textBox1.Text;
            buttonSayisi = Int32.Parse(cB1.SelectedItem.ToString());
            int bXkonum = ((panel1.Width - (buttonSayisi * 20)) / 2);
            for (int i = buttonSayisi; i > 0; i--)
            {
                Button butonlar = new Button();
                butonlar.Location = new System.Drawing.Point(bXkonum, 390 - (((buttonSayisi + 1) - i) * 30));
                butonlar.Name = "Disk " + i.ToString();
                butonlar.Size = new System.Drawing.Size(20 * i, 30);
                butonlar.BackColor = Color.FromArgb(rasgeleSayi.Next(255), rasgeleSayi.Next(255), rasgeleSayi.Next(255));
                butonlar.FlatStyle = FlatStyle.Flat;
                butonlar.Text = i.ToString();
                panel1.Controls.Add(butonlar);
                butonlar.Cursor = Cursors.SizeAll;
                butonlar.MouseDown += butonlar_MouseDown;
                bXkonum += 10;
            }
            saniye = 0;
            timer_sure.Start();
        }
        private void timer_sure_Tick(object sender, EventArgs e)
        {
            saniye++;
            labelSure.Text = " Süreniz(sn) :" + saniye.ToString();
        }
        private void btnOyna_Click(object sender, EventArgs e)
        {
            yeniOyun();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                btnOyna.Enabled = false;
            else
                btnOyna.Enabled = true;
        }
    }
}