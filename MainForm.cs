using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    public partial class MainForm : Form, IHookCallback
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!Visible)
            {

                Visible = true;
            }
        }

        void IHookCallback.OnKeyDown(Keys key)
        {
            Console.WriteLine(key);
        }

        void IHookCallback.OnKeyUp(Keys key)
        {
            
        }
    }
}
