using System.Net;
using System.Xml;

namespace TsLabBinanceTickDownloader
{
    public partial class Form1 : Form
    {
        public string url_futures = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/futures/um/daily/aggTrades/";
        public string url_spot = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/spot/daily/aggTrades/";
        List<string> list_arhives = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        public static List<string> arr_coin = new List<string>() { };
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public string Get_Html_Code(string url)
        {
            string response;
            using (var webClient = new WebClient())
            {
                // Выполняем запрос по адресу и получаем ответ в виде строки
                response = webClient.DownloadString(url);
            }
            return response;
        }
        public void xml_get_info(string data, string type)
        {
            if (type == "futures")
            {
                arr_coin.Clear();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(data);

                XmlNodeList nodeList = xDoc.GetElementsByTagName("CommonPrefixes");
                foreach (XmlElement element in nodeList)
                {
                    arr_coin.Add(element.GetElementsByTagName("Prefix")[0].InnerText.Replace("data/futures/um/daily/aggTrades/", "").Replace("/", ""));

                }
            }
            if (type == "spot")
            {
                arr_coin.Clear();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(data);

                XmlNodeList nodeList = xDoc.GetElementsByTagName("CommonPrefixes");
                foreach (XmlElement element in nodeList)
                {
                    arr_coin.Add(element.GetElementsByTagName("Prefix")[0].InnerText.Replace("data/spot/daily/aggTrades/", "").Replace("/", ""));

                }
            }
            if (type == "fut_coin")
            {
                list_arhives.Clear();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(data);

                XmlNodeList nodeList = xDoc.GetElementsByTagName("Contents");
                foreach (XmlElement element in nodeList)
                {
                    list_arhives.Add(element.GetElementsByTagName("Key")[0].InnerText);
                }
            }
            if (type == "spot_coin")
            {
                arr_coin.Clear();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(data);

                XmlNodeList nodeList = xDoc.GetElementsByTagName("CommonPrefixes");
                foreach (XmlElement element in nodeList)
                {

                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var item in arr_coin)
            {
                int i = item.IndexOf(textBox1.Text.ToUpper());
                if (i != -1)
                {
                    listBox1.Items.Add(item);
                }
            }
        }
        public void listbupdate()
        {
            listBox1.Items.Clear();
            foreach (var item in arr_coin)
            {

                listBox1.Items.Add(item);

            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = false;
            listBox1.BackColor = Color.LightGray;
            textBox1.Enabled = false;
            xml_get_info(Get_Html_Code(url_futures), "futures");
            listBox1.Enabled = true;
            listBox1.BackColor = Color.White;
            textBox1.Enabled = true;
            listbupdate();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = false;
            listBox1.BackColor = Color.LightGray;
            textBox1.Enabled = false;
            xml_get_info(Get_Html_Code(url_spot), "spot");
            listBox1.Enabled = true;
            listBox1.BackColor = Color.White;
            textBox1.Enabled = true;
            listbupdate();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label_coin_selected_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label_coin_selected.Text = listBox1.Text;
            coin_setup(listBox1.Text);
            button1.Enabled = true;
        }
        public void coin_setup(string coin_name)
        {
            xml_get_info(Get_Html_Code(url_futures + "/"), "fut_coin");

        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}