using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = "Naver Translation";
            label2.Text = "Kakao Translation";
            label1.Font = new Font("맑은고딕", 15, FontStyle.Bold);
            label2.Font = new Font("맑은고딕", 15, FontStyle.Bold);
            richTextBox1.Text = "Input text";
            button1.Text = "번역";
            richTextBox1.GotFocus += richTextBox1_GotFocus;
            richTextBox1.LostFocus += richTextBox1_LostFocus;
        }

        public string run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\lg\AppData\Local\Programs\Python\Python38-32\python.exe";
            start.Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd();
                    Console.WriteLine(stderr);
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(richTextBox1.Text == null || richTextBox1.Text == "")
            {
                richTextBox1.Text = "Input text please";
                return;
            }

            var source_lang = detectLang(richTextBox1.Text);
            string translated_text = translate(source_lang, richTextBox1.Text);
            System.Drawing.Font newFont = new Font("Verdana", 10f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 178, false);
            richTextBox2.Text = translated_text;
            richTextBox2.Font = new Font("맑은고딕", 20, FontStyle.Bold);

            string data = source_lang + " " + richTextBox1.Text;
            richTextBox3.Text = this.run_cmd("kakao_translation.py", data);
            richTextBox3.Font = new Font("맑은고딕", 20, FontStyle.Bold);
        }

        private void richTextBox1_GotFocus(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void richTextBox1_LostFocus(object sender, EventArgs e)
        {
            if(richTextBox1.Text != "Input Text" && richTextBox1.Text != "")
            {
                return;
            }
            richTextBox1.Text = "Input Text";
            richTextBox1.ForeColor = Color.Red;
            richTextBox1.Font = new Font("맑은고딕", 20, FontStyle.Bold);
        }

        static string translate(string source_lang, string input_text)
        {
            string url = "https://openapi.naver.com/v1/papago/n2mt";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "");
            request.Headers.Add("X-Naver-Client-Secret", "");
            request.Method = "POST";
            string query = input_text;
            string data = "";
            if (source_lang == "ko")
            {
                data = "source=" + source_lang + "&target=en&text=" + query;
            }
            else if (source_lang == "en")
            {
                data = "source=" + source_lang + "&target=ko&text=" + query;
            }
            byte[] byteDataParams = Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string translated_text = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();

            var trans_json = Newtonsoft.Json.Linq.JObject.Parse(translated_text);
            Console.WriteLine("Papago = " + (string)trans_json["message"]["result"]["translatedText"]);
            return (string)trans_json["message"]["result"]["translatedText"];
        }

        static string detectLang(string input_text)
        {
            string url = "https://openapi.naver.com/v1/papago/detectLangs";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "");
            request.Headers.Add("X-Naver-Client-Secret", "");
            request.Method = "POST";
            string query = input_text;
            byte[] byteDataParams = Encoding.UTF8.GetBytes("query=" + query);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string detectLangs = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();

            var trans_json = Newtonsoft.Json.Linq.JObject.Parse(detectLangs);
            return (string)trans_json["langCode"];
        }
    }
}
