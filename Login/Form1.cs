using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyClasses;

namespace Login
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool isConnected()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isConnected())
            {
                string req = "select * from utilisateur where nomutilisateur='" + textBox1.Text + "' and pwd='" + textBox2.Text + "'";
                ConnectionDB conn = ConnectionDB.getInstance();
                if (conn.nombreSelectionner(req) > 0)
                {
                    DataSet ds = conn.selectionner(req);
                    LastUserConnection usr = new LastUserConnection();
                    usr.ajouter(ds.Tables[0].Rows[0][0].ToString());
                    this.Close();
                }
                else
                    MessageBox.Show("login incorrect");
            }
            else
                MessageBox.Show("Connect to internet please");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //************************** Deplacement  du form *****************************
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }       
        //*************************************************

        
    }
}
