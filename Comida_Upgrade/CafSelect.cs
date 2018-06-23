using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using InfoBox;

namespace Comida_Upgrade
{
    public partial class CafSelect : Form
    {
        private string _cafid;
        private string _access;
        public CafSelect()
        {
            InitializeComponent();
        }
        internal class CafeteriaInfo
        {
            public static string CafId { get; set; }
            public static string Cafname { get; set; }
        }
        public class MyWorkerClass
        {
           // public string CafeId;
           public XmlNodeList Cafetype;
        }
        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                InformationBox.Show("Please Select Cafeteria Name", "Project Administrator", InformationBoxButtons.OK,
                    InformationBoxIcon.Information);
            }
            else
            {
                CafeteriaInfo.CafId = materialLabel2.Text.Trim();
                CafeteriaInfo.Cafname = comboBox1.Text;
                var login = new Firstpage();
                login.Show();
                Hide();
            }
        }
        private void FetchCafeteriaName(MyWorkerClass obj)
        {
           // comboBox1.Items.Clear();
            try
            {
                var bs = new BindingSource();
                const string url = "http://localhost/comido/cgi_bin/fetch_cafeteria_details.php";
               // var ds = new DataSet();
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 80000;
                request.ContentType = "application/xml";
                var response = (HttpWebResponse)request.GetResponse();
                var datastream = response.GetResponseStream();
                var reader = XmlReader.Create(datastream ?? throw new InvalidOperationException());
                var doc = new XmlDocument();
                doc.Load(reader);
                var nodeList = doc.SelectNodes("response/status");
                if (nodeList != null) obj.Cafetype = nodeList;
                //if (nodeList == null) return;
                int i = 0;
                //foreach (XmlNode node in nodeList)
                //    obj.Cafetype = node.SelectSingleNode("Name")?.InnerText;
                backgroundWorker1.ReportProgress(i, obj);
                i++;
                Thread.Sleep(10);
                ////if (!comboBox1.Items.Contains(node.SelectSingleNode("Name")?.InnerText))
                ////        comboBox1.Items.Add(node.SelectSingleNode("Name")?.InnerText ?? throw new InvalidOperationException());

            }
            catch (Exception ce)
            {
                InformationBox.Show(ce.Message);
            }
        }
        private void CafSelect_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            var obj = new MyWorkerClass();

            if (!backgroundWorker1.IsBusy)

            {
                backgroundWorker1.RunWorkerAsync(obj);

                materialFlatButton1.Enabled = false;

               // bunifuCustomLabel4.Text = @"Loading...";
            }
        }
        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            if (InformationBox.Show(@"You Are About To Exit The Application. Please Confirm", "Project Administrator",
                    InformationBoxButtons.YesNo, InformationBoxIcon.Question).ToString() == "No")
            {
            }
            else
            {
                Application.Exit();
                Application.ExitThread();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                const string url = "http://localhost/comido/cgi_bin/fetch_cafeteria_details.php";
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
                    _cafid = status.Points;
                    materialLabel2.Text = _cafid;
                }
            }
            catch (Exception ce)
            {
                InformationBox.Show(ce.Message);
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var obj = (MyWorkerClass)e.Argument;
            FetchCafeteriaName(obj);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (!backgroundWorker1.CancellationPending)
            {
                var obj = (MyWorkerClass)e.UserState;
               foreach (XmlNode node in obj.Cafetype)
                   
                    if (!comboBox1.Items.Contains(node))
                {
                  comboBox1.Items.Add(node.SelectSingleNode("Name")?.InnerText ?? throw new InvalidOperationException());
                        comboBox1.SelectedIndex = e.ProgressPercentage;
                }
                //lblEmployeeName.Text = obj.TableName;
                //lblTotalRows.Text = @"Rows processed : " + e.ProgressPercentage;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            materialFlatButton1.Enabled = true;
        }
    }
}
