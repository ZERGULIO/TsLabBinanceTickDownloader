using System.Net;
using System.Xml;

namespace TsLabBinanceTickDownloader
{
    public partial class Form1 : Form
    {
        public string url_futures = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/futures/um/daily/aggTrades/";
        public string url_spot = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/spot/daily/aggTrades/";
        List<string> list_arhives = new List<string>();
        public string coin_type = "";
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
            if (type == "list_arhives")
            {
                list_arhives.Clear();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(data);

                XmlNodeList nodeList = xDoc.GetElementsByTagName("Contents");
                foreach (XmlElement element in nodeList)
                {
                    string text = element.GetElementsByTagName("Key")[0].InnerText;
                    if(text.IndexOf("CHECKSUM") == -1)
                    {
                        list_arhives.Add(text);
                    }
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
            button1.Enabled = false;
            coin_type = "futures";
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
            button1.Enabled = false;
            coin_type = "spot";
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
            button1.Text = "Загрузка";
            button1.Enabled = false;
            coin_setup(listBox1.Text);
            button1.Enabled = true;
            label_coin_selected.Text = listBox1.Text;
            button1.Text = "Выбрать";
        }
        public void coin_setup(string coin_name)
        {
            string url_fut_arh = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/futures/um/daily/aggTrades/";
            string url_spot_arh = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/spot/daily/aggTrades/";
            string url = "";
            if (coin_type == "futures")
            {
                url = url_fut_arh + coin_name + "/";
            }
            if (coin_type == "spot")
            {
                url = url_spot_arh + coin_name + "/";
            }

            xml_get_info(Get_Html_Code(url), "list_arhives");

            label1.Text = list_arhives.Count.ToString();

        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}