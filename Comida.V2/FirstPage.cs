using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using InfoBox;

namespace Comida.V2
{
    public partial class FirstPage : Form
    {
        private string _h;
        public FirstPage()
        {
            InitializeComponent();
        }

        internal class Logininfo
        {
            public static string Username { get; set; }
        }

        public class MyWorkerClass
        {
            public string Access;
        }

        private void FirstPage_Load(object sender, EventArgs e)
        {
        }

        private void Loadxml(MyWorkerClass obj)
        {
            try
            {
               Logininfo.Username = textBox1.Text;
                var to = textBox1.Text.Trim();
                var url = "http://localhost/comido/cgi_bin/is_staff.php?username=" + to;
                var requ = WebRequest.Create(url);
                requ.Timeout = 80000;
                var response = requ.GetResponse();
                var stream = response.GetResponseStream();
                var compSpecs = new XmlDocument();
                compSpecs.Load(stream ?? throw new InvalidOperationException());
                //Load the data from the file into the XmlDocument (CompSpecs) //
                var nodeList = compSpecs.GetElementsByTagName("status");
                var i = 0;
                var parentNode = nodeList[0].ParentNode;
                var xmlNode = parentNode?.InnerText.Trim();
                if (xmlNode != null) obj.Access = xmlNode;
                backgroundWorker1.ReportProgress(i, obj);
                i++;
                Thread.Sleep(10);
                
            }
            catch (Exception exec)
            {
                bunifuTransition1.HideSync(homeButton1);
                var interneterror = new InternetError();
                interneterror.ShowDialog();
            }
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var obj = (MyWorkerClass) e.Argument;
            Loadxml(obj);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending) return;
            var obj = (MyWorkerClass) e.UserState;
            
            switch (obj.Access)
            {
                case "1":
                    textBox1.Clear();
                    bunifuTransition1.HideSync(homeButton1);
                    var success = new SuccessAlert();
                    success.ShowDialog();

                    var homepage = new Activity();
                    homepage.Show();
                    Hide();
                    break;
                case "0":
                    textBox1.Clear();
                    bunifuTransition1.HideSync(homeButton1);
                    InformationBox.Show("Sorry Your Login ID Is Incorrect", "Comida",
                        InformationBoxButtons.OK, InformationBoxIcon.Error);
                    
                    ActiveControl = textBox1;
                    textBox1.Focus();
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.Enabled = true;
        }
       
        private void bunifuImageButton1_Click_2(object sender, EventArgs e)
        {
            var question = new HomeAlert();
            question.ShowDialog();
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            if (textBox1.Text == "")
            {
                InformationBox.Show("Please Scan User Id", "Comida", InformationBoxButtons.OK,
                    InformationBoxIcon.Information);
                textBox1.Focus();
            }
            else
            {

               timer1.Start();
            }
        }
        private void FirstPage_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void FirstPage_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bunifuTransition1.ShowSync(homeButton1);
            progressBar1.Increment(10);
            //this code assign the timer to the label
            _h = progressBar1.Value.ToString();
            //this code stops the timer by 100% and starts the next form.
            if (_h != "100") return;
            timer1.Stop();
            progressBar1.Value = 0;
            var obj = new MyWorkerClass();

            if (backgroundWorker1.IsBusy) return;
            backgroundWorker1.RunWorkerAsync(obj);
            textBox1.Enabled = false;
        }
    }
}
