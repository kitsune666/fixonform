using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace FixOnForm
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKeys);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        IntPtr _ha;
        bool _state = true;
        string _wName = "";
        bool state = false;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();
                _ha = GetForegroundWindow();

                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    label1.ForeColor = Color.Green;
                    _wName = Buff.ToString();
                    _ha = handle;
                }
                else
                {
                    label1.ForeColor = Color.Red;
                    _wName = "nothing";
                }
                try
                {
                    BeginInvoke(new Action(() => label1.Text = _wName));
                }
                catch { }

                
                Thread.Sleep(100);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int k = 75, i = 73;//K I
            while (true)
            {

                if (GetAsyncKeyState(k) != 0 && GetAsyncKeyState(i) != 0)
                {
                    ABC();
                }
                Thread.Sleep(100);
            }
        }

        void ABC() {

            state = !state;

            uint pid;
            GetWindowThreadProcessId(_ha, out pid);
            Process p = Process.GetProcessById((int)pid);

            Rectangle myRect = new Rectangle();
            RECT rct;

            GetWindowRect(new HandleRef(this, _ha), out rct);


            myRect.X = rct.Left;
            myRect.Y = rct.Top;
            myRect.Width = rct.Right - rct.Left - 5;
            myRect.Height = rct.Bottom - rct.Top - 5;

            if (state == true)
            {
                ForeColor = Color.Red;
                BeginInvoke(new Action(() => label2.Text = "Lock = " + Convert.ToString(_wName)));
                Cursor.Clip = new Rectangle(new Point(myRect.X+2, myRect.Y), new Size(myRect.Width, myRect.Height));
                notifyIcon1.BalloonTipText = "Lock = " + Convert.ToString(_wName);
                notifyIcon1.ShowBalloonTip(1000);
               
            }

            if (state == false)
            {

                ForeColor = Color.Green;
                BeginInvoke(new Action(() => label2.Text = "Unlock = " + Convert.ToString(_wName)));
                Cursor.Clip = Rectangle.Empty;
                notifyIcon1.BalloonTipText = "Unlock = " + Convert.ToString(_wName);
                notifyIcon1.ShowBalloonTip(1000);
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state == true)
            {
                this.Hide();
                _state = false;
            }
            else if (_state == false)
            {
                this.Show(); _state = true;
            }
        }

    }
}
