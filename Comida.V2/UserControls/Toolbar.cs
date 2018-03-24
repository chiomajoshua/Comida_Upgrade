using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comida.V2.UserControls
{
    public partial class Toolbar : UserControl
    {
        private int _formtype;
        public Toolbar()
        {
            InitializeComponent();
        }

        private void Toolbar_Load(object sender, EventArgs e)
        {
            _formtype = CafSelect.ControlId.FormId;
            if (_formtype == 1)
            {
                pictureBox2.Visible = false;
            }
        }
    }
}
