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
    public partial class Activity : Form
    {
        public Activity()
        {
            InitializeComponent();
        }

        public class MyWorkerClass
        {
            public string Access;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            if ((textBox1.Text != "") && (!System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "^[0-9]*$"))) return;
            var obj = new MyWorkerClass();
            bunifuTransition1.ShowSync(transactionSuccessful1);
            if (backgroundWorker1.IsBusy) return;
            backgroundWorker1.RunWorkerAsync(obj);
            textBox1.Enabled = false;
            textBox1.Focus();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
           
        }

        private void bunifuImageButton6_Click(object sender, EventArgs e)
        {
            switch (panel1.Visible)
            {
                case false:
                    bunifuImageButton6.Image = Properties.Resources.Cancel;
                    bunifuTransition1.ShowSync(panel1);
                    break;
                case true:
                    bunifuImageButton6.Image = Properties.Resources.Menu;
                    bunifuTransition1.HideSync(panel1);
                    break;
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
            if (obj.Access != "")
            {
                bunifuTransition1.HideSync(transactionSuccessful1);
                PrintMethod();
                textBox1.Clear();
                ActiveControl = textBox1;
                textBox1.Focus();
            }
            else if (obj.Access == "")
            {
                textBox1.Clear();
                bunifuTransition1.HideSync(transactionSuccessful1);

                ActiveControl = textBox1;
                textBox1.Focus();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.Enabled = true;
        }
        private void PrintMethod()
        {
            try
            {
                var pd = new PrintDocument();
                pd.PrintPage += pd_PrintPage;
                pd.PrintController = new StandardPrintController();
               pd.Print();
            }
            catch (Exception excep)
            {
                InformationBox.Show(excep.Message);
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
                const int space = 50;
                var title = Application.StartupPath + "\\imagenow.png";
                var g = e.Graphics;
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
                g.DrawString("Amount: ", fBody1, sb, 10, space + 150);
                g.DrawString(amounts, fBody1, sb, 70, space + 150);
                g.DrawString("Terminal:", fBody1, sb, 10, space + 180);
                g.DrawString(drawn, fBody1, sb, 80, space + 180);
                g.DrawString("---------------------------", fBody1, sb, 10, space + 210);
                g.DrawString("**Powered By COMIDA**", fBody1, sb, 10, space + 240);
                g.DrawString(" softdev@run.edu.ng", fBody1, sb, 10, space + 270);
            }
            catch (Exception excep)
            {
                InformationBox.Show(excep.Message);
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

                var nodeList = compSpecs.SelectNodes("response");
                var i = 0;
                if (nodeList != null)
                {
                    var parentNode = nodeList[0].FirstChild;
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

        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            var question = new HomeAlert();
            question.ShowDialog();
        }
    }
}