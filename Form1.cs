using System.Diagnostics;
using System.Net;
using System.Xml;
using System.IO.Compression;
using System.Text;

namespace TsLabBinanceTickDownloader
{
    public partial class Form1 : Form
    {
        public string url_futures = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/futures/um/daily/aggTrades/";
        public string url_spot = "https://s3-ap-northeast-1.amazonaws.com/data.binance.vision?delimiter=/&prefix=data/spot/daily/aggTrades/";
        List<string> list_arhives = new List<string>();
        public string coin_type = "";
        public string Core_Folder = Directory.GetCurrentDirectory() + @"\Core\";
        public string Temp_Folder = Directory.GetCurrentDirectory() + @"\Core\Temp_Folder\";
        static List<string> new_list = new List<string>();
        string version = "1.1";

        public Form1()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            InitializeComponent();
            Folder_Work();
            this.Text = "TSBDT-v" + version;
        }
        public static List<string> arr_coin = new List<string>() { };
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Выберите - Тип рынка - в верхнем левом углу");
        }
        public string Get_Html_Code(string url)
        {
            string response = "";
            using (var webClient = new WebClient())
            {
                webClient.Proxy = null;
                // Выполняем запрос по адресу и получаем ответ в виде строки
                try
                {
                    response = webClient.DownloadString(url);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
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
                        list_arhives.Add("https://data.binance.vision/" + text);
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
        public  void listbupdate()
        {
            listBox1.Items.Clear();
            foreach (var item in arr_coin)
            {

                listBox1.Items.Add(item);

            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            logbox.Clear();
            Log("Выбран тип рынка - Futures");
            coin_type = "futures";
            listBox1.Enabled = false;
            listBox1.BackColor = Color.LightGray;
            textBox1.Enabled = false;
            xml_get_info(Get_Html_Code(url_futures), "futures");
            listBox1.Enabled = true;
            listBox1.BackColor = Color.White;
            textBox1.Enabled = true;
            listbupdate();
            Log("Загружено инструментов: " + arr_coin.Count.ToString());
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            logbox.Clear();
            Log("Выбран тип рынка - Spot");
            coin_type = "spot";
            listBox1.Enabled = false;
            listBox1.BackColor = Color.LightGray;
            textBox1.Enabled = false;
            xml_get_info(Get_Html_Code(url_spot), "spot");
            listBox1.Enabled = true;
            listBox1.BackColor = Color.White;
            textBox1.Enabled = true;
            listbupdate();
            Log("Загружено инструментов: " + arr_coin.Count.ToString());
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            button2.Enabled = false;
            button1.Text = "Загрузка";
            button1.BackColor = Color.IndianRed;
            button1.Enabled = false;
            coin_setup(listBox1.Text);
            button1.Enabled = true;
            label_coin_selected.Text = listBox1.Text;
            button1.Text = "Выбрать";
            button1.BackColor = Color.Gainsboro;
        }
        public  void coin_setup(string coin_name) // Информация об Инструменте кол-во ит.д
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

            label1.Text = list_arhives.Count.ToString() + " дней";
            numericUpDown1.Maximum = list_arhives.Count();

            foreach (var item in list_arhives)
            {
                //Вывод массива ссылок на архивы
            }
            Download_Setup(coin_name);

        }
        List<string> download_url = new List<string>();
        public void Download_Setup(string coin_name) // Генерация массива ссылок для последующей скачки их
        {
            string date(int i)
            {
                DateTime dateTime = DateTime.Today.Date;
                return dateTime.AddDays(-i).ToString("yyyy-MM-dd");

            }
            numericUpDown1.Enabled = true;
            button2.Enabled = true;
            if(coin_type == "futures")
            {
                download_url.Clear();
                for (int i = 1; i < 502; i++)
                {
                    download_url.Add("https://data.binance.vision/data/futures/um/daily/aggTrades/" + coin_name + "/" + coin_name + "-aggTrades-" + date(i) + ".zip");
                    //Log(download_url[i-1]);
                }
            }
            if (coin_type == "spot")
            {
                download_url.Clear();
                for (int i = 1; i < 502; i++)
                {
                    download_url.Add("https://data.binance.vision/data/spot/daily/aggTrades/" + coin_name + "/" + coin_name + "-aggTrades-" + date(i) + ".zip");
                    //Log(download_url[i - 1]);
                }
            }
            
        }
        public async Task Download_File(int max)
        {
            int unlock = 0;
            int thread = 4;
            block();
            int index = 0;
            string get_url()
            {

                 string url = download_url[index];
                 index++;
                 return url;

            }
            var tasks = new List<Task>();
            int dow = 0;
            for (var i = 0; i < thread; i++)
            {
                tasks.Add(Task.Run(() => {
                    
                    while (true) 
                    {
                        dow++;
                        using (var client = new WebClient())
                        {
                            WebRequest.DefaultWebProxy = null;
                            try
                            {
                                string url = get_url();
                                if(dow > max) { break; }
                                string filename = Path.GetFileName(url);
                                client.DownloadFile(url, Temp_Folder + filename);
                                Log("Загрузка + " + filename);
                            }
                            catch (Exception e)
                            {
                                dow--;
                                continue;
                            }
                        }
                    }
                    unlock++;
                    if(unlock >= thread)
                    {
                        this.unlock();
                        Log("Завершили загрузку архивов");
                        Extract_Arhives();
                    }
                }));
            }
            //Task.WaitAll(tasks.ToArray());
            
        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        public void Log(string text) // Вывод в Log
        {
            logbox.AppendText(text + Environment.NewLine);
        }
        private void button3_Click(object sender, EventArgs e) // Папка Tslab с кешами в AppData
        {
            Process.Start("explorer", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TSLab\TSLab 2.0");
        }
        private void button4_Click(object sender, EventArgs e) // Папка с Тиками
        {
            Process.Start("explorer", Directory.GetCurrentDirectory() + @"\Core\_result");
        }
        private void button2_Click(object sender, EventArgs e) // Кнопка начала - Загрузки
        {
            if (Directory.Exists(Temp_Folder))
            {
                Directory.Delete(Temp_Folder, true);
                Directory.CreateDirectory(Temp_Folder);
            }
            else
            {
                Directory.CreateDirectory(Temp_Folder);
            }
            Log("Запуск загрузки - Кол-во дней истории - " + numericUpDown1.Value);
            Download_File((int)numericUpDown1.Value);
        }
        public void block()
        {
            groupBox2.Enabled = false;
            button1.Enabled = false;
            listBox1.Enabled = false;
            textBox1.Enabled = false;
            groupBox1.Enabled = false;
        }
        public void unlock()
        {
            groupBox2.Enabled = true;
            button1.Enabled = true;
            listBox1.Enabled = true;
            textBox1.Enabled = true;
            groupBox1.Enabled = true;
        }
        public void Folder_Work() // При запуске проверяет наличие нужных папок для работы
        {
            try
            {
                if (Directory.Exists(Core_Folder))
                {
                    if (Directory.Exists(Core_Folder + "_result"))
                    {

                    }
                    else
                    {
                        Directory.CreateDirectory(Core_Folder + "_result");
                    }
                    if (Directory.Exists(Temp_Folder))
                    {
                        Directory.Delete(Temp_Folder, true);
                        Directory.CreateDirectory(Temp_Folder);
                    }
                    else
                    {
                        Directory.CreateDirectory(Temp_Folder);
                    }
                }
                else
                {
                    Log("Прога Работать не будет. ВЫ СЛОМАЛИ ЕЁ.... СКАЧАЙТЕ НОРМАЛЬНУЮ СБОРКУ С GITHUBA....");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void Extract_Arhives()
        {
            string[] arr_file = Directory.GetFiles(Temp_Folder);
            foreach (var item in arr_file)
            {

                Log("Разархивирование - " + Path.GetFileName(item));
                ZipFile.ExtractToDirectory(item, Temp_Folder);
                File.Delete(item);
            }
            string[] arr_csv = Directory.GetFiles(Temp_Folder);
            foreach (var item in arr_csv)
            {
                Read_CSV(item, Path.GetFileName(item));
            }
            StartCmdBatch();
        }
        public void Read_CSV(string path, string namefile) // Чтения CSV файлов
        {

            // чтение из файла
            try
            {
                string line;
                new_list.Clear();
                new_list.Add("<TRADENO>,<LAST>,<VOL>,<DATE>,<TIME>,<OPER>");
                using (StreamReader f = new StreamReader(path))
                {
                    while ((line = f.ReadLine()) != null)
                    {
                        Converting(ref line);
                    }
                }
                //Console.WriteLine("Заполнили массив");
                Write_File(new_list, namefile);  // Записываем в файл готовые тики
                Log("Конвертирование - " + namefile);

            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

        }
        public void Converting(ref string line) // Конвертирование данных в нужный формат тиков для TsLab
        {
            string TRADENO, LAST, VOL, DATE, TIME, OPER;

            string[] l = line.Split(',');
            TRADENO = l[0];
            LAST = l[1];
            VOL = l[2];
            DATE = Get_Date(Convert.ToDouble(l[5].Remove(l[5].Length - 3)));
            TIME = Get_Time(Convert.ToDouble(l[5].Remove(l[5].Length - 3)));
            if (l[6] == "true") { OPER = "B"; }
            else { OPER = "S"; }
            string good = TRADENO + "," + LAST + "," + VOL + "," + DATE + "," + TIME + "," + OPER;

            new_list.Add(good);

            static string Get_Date(double unixTimeStamp)
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(unixTimeStamp);
                return dateTime.ToString("yyyyMMdd");
            }
            static string Get_Time(double unixTimeStamp)
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(unixTimeStamp);
                return dateTime.ToString("HHmmss");
            }
        }
        public void Write_File(List<string> new_list, string namefile)
        {
            using (StreamWriter w = new StreamWriter(Temp_Folder + namefile.Replace(".csv", ".txt"), false))
            {

                foreach (var item in new_list)
                {
                    w.WriteLine(item);
                }
                File.Delete(Temp_Folder + "/" + namefile);

            }
        }
        public void StartCmdBatch()
        {
            string[] arr_file = Directory.GetFiles(Temp_Folder);
            using (StreamWriter w = new StreamWriter(Core_Folder + "run.bat", false, Encoding.Default))
            {
                w.WriteLine("cd Core");
                foreach (var item in arr_file)
                {
                    w.WriteLine("ConvertTicks.exe " + label_coin_selected.Text + " \"" + item + "\" \\c");
                }
            }
            string parameters = String.Format("/k \"{0}\"", Core_Folder + "run.bat");
            System.Diagnostics.Process.Start("cmd", parameters);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/ZERGULIO/TsLabBinanceTickDownloader");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}