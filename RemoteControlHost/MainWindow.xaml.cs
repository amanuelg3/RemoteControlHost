using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace RemoteControlHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UdpClient _udpClient;
        private Thread _udpThread;
        private HttpListener _httpServer;
        private Thread _httpThread;

        public MainWindow()
        {
            InitializeComponent();

            _udpThread = new Thread(ReceiveUdpPackets);
            _udpThread.IsBackground = true;
            _udpThread.Start();

            _httpThread = new Thread(HttpServeThead);
            _httpThread.IsBackground = true;
            _httpThread.Start();
        }

        private XDocument GetXmlSetup()
        {
            var document = new XDocument(
                new XDeclaration("1.0","utf-8","yes"),
                new XElement("commands",
                    new XElement("previous","Previous"),
                    new XElement("playpause","Play/pause"),
                    new XElement("next","Next"))
                );
            return document;
        }

        private void HttpServeThead()
        {
            _httpServer = new HttpListener();
            _httpServer.Prefixes.Add("http://192.168.0.25:50004/");
            _httpServer.Start();
            while (true)
            {
                var context =_httpServer.GetContext();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                
                // Construct a response. 
                string responseString = GetXmlSetup().ToString(SaveOptions.DisableFormatting);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }

        private void ReceiveUdpPackets()
        {
            _udpClient = new UdpClient(50000);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                var data = _udpClient.Receive(ref RemoteIpEndPoint);
                var txt = Encoding.UTF8.GetString(data);
                Dispatcher.Invoke(() => HandleMessage(txt));
            }
        }

        private void HandleMessage(string messageText)
        {
            switch (messageText)
            {
                case "next":
                    NextCommand();
                    break;
                case "previous":
                    PreviousCommand();
                    break;
                case "playPause":
                    PlayPauseCommand();
                    break;
            }
        }

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            /*Virtual Key code.  Must be from 1-254.  If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.*/
            public ushort wVk;
            /*A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.*/
            public ushort wScan;
            /*Specifies various aspects of a keystroke.  See the KEYEVENTF_ constants for more information.*/
            public uint dwFlags;
            /*The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.*/
            public uint time;
            /*An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.*/
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private INPUT[] GenereateInputForVirtualKeyCode(byte keyCode)
        {
            return new INPUT[]
                {
                    new INPUT
                        {
                            type = INPUT_KEYBOARD,
                            u = new InputUnion
                                {
                                    ki = new KEYBDINPUT
                                        {
                                            wVk = keyCode,
                                            wScan = 0,
                                            dwFlags = 0,
                                            dwExtraInfo = GetMessageExtraInfo(),
                                        }
                                }
                        }
                };
        }

        /// <summary>
        /// Keycodes : http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
        /// </summary>
        /// <param name="key"></param>
        private void SendVirtualKey(byte key)
        {
            var pInput = GenereateInputForVirtualKeyCode(key);
            SendInput(1, pInput, Marshal.SizeOf(typeof(INPUT)));
        }

        private void PreviousCommand()
        {
            SendVirtualKey(0xB1);            
        }

        private void NextCommand()
        {
            SendVirtualKey(0xB0);            
        }

        private void PlayPauseCommand()
        {
            SendVirtualKey(0xB3);
        }
    }
}
