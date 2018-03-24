using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comida.V2
{
    public partial class LogoutAlert : Form
    {
        public LogoutAlert()
        {
            InitializeComponent();
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            // to start new instance of application
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            //to turn off current app
            Close();
        }
    }
}
