using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace NotepadCompiler
{

    public partial class Form1 : Form
    {

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd,
            int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        public Form1()
        {
            InitializeComponent();
        }

        private string batCompile = "set PATH=C:\\MinGW\\bin;%PATH% & del \"C:\\NPC\\NPC_compiled.exe\" >nul 2>&1 & gcc C:\\NPC\\NPC_source.c -o \"C:\\NPC\\NPC_compiled.exe\">C:\\NPC\\NPC_compile_output 2>&1";
        private string vbsCompile = "Set oShell = CreateObject(\"Wscript.Shell\") : Dim strArgs: strArgs = \"cmd /c C:\\NPC\\NPC_compile.bat\" : oShell.Run strArgs, 0, false";
        
        private string batExecute = "start \"\" \"cmd /c C:\\NPC\\NPC_compiled.exe>C:\\NPC\\NPC_execute_output\"";
        private string vbsExecute = "Set oShell = CreateObject(\"Wscript.Shell\") : Dim strArgs: strArgs = \"cmd /c C:\\NPC\\NPC_execute.bat\" : oShell.Run strArgs, 0, false";

        private int closeReqTimes = 0;

        private Process notepadProcess = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon.Icon = Properties.Resources.normal;
            if (!Directory.Exists("C:\\NPC"))
            {
                MessageBox.Show("Please create C:\\NPC\\ folder to continue.");
                Close();
            }

            if (!Directory.Exists("C:\\MinGW\\bin"))
            {
                MessageBox.Show("Please install MinGW compiler on C:\\MinGW\\ directory!");
                Close();
            }

            int id1 = HotKeyManager.RegisterHotKey(Keys.C, KeyModifiers.Alt);
            int id2 = HotKeyManager.RegisterHotKey(Keys.S, KeyModifiers.Alt);
            int id3 = HotKeyManager.RegisterHotKey(Keys.X, KeyModifiers.Alt);
            int id4 = HotKeyManager.RegisterHotKey(Keys.N, KeyModifiers.Alt);

            HotKeyManager.HotKeyPressed += async (s, ea) => {
                if (notepadProcess != null && notepadProcess.HasExited)
                {
                    notepadProcess = null;
                }
                closeReqTimes = 0;

                if(ea.Key == Keys.N)
                {
                    System.IO.File.AppendAllText("C:\\NPC\\Senza nome", "");
                    //start notepad on topmost, opening 'Senza titolo' file
                    notepadProcess = Process.Start("notepad.exe", "C:\\NPC\\Senza nome");
                    while (notepadProcess.MainWindowHandle.ToInt32() == 0)
                    {
                        await Task.Delay(100);
                    }
                }

                if (ea.Key == Keys.X)
                {
                    //close request
                    closeReqTimes++;
                    if (closeReqTimes == 5)
                    {
                        notifyIcon.Visible = false;

                        HotKeyManager.UnregisterHotKey(id1);
                        HotKeyManager.UnregisterHotKey(id2);
                        HotKeyManager.UnregisterHotKey(id3);
                        HotKeyManager.UnregisterHotKey(id4);
                        Close();
                        return;
                    }
                    return;
                }

                if (ea.Key == Keys.C)
                {
                    //source from clipboards
                    runCompilerProcess(getClipboard());
                }

                if (ea.Key == Keys.S)
                {
                    // source from text-file
                    if (!File.Exists("C:\\NPC\\Senza nome"))
                    {
                        //show error
                        setIcon(Properties.Resources.compileerror);
                        await Task.Delay(150);
                        setIcon(Properties.Resources.normal);
                        await Task.Delay(150);
                        setIcon(Properties.Resources.compileerror);
                        await Task.Delay(150);
                        setIcon(Properties.Resources.normal);
                        await Task.Delay(150);
                        setIcon(Properties.Resources.compileerror);
                        await Task.Delay(150);
                        setIcon(Properties.Resources.normal);
                        await Task.Delay(150);
                        return;
                    }
                    runCompilerProcess(System.IO.File.ReadAllText("C:\\NPC\\Senza nome"));
                }

            };
            Hide();
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }


        private async void runCompilerProcess(string source)
        {
            setIcon(Properties.Resources.compiling);

            if (notepadProcess != null)
            {
                SetWindowPos(notepadProcess.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }

            await Task.Delay(1000);

            string compileOut = "";
            string execOutput = "";

            System.IO.File.WriteAllText("C:\\NPC\\NPC_compile.bat", batCompile);
            System.IO.File.WriteAllText("C:\\NPC\\NPC_compile.vbs", vbsCompile);

            System.IO.File.WriteAllText("C:\\NPC\\NPC_execute.bat", batExecute);
            System.IO.File.WriteAllText("C:\\NPC\\NPC_execute.vbs", vbsExecute);

            System.IO.File.WriteAllText("C:\\NPC\\NPC_source.c", source);

            if (File.Exists("C:\\NPC\\NPC_compile_output"))
            {
                File.Delete("C:\\NPC\\NPC_compile_output");
            }
            Process.Start("C:\\NPC\\NPC_compile.vbs");
            while (!System.IO.File.Exists("C:\\NPC\\NPC_compile_output"))
            {
                setIcon(Properties.Resources.normal);
                await Task.Delay(250);
                setIcon(Properties.Resources.compiling);
                await Task.Delay(250);
            }

            await Task.Delay(500);
            compileOut = readFile("C:\\NPC\\NPC_compile_output", true)[0];
            Console.WriteLine("COMPILE OUT: " + compileOut);
            if (compileOut.Trim().Length != 0)
            {
                //compilation failure
                setIcon(Properties.Resources.compileerror);

                //find output with error
                bool errorFound = false;
                foreach(string line in readFile("C:\\NPC\\NPC_compile_output", false))
                {
                    if (line.Contains("error:"))
                    {
                        errorFound = true;
                        string nl = line.Substring(line.IndexOf("error:") + 6);
                        setClipboard(nl);
                        break;
                    }
                }
                if (!errorFound)
                {
                    setClipboard(compileOut);
                }

                await Task.Delay(2500);
                setIcon(Properties.Resources.normal);

                if (notepadProcess != null)
                {
                    SetWindowPos(notepadProcess.MainWindowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                }
                return;
            }


            if (File.Exists("C:\\NPC\\NPC_execute_output"))
            {
                File.Delete("C:\\NPC\\NPC_execute_output");
            }
            Process.Start("C:\\NPC\\NPC_execute.vbs");
            while (!System.IO.File.Exists("C:\\NPC\\NPC_execute_output"))
            {
                setIcon(Properties.Resources.normal);
                await Task.Delay(250);
                setIcon(Properties.Resources.compiling);
                await Task.Delay(250);
            }

            await Task.Delay(500);
            execOutput = readFile("C:\\NPC\\NPC_execute_output", true)[0];

            if (notepadProcess != null)
            {
                SetWindowPos(notepadProcess.MainWindowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }

            setClipboard(execOutput);
            setIcon(Properties.Resources.compileok);

            await Task.Delay(2500);
            setIcon(Properties.Resources.normal);
        }

        private string[] readFile(string filename, bool inline)
        {
            string[] content = { };
            bool success = false;
            while (!success)
            {
                try
                {
                    content = System.IO.File.ReadAllLines(filename);
                    success = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("read file error: " + ex.Message);
                    success = false;
                }
            }
            if (inline)
            {
                string inlineS = "";
                foreach(string s in content)
                {
                    inlineS += s + Environment.NewLine;
                }
                string[] inlineSS = { inlineS };
                return inlineSS;
            }
            return content;
        }

        private void setIcon(Icon icon)
        {
            this.Invoke(new Action(() => { notifyIcon.Icon = icon; }));
        }
        private void setClipboard(string text)
        {
            string cb = text;
            if (string.IsNullOrEmpty(cb))
            {
                cb = "--";
            }
            this.Invoke(new Action(() => { Clipboard.SetText(cb); }));
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            Visible = false;
            WindowState = FormWindowState.Minimized;
            e.Cancel = true;
            notifyIcon.ShowBalloonTip(5, "Already running!", "To leave, right Click on this icon --> Esci", ToolTipIcon.Info);
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

