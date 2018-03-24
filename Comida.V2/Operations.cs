using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using InfoBox;

namespace Comida.V2
{
    public partial class Operations : Form
    {
        public Operations()
        {
            InitializeComponent();
        }

        public class MyWorkerClass
        {
            public string Access;
        }

        private void Operations_Load(object sender, EventArgs e)
        {

        }

        private void Operations_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            var question = new HomeAlert();
            question.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                const string type = "Cash";
                var drawn = FirstPage.Logininfo.Username;
                var amounts = textBox1.Text.Trim();
                var cafname = CafSelect.CafeteriaInfo.Cafname;
                //string Customer = "Manna Palace";
                const int space = 50;
                var title = Application.StartupPath + "\\capture.png";
                var g = e.Graphics;
                //g.DrawRectangle(Pens.Black, 5, 5, 378, 445);
                g.DrawImage(Image.FromFile(title), 30, 5);
                var fBody = new Font("Lucida Console", 9, FontStyle.Regular);
                var fBody1 = new Font("Lucida Console", 9, FontStyle.Regular);
                var rs = new Font("Stencil", 10, FontStyle.Bold);
                var fTType = new Font("", 150, FontStyle.Bold);
                var sb = new SolidBrush(Color.Black);
                g.DrawString("", fBody1, sb, 10, 100);
                g.DrawString("*" + cafname + "*", fBody1, sb, 10, space);
                g.DrawString("---------------------------", fBody1, sb, 10, space + 30);
                g.DrawString("Date: ", fBody1, sb, 10, space + 60);
                g.DrawString(DateTime.Now.ToShortDateString(), fBody1, sb, 50, space + 60);
                g.DrawString("Time: ", fBody1, sb, 10, space + 90);
                g.DrawString(DateTime.Now.ToShortTimeString(), fBody1, sb, 50, space + 90);
                g.DrawString("Type: ", fBody1, sb, 10, space + 120);
                g.DrawString(type, fBody1, sb, 50, space + 120);
                //g.DrawString("DriverName:", fBody, sb, 10, SPACE+120);
                //g.DrawString(txtDriveName.Text, fBody1, sb, 153, SPACE + 120);
                g.DrawString("Amount: ", fBody1, sb, 10, space + 150);
                g.DrawString(amounts, fBody1, sb, 70, space + 150);
                g.DrawString("Terminal:", fBody1, sb, 10, space + 180);
                g.DrawString(drawn, fBody1, sb, 80, space + 180);
                g.DrawString("---------------------------", fBody1, sb, 10, space + 210);
                g.DrawString("**Powered By COMIDA**", fBody1, sb, 10, space + 240);
                //  g.DrawString(TType, fTType, sb, 230, 120);
                g.DrawString("softdev@run.edu.ng", fBody1, sb, 10, space + 270);
            }
            catch (Exception excep)
            {
                InformationBox.Show(excep.Message, "Comida", InformationBoxButtons.OK,
                    InformationBoxIcon.Error);
                InformationBox.Show(excep.StackTrace);
            }
        }

        private void bunifuImageButton6_Click(object sender, EventArgs e)
        {
            switch (panel1.Visible)
            {
                case false:
                    bunifuImageButton6.Image = Properties.Resources.close;
                    bunifuTransition1.ShowSync(panel1);
                    break;
                case true:
                    bunifuImageButton6.Image = Properties.Resources.menuicon;
                    bunifuTransition1.HideSync(panel1);
                    break;
            }
        }

        private void Loadtransaction(MyWorkerClass obj)
        {
            try
            {
                var attendant = FirstPage.Logininfo.Username;
                var price = textBox1.Text.Trim();
                var cafid = CafSelect.CafeteriaInfo.CafId;
                const string matric = "null";
                var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                const string type = "cash";
                const string pin = "null";
                var url = "http://exeat.run.edu.ng/comido/cgi_bin/insert_transaction.php?matric=" + matric +
                          "&pin=" + pin + "&attendant=" + attendant + "&amount=" + price + "&datetime=" +
                          datetime + "&type=" + type + "&CafID=" + cafid + "";
                var requ = WebRequest.Create(url);
                requ.Timeout = 80000;
                var response = requ.GetResponse();
                var stream = response.GetResponseStream();
                var compSpecs = new XmlDocument();
                compSpecs.Load(stream ?? throw new InvalidOperationException());

                var nodeList = compSpecs.SelectNodes("response/status");
                var i = 0;
                if (nodeList != null)
                {
                    var parentNode = nodeList[0].ParentNode;
                    var xmlNode = parentNode?.InnerText.Trim();
                    if (xmlNode != null) obj.Access = xmlNode;
                }
                backgroundWorker1.ReportProgress(i, obj);
                i++;
                Thread.Sleep(10);
            }
            catch (Exception exec)
            {
                bunifuTransition1.HideSync(transactionSuccessful1);
                var interneterror = new InternetError();
                interneterror.ShowDialog();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var obj = (MyWorkerClass)e.Argument;
            Loadtransaction(obj);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending) return;
            var obj = (MyWorkerClass)e.UserState;
            switch (obj.Access)
            {
                case "1":
                    //textBox1.Clear();
                    bunifuTransition1.HideSync(transactionSuccessful1);
                    PrintMethod();
                    textBox1.Clear();
                    ActiveControl = textBox1;
                    textBox1.Focus();


                    break;
                case "0":
                    textBox1.Clear();
                   bunifuTransition1.HideSync(transactionSuccessful1);
                    
                    ActiveControl = textBox1;
                    textBox1.Focus();
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.Enabled = true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter) return;
            //if (textBox1.Text == "") return;
            //var obj = new MyWorkerClass();

            //if (backgroundWorker1.IsBusy) return;
            //backgroundWorker1.RunWorkerAsync(obj);
            //textBox1.Enabled = false;
        }

        private void Operations_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter) return;
            //if (textBox1.Text == "") return;
            //var obj = new MyWorkerClass();

            //if (backgroundWorker1.IsBusy) return;
            //backgroundWorker1.RunWorkerAsync(obj);
            //textBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var obj = new MyWorkerClass();

            if (backgroundWorker1.IsBusy) return;
            backgroundWorker1.RunWorkerAsync(obj);
            textBox1.Enabled = false;
        }
      }
}
