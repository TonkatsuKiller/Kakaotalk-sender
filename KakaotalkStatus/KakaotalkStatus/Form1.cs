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
using System.Threading;
using System.IO;

namespace KakaotalkStatus
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string ipclassname, string ipwindowname);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwnd1, IntPtr hwnd2, string ipsz1, string ipsz2);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wmsg, int wparam, string iparam);

        [DllImport("user32.dll")]
        public static extern uint PostMessage(IntPtr hwnd, int wmsg, int wparam, int iparam);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hwnd, int ncmdshow);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        Thread kakaotalkThread;
        void kakaotalkMain()
        {
            int st = 0;
            List<string> set = new List<string>();
            List<string> rooms = new List<string>();
            for (int i = 0; i < List.Rows.Count; i++)
            {
                rooms.Add(List.Rows[i].Cells[0].Value.ToString());
            }

            if (rooms.Count != 0)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    IntPtr room = FindWindow(null, rooms[i]);
                    if (room != IntPtr.Zero)
                    {
                        Thread.Sleep(100);
                        IntPtr chk = FindWindowEx(room, IntPtr.Zero, "RICHEDIT50W", "");
                        if (chk != IntPtr.Zero)
                        {
                            Thread.Sleep(100);
                            SendMessage(chk, 0x000c, 0, "Test text"); //0x000c = set text
                            Thread.Sleep(100);
                            PostMessage(chk, 0x0100, 0xD, 0x0101); //0x0100 = KeyDown //0xD = 줄바꿈 (Enter 추정) //0x0101 = KeyUp

                            st = 0;
                        }
                    }
                    else
                    {
                        st = 1;
                    }
                    set.Add(rooms[i] + " " + st);
                }
            }
            List.Rows.Clear();
            for (int i = 0; i < set.Count; i++)
            {
                string value = set[i];
                var str = value.Split(' ').Last();
                var room = value.Replace(str, "").Trim();
                
                if (str == "0")
                {
                    List.Rows.Add(room, "Successed");
                    List.Rows[i].Cells[1].Style.ForeColor = Color.Green;
                }
                else if (str == "1")
                {
                    List.Rows.Add(room, "Failed");
                    List.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                }
            }
        }

        public static string path = null;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "all files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                List.Rows.Clear();
                path = null;
                path = ofd.FileName;
                string[] contents = File.ReadAllLines(path);
                for (int i = 0; i < contents.Length; i++)
                {
                    List.Rows.Add(contents[i], "-");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            kakaotalkMain();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            List.Rows.Clear();
            string[] contents = File.ReadAllLines(path);
            for (int i = 0; i < contents.Length; i++)
            {
                List.Rows.Add(contents[i], "-");
            }
        }
    }
}
