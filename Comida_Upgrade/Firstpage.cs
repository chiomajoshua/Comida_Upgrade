using System;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using InfoBox;

namespace Comida_Upgrade
{
    public partial class Firstpage : Form
    {
        public Firstpage()
        {
            InitializeComponent();
        }

        private void Firstpage_Load(object sender, EventArgs e)
        {
        }

        private void Firstpage_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.X) return;
            if (
                InformationBox.Show("Do You Really Want To Exit The App?", "Comida Administrator",
                    InformationBoxButtons.YesNo, InformationBoxIcon.Question).ToString() == "No")
            {
                ActiveControl = textBox1;
                textBox1.Focus();
            }
            else
            {
                Application.Exit();
                Application.ExitThread();
            }
        }

        private void Loadxml()
        {
            try
            {
                var to = textBox1.Text.Trim();
                var url = "http://exeat.run.edu.ng/comido/cgi_bin/is_staff.php?username=" + to;
                var requ = WebRequest.Create(url);
                requ.Timeout = 80000;
                var response = requ.GetResponse();
                var stream = response.GetResponseStream();
                var compSpecs = new XmlDocument();
                compSpecs.Load(stream);
                //Load the data from the file into the XmlDocument (CompSpecs) //
                var nodeList = compSpecs.GetElementsByTagName("status");
                // Create a list of the nodes in the xml file //
                label2.Text = nodeList[0].ParentNode.ChildNodes[0].InnerText;
                if (label2.Text.Trim() == "1")
                {
                    Logininfo.username = textBox1.Text;
                    var homepage = new Form1();
                    homepage.Show();
                    Hide();
                }
                else
                {
                    InformationBox.Show("Sorry Your Login ID Is Incorrect", "Comida Administrator",
                        InformationBoxButtons.OK, InformationBoxIcon.Error);
                    label2.Text = "";
                    textBox1.Clear();
                    ActiveControl = textBox1;
                    textBox1.Focus();
                }
            }
            catch (Exception exec)
            {
                InformationBox.Show(exec.Message + " " + "due to poor internet connection", "Comida Administrator",
                    InformationBoxButtons.OK, InformationBoxIcon.Error);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                Loadxml();
            }
        }

        internal class Logininfo
        {
            public static string username { get; set; }
        }
    }
}