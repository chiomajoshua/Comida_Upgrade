using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using InfoBox;

namespace Comida.V2
{
    public partial class CafSelect : Form
    {
        private string _h;
        private string _cafid;

        //private string _access;

        ////public int FormId = 1;
        public static class ControlId
        {
            public static int FormId { get; set; }
        }

        internal class CafeteriaInfo
        {
            public static string CafId { get; set; }
            public static string Cafname { get; set; }
        }

        public class MyWorkerClass
        {
            public XmlNodeList Cafetype;
        }

        public CafSelect()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            var obj = new MyWorkerClass();

            if (backgroundWorker1.IsBusy) return;
            backgroundWorker1.RunWorkerAsync(obj);

            bunifuFlatButton1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(10);
            //this code assign the timer to the label
            _h = progressBar1.Value.ToString();
            //this code stops the timer by 100% and starts the next form.
            if (_h != "100") return;
            timer1.Stop();
            bunifuTransition1.HideSync(panel2);
            progressBar1.Value = 0;
        }

        private void FetchCafeteriaName(MyWorkerClass obj)
        {
            try
            {
               const string url = "http://exeat.run.edu.ng/comido/cgi_bin/fetch_cafeteria_details.php";
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Timeout = 80000;
                request.ContentType = "application/xml";
                var response = (HttpWebResponse) request.GetResponse();
                var datastream = response.GetResponseStream();
                var reader = XmlReader.Create(datastream ?? throw new InvalidOperationException());
                var doc = new XmlDocument();
                doc.Load(reader);
                var nodeList = doc.SelectNodes("response/status");
                if (nodeList != null) obj.Cafetype = nodeList;
                const int i = 0;
                backgroundWorker1.ReportProgress(i, obj);
            }
            catch (Exception ce)
            {
                var interneterror = new InternetError();
                interneterror.ShowDialog();
                // to start new instance of application
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                //to turn off current app
                Close();
            }
        }

        private void bunifuCards1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var obj = (MyWorkerClass) e.Argument;
            FetchCafeteriaName(obj);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending) return;
            var obj = (MyWorkerClass) e.UserState;
            foreach (XmlNode node in obj.Cafetype)
                if (!comboBox1.Items.Contains(node))
                {
                    comboBox1.Items.Add(node.SelectSingleNode("Name")?.InnerText ??
                                        throw new InvalidOperationException());
                    comboBox1.SelectedIndex = e.ProgressPercentage;
                }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bunifuFlatButton1.Enabled = true;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
           var question = new HomeAlert();
            question.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                const string url = "http://exeat.run.edu.ng/comido/cgi_bin/fetch_cafeteria_details.php";
                var requ = WebRequest.Create(url);
                requ.Timeout = 80000;
                var responses = requ.GetResponse();
                var stream = responses.GetResponseStream();
                var xmlDoc = XDocument.Load(stream);
                var response = from status in xmlDoc.Descendants("status")
                    where status.Element("Name")?.Value == comboBox1.SelectedItem.ToString()
                    select new
                    {
                        Points = status.Element("CafID")?.Value
                    };
                foreach (var status in response)
                {
                    //timer1.Interval = 10;
                    timer1.Start();
                    _cafid = status.Points;
                    bunifuCustomLabel2.Text = comboBox1.Text + @" Selected";
                    panel2.Top = 40;
                    panel2.Left = Screen.PrimaryScreen.Bounds.Width - 250;
                    var m = (panel2.Size.Width - bunifuCustomLabel2.Size.Width) / 2;
                    bunifuCustomLabel2.Location = new Point(m, bunifuCustomLabel2.Location.Y);
                    bunifuTransition1.ShowSync(panel2);
                }
            }
            catch (Exception ce)
            {
                var interneterror = new InternetError();
                interneterror.ShowDialog();
            }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            CafeteriaInfo.CafId = _cafid;
            CafeteriaInfo.Cafname = comboBox1.Text;
            var loginpage = new FirstPage();
            loginpage.Show();
            Hide();
        }

        private void CafSelect_Paint(object sender, PaintEventArgs e)
        {
            comboBox1.SelectionLength = 0;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}