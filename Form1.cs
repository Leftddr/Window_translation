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
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Diagnostics;


namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {

        static int count = 0;
        static string filePath = "";
        private const uint SHOWWINDOW = 0x0040;
        private FormWindowState mLastState;
        Point point_for_richTextBox1;
        Point point_for_richTextBox2;
        Point point_for_richTextBox3;
        Point point_for_label1;
        Point point_for_label2;

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
        internal enum WNDSTATE : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }
        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
           int x, int y, int width, int height, uint uFlags);

        // Windows 의 Position 을 가져옴.

        void GetWindowPos(IntPtr hwnd, ref int ptrPhwnd, ref int ptrNhwnd, ref Point ptPoint, ref Size szSize, ref WNDSTATE intShowCmd)
        {
            WINDOWPLACEMENT wInf = new WINDOWPLACEMENT();
            wInf.length = System.Runtime.InteropServices.Marshal.SizeOf(wInf);
            GetWindowPlacement(hwnd, ref wInf);
            szSize = new Size(wInf.rcNormalPosition.Right - (wInf.rcNormalPosition.Left * 2),
                wInf.rcNormalPosition.Bottom - (wInf.rcNormalPosition.Top * 2));
            ptPoint = new Point(wInf.rcNormalPosition.Left, wInf.rcNormalPosition.Top);
        
        }

        Point getLocationPoint(Process me)
        {
            //Process me = Process.GetCurrentProcess(); // 현재 실행중인 Program 의 Process 를 가져온다.
            IntPtr hwnd = (IntPtr)me.MainWindowHandle; // me.ID 는 자신의 PID, me.MainWindowHandle 은 Spy++ 에서 확인할 수 있는 핸들 값이다.
            int ptrPhwnd = 0, ptrNhwnd = 0;
            Point ptPoint = new Point();
            Size szSize = new Size();
            WNDSTATE intShowCmd = 0;

            GetWindowPos(hwnd, ref ptrPhwnd, ref ptrNhwnd, ref ptPoint, ref szSize, ref intShowCmd);
            Console.WriteLine("X : {0}", ptPoint.X);
            Console.WriteLine("Y : {0}", ptPoint.Y);
            Console.WriteLine("Height : {0}", szSize.Height);
            Console.WriteLine("Width : {0}", szSize.Width);
            return ptPoint;
        }

        //부모 클래스의 메소드를 오버라이드 함으로써 사이즈 변화를 감지한다.
        protected override void OnClientSizeChanged(EventArgs e)
        {
            if(this.WindowState != mLastState)
            {
                mLastState = this.WindowState;
                OnWindowStateChanged(e);
            }
            else {
                Console.WriteLine("Detect resize: {0}, {1}", Size.Width, Size.Height);
                label1.Location = new Point(13, 23);
                label2.Location = new Point(13, Size.Height / 2 + 2);
                richTextBox2.Location = new Point(9, 41);
                richTextBox3.Location = new Point(9, Size.Height / 2 + 20);
                richTextBox2.Size = new Size(Size.Width / 2 - 20 , Size.Height / 3);
                richTextBox3.Size = new Size(Size.Width / 2 - 20 , Size.Height / 3);
            }
            base.OnClientSizeChanged(e);
        }

        protected void OnWindowStateChanged(EventArgs e)
        {
            Console.WriteLine("Window State: {0}", WindowState);

        }

        public Form1()
        {
            //모든 form을 초기화 하는 함수 이다.
            InitializeComponent();

            point_for_richTextBox1 = richTextBox1.Location;
            point_for_richTextBox2 = richTextBox2.Location;
            point_for_richTextBox3 = richTextBox3.Location;
            point_for_label1 = label1.Location;
            point_for_label2 = label2.Location;

            Console.WriteLine(point_for_richTextBox2.X);
            Console.WriteLine(point_for_richTextBox2.Y);
            Console.WriteLine(point_for_label1.X);

            mLastState = this.WindowState;
            Process[] allProc = Process.GetProcesses();
            foreach(Process p in allProc)
            {
                if(p.ToString() == "System.Diagnostics.Process (WindowsFormsApp5)")
                {
                    getLocationPoint(p);
                    SetWindowPos(p.MainWindowHandle, this.Handle, 0, 0, Screen.GetWorkingArea(this).Width * 2 / 3, Screen.GetWorkingArea(this).Height, SHOWWINDOW);
                }
            }
            //FIND Python3.exe
            DirFileSearch(@"C:\Users\lg\AppData\Local\", "python");
            label1.Text = "Naver Translation";
            label2.Text = "Kakao Translation";
            label1.Font = new Font("맑은고딕", 15, FontStyle.Bold);
            label2.Font = new Font("맑은고딕", 15, FontStyle.Bold);
            richTextBox1.Text = "Input text";
            button1.Text = "번역";
            button2.Text = "오류 검사";
            button2.Click += button2_Click;
            button1.Font = new Font("맑은 고딕", 15, FontStyle.Bold);
            button2.Font = new Font("맑은 고딕", 15, FontStyle.Bold);
            richTextBox1.GotFocus += richTextBox1_GotFocus;
            richTextBox1.LostFocus += richTextBox1_LostFocus;

            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            button2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //richTextBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //richTextBox3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        }

        public static string run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = filePath;
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
                    if (stderr != "")
                    {
                        MessageBox.Show(stderr);
                        return null;
                    }
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
            if(translated_text == null) { return; }
            System.Drawing.Font newFont = new Font("Verdana", 10f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 178, false);
            richTextBox2.Text = translated_text;
            richTextBox2.Font = new Font("맑은고딕", 20, FontStyle.Bold);

            string[] tmp_str = richTextBox1.Text.Split(new char[] { '\n' });
            string tmp = "";
            for (int i = 0; i < tmp_str.Length; i++)
            {
                if (tmp_str[i] == "") continue;
                tmp += tmp_str[i] + " ";
            }
            string data = source_lang + " " + tmp;
            richTextBox3.Text = run_cmd("kakao_translation.py", data);
            richTextBox3.Font = new Font("맑은고딕", 20, FontStyle.Bold);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == null || richTextBox1.Text == "")
            {
                richTextBox1.Text = "Input text please";
                return;
            }

            var source_lang = detectLang(richTextBox1.Text);
            string data = source_lang + " " + richTextBox1.Text;
            string result = run_cmd("error_check.py", data);
            if(result != null)
            {
                MessageBox.Show(result, "오류검사");
            }
        }

        private void richTextBox1_GotFocus(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.Font = new Font("맑은고딕", 10, FontStyle.Bold);
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
            string url = "https://naveropenapi.apigw.ntruss.com/nmt/v1/translation";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "");
            request.Headers.Add("X-NCP-APIGW-API-KEY", "");
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
            string translated_text = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                translated_text = reader.ReadToEnd();
                stream.Close();
                response.Close();
                reader.Close();
            }
            catch (WebException e)
            {
                string result = run_cmd("error_check.py", source_lang + " " + input_text);
                if(result != null)
                {
                    MessageBox.Show(result, "오류 발견");
                }
                return null;
            }

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

        //it will find your python3 of python3.exe filepath but it you should your python file in local system
        static void DirFileSearch(string path, string file)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path, $"{file}.exe");
                foreach(string f in files)
                {
                    Console.WriteLine($"[{count++}] path - {f}");
                    if (f.Contains("Python36") || f.Contains("Python37") || f.Contains("Python38")) { filePath = f; return; }
                }

                if(dirs.Length > 0)
                {
                    foreach(string dir in dirs)
                    {
                        DirFileSearch(dir, file);
                    }
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex);
            }
        }

    }
}
