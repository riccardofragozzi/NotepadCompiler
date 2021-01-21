using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace NotepadCompiler
{

    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(Keys key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
            {
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            }
            else
            {
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            }
        }

        void TestProc()
        {
            //PressKey(Keys.ControlKey, false);
            PressKey(Keys.P, false);
            PressKey(Keys.P, true);
            //PressKey(Keys.ControlKey, true);
        }
        // Simulate a key press event


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon.Icon = Properties.Resources.normal;
            if (!Directory.Exists("C:\\NPC"))
            {
                MessageBox.Show("Please create C:\\NPC\\ folder to continue.");
                Close();
            }
            HotKeyManager.RegisterHotKey(Keys.K, KeyModifiers.Alt);
            HotKeyManager.HotKeyPressed += (s, ea) => {
            };
            Hide();
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }


        private async void runCompilerProcess()
        {
            setIcon(Properties.Resources.compiling);
            await Task.Delay(1000);

            if (System.IO.File.Exists("C:\\NPC\\NPC_status"))
            {
                System.IO.File.Delete("C:\\NPC\\NPC_status");
            }
            if (!System.IO.File.Exists("C:\\NPC\\NPC_compile_output"))
            {
                System.IO.File.Create("C:\\NPC\\NPC_compile_output");
            }
            if (!System.IO.File.Exists("C:\\NPC\\NPC_exec_output"))
            {
                System.IO.File.Create("C:\\NPC\\NPC_exec_output");
            }

            string input = getClipboard();
            string compileOut = "";
            string execOutput = "";

            //System.IO.File.WriteAllText("C:\\NPC\\NPC_source.c", input);
            Process.Start("C:\\NPC\\NPC_process.bat");
            //bool fileExists = false;
            while (!System.IO.File.Exists("C:\\NPC\\NPC_status"))
            {
                setIcon(Properties.Resources.normal);
                await Task.Delay(250);
                setIcon(Properties.Resources.compiling);
                await Task.Delay(250);
            }

            compileOut = System.IO.File.ReadAllText("C:\\NPC\\NPC_compile_output");
            execOutput = System.IO.File.ReadAllText("C:\\NPC\\NPC_exec_output");

            if (compileOut.Trim().Length == 0)
            {
                setClipboard(execOutput);
                setIcon(Properties.Resources.compileok);
            }
            else
            {
                setIcon(Properties.Resources.compileerror);
                setClipboard(compileOut);
            }

            await Task.Delay(2500);
            setIcon(Properties.Resources.normal);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            runCompilerProcess();
        }
        private void setIcon(Icon icon)
        {
            this.Invoke(new Action(() => { notifyIcon.Icon = icon; }));
        }
        private void setClipboard(string text)
        {
            this.Invoke(new Action(() => { Clipboard.SetText(text); }));
        }
        private string getClipboard()
        {
            string text = "";
            this.Invoke(new Action(() => { text = Clipboard.GetText(); }));
            return text;
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Close();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Visible = true;
            WindowState = FormWindowState.Normal;
        }
    }

    public static class HotKeyManager
    {
        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

        public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            _windowReadyEvent.WaitOne();
            int id = System.Threading.Interlocked.Increment(ref _id);
            _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)modifiers, (uint)key);
            return id;
        }

        public static void UnregisterHotKey(int id)
        {
            _wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), _hwnd, id);
        }

        delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
        delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

        private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
        {
            RegisterHotKey(hwnd, id, modifiers, key);
        }

        private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
        {
            UnregisterHotKey(_hwnd, id);
        }

        private static void OnHotKeyPressed(HotKeyEventArgs e)
        {
            if (HotKeyManager.HotKeyPressed != null)
            {
                HotKeyManager.HotKeyPressed(null, e);
            }
        }

        private static volatile MessageWindow _wnd;
        private static volatile IntPtr _hwnd;
        private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
        static HotKeyManager()
        {
            Thread messageLoop = new Thread(delegate ()
            {
                Application.Run(new MessageWindow());
            });
            messageLoop.Name = "MessageLoopThread";
            messageLoop.IsBackground = true;
            messageLoop.Start();
        }

        private class MessageWindow : Form
        {
            public MessageWindow()
            {
                _wnd = this;
                _hwnd = this.Handle;
                _windowReadyEvent.Set();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                    HotKeyManager.OnHotKeyPressed(e);
                }

                base.WndProc(ref m);
            }

            protected override void SetVisibleCore(bool value)
            {
                // Ensure the window never becomes visible
                base.SetVisibleCore(false);
            }

            private const int WM_HOTKEY = 0x312;
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int _id = 0;
    }


    public class HotKeyEventArgs : EventArgs
    {
        public readonly Keys Key;
        public readonly KeyModifiers Modifiers;

        public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint)hotKeyParam.ToInt64();
            Key = (Keys)((param & 0xffff0000) >> 16);
            Modifiers = (KeyModifiers)(param & 0x0000ffff);
        }
    }

    [Flags]
    public enum KeyModifiers
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }



}

