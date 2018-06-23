using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using InfoBox;

namespace Comida_Upgrade
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == false)
            {
                panel1.Visible = true;
            }
            else if (panel1.Visible)
            {
                panel1.Visible = false;
                ActiveControl = textBox1;
                textBox1.Focus();
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                var Type = "Cash";
                var drawn = Firstpage.Logininfo.username;
                var amounts = textBox1.Text.Trim();
                var cafname = CafSelect.CafeteriaInfo.Cafname;
                //string Customer = "Manna Palace";
                var SPACE = 50;
                var title = Application.StartupPath + "\\imagenow.png";
                var g = e.Graphics;
                //g.DrawRectangle(Pens.Black, 5, 5, 378, 445);
                g.DrawImage(Image.FromFile(title), 30, 5);
                var fBody = new Font("Lucida Console", 9, FontStyle.Regular);
                var fBody1 = new Font("Lucida Console", 9, FontStyle.Regular);
                var rs = new Font("Stencil", 10, FontStyle.Bold);
                var fTType = new Font("", 150, FontStyle.Bold);
                var sb = new SolidBrush(Color.Black);
                g.DrawString("", fBody1, sb, 10, 100);
                g.DrawString("*" + cafname + "*", fBody1, sb, 10, SPACE);
                g.DrawString("---------------------------", fBody1, sb, 10, SPACE + 30);
                g.DrawString("Date: ", fBody1, sb, 10, SPACE + 60);
                g.DrawString(DateTime.Now.ToShortDateString(), fBody1, sb, 50, SPACE + 60);
                g.DrawString("Time: ", fBody1, sb, 10, SPACE + 90);
                g.DrawString(DateTime.Now.ToShortTimeString(), fBody1, sb, 50, SPACE + 90);
                g.DrawString("Type: ", fBody1, sb, 10, SPACE + 120);
                g.DrawString(Type, fBody1, sb, 50, SPACE + 120);
                //g.DrawString("DriverName:", fBody, sb, 10, SPACE+120);
                //g.DrawString(txtDriveName.Text, fBody1, sb, 153, SPACE + 120);
                g.DrawString("Amount: ", fBody1, sb, 10, SPACE + 150);
                g.DrawString(amounts, fBody1, sb, 70, SPACE + 150);
                g.DrawString("Terminal:", fBody1, sb, 10, SPACE + 180);
                g.DrawString(drawn, fBody1, sb, 80, SPACE + 180);
                g.DrawString("---------------------------", fBody1, sb, 10, SPACE + 210);
                g.DrawString("**Powered By COMIDA**", fBody1, sb, 10, SPACE + 240);
                //  g.DrawString(TType, fTType, sb, 230, 120);
                g.DrawString("Helpline:+2348140806886", fBody1, sb, 10, SPACE + 270);
            }
            catch (Exception excep)
            {
                InformationBox.Show(excep.Message, "Comida Administrator", InformationBoxButtons.OK,
                    InformationBoxIcon.Error);
                InformationBox.Show(excep.StackTrace);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char) Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = @"null";
            label2.Text = @"Welcome," + Firstpage.Logininfo.username;
            ActiveControl = textBox1;
            textBox1.Focus();
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            if (InformationBox.Show("Do You Really Want To Logout?", "Comida Administrator", InformationBoxButtons.YesNo,
                    InformationBoxIcon.Question).ToString() == "No")
            {
                ActiveControl = textBox1;
                textBox1.Focus();
            }
            else
            {
                var firstpage = new Firstpage();
                firstpage.Show();
                Hide();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ActiveControl = textBox1;
            textBox1.Focus();
        }

        private void PrintMethod()
        {
            try
            {
                var pd = new PrintDocument();
                //PaperSize ps = new PaperSize("", 261, 445);
                pd.PrintPage += pd_PrintPage;
                pd.PrintController = new StandardPrintController();
                // pd.DefaultPageSettings.Margins.Left = 0;
                //  pd.DefaultPageSettings.Margins.Right = 0;
                //  pd.DefaultPageSettings.Margins.Top = 0;
                //  pd.DefaultPageSettings.Margins.Bottom = 0;
                //pd.DefaultPageSettings.PaperSize = ps;
                pd.Print();
            }
            catch (Exception excep)
            {
                InformationBox.Show(excep.Message, "Comida Administrator", InformationBoxButtons.OK,
                    InformationBoxIcon.Error);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "")
                {
                    InformationBox.Show("Please Enter The Amount You Would Like To Purchase", "Comida Administrator",
                        InformationBoxButtons.OK, InformationBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        var attendant = Firstpage.Logininfo.username;
                        var price = textBox1.Text.Trim();
                        var cafid = CafSelect.CafeteriaInfo.CafId;
                        const string matric = "null";
                        //string matric = "CMP/14/5788";
                        //string attendant = "A4572H04240016";
                        var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        const string type = "cash";
                        const string pin = "null";
                        var url = "http://localhost/comido/cgi_bin/insert_transaction.php?matric=" + matric +
                                  "&pin=" + pin + "&attendant=" + attendant + "&amount=" + price + "&datetime=" +
                                  datetime + "&type=" + type + "&CafID="+ cafid +"";
                        var requ = WebRequest.Create(url);
                        requ.Timeout = 80000;
                        var response = requ.GetResponse();
                        var stream = response.GetResponseStream();
                        var compSpecs = new XmlDocument();
                        compSpecs.Load(stream);

                        //Load the data from the file into the XmlDocument (CompSpecs) //
                        XmlNodeList nodeList = compSpecs.GetElementsByTagName("response");
                        var status = nodeList[0].ParentNode.ChildNodes[0].InnerText;
                        var typ = nodeList[0].ChildNodes[1].InnerText;
                        if (nodeList[0].ChildNodes[1].InnerText == "")
                        {
                            var error = status;
                            InformationBox.Show("Error Message: " + error, "Comida Administrator",
                                InformationBoxButtons.OK, InformationBoxIcon.Information);
                            var homepage = new Form1();
                            homepage.Show();
                            Hide();
                        }
                        else if (nodeList[0].ParentNode.ChildNodes[0].InnerText != "" &&
                                 nodeList[0].ChildNodes[1].InnerText.ToString() != "")
                        {
                            PrintMethod();
                            textBox1.Clear();
                            ActiveControl = textBox1;
                            textBox1.Focus();
                        }
                    }
                    catch (Exception exec)
                    {
                        InformationBox.Show(exec.Message + " " + "due to poor internet connection",
                            "Comida Administrator", InformationBoxButtons.OK, InformationBoxIcon.Error);
                        var homepage = new Form1();
                        homepage.Show();
                        Hide();
                    }
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "")
                {
                    InformationBox.Show(@"Please Enter The Amount You Would Like To Purchase", "Comida Administrator",
                        InformationBoxButtons.OK, InformationBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        var attendant = Firstpage.Logininfo.username;
                        var price = textBox1.Text.Trim();
                        var cafid = CafSelect.CafeteriaInfo.CafId;
                        const string matric = "null";
                        //string matric = "CMP/14/5788";
                        //string attendant = "A4572H04240016";
                        var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        const string type = "cash";
                        const string pin = "null";
                        var url = "http://localhost/comido/cgi_bin/insert_transaction.php?matric=" + matric +
                                  "&pin=" + pin + "&attendant=" + attendant + "&amount=" + price + "&datetime=" +
                                  datetime + "&type=" + type + "&CafID="+ cafid +"";
                        var requ = WebRequest.Create(url);
                        requ.Timeout = 80000;
                        var response = requ.GetResponse();
                        var stream = response.GetResponseStream();
                        var compSpecs = new XmlDocument();
                        compSpecs.Load(stream);

                        //Load the data from the file into the XmlDocument (CompSpecs) //
                        var nodeList = compSpecs.GetElementsByTagName("response");
                        var status = nodeList[0].ParentNode.ChildNodes[0].InnerText;
                        var typ = nodeList[0].ChildNodes[1].InnerText;
                        if (nodeList[0].ChildNodes[1].InnerText == "")
                        {
                            var error = status;
                            InformationBox.Show("Error Message: " + error, "Comida Administrator",
                                InformationBoxButtons.OK, InformationBoxIcon.Information);
                            var homepage = new Form1();
                            homepage.Show();
                            Hide();
                        }
                        else if (nodeList[0].ParentNode.ChildNodes[0].InnerText != "" &&
                                 nodeList[0].ChildNodes[1].InnerText != "")
                        {
                            PrintMethod();
                            textBox1.Clear();
                            ActiveControl = textBox1;
                            textBox1.Focus();
                        }
                    }
                    catch (Exception exec)
                    {
                        InformationBox.Show(exec.Message + " " + "due to poor internet connection",
                            "Comida Administrator", InformationBoxButtons.OK, InformationBoxIcon.Error);
                        var homepage = new Form1();
                        homepage.Show();
                        Hide();
                    }
                }
            }
        }
    }
}
