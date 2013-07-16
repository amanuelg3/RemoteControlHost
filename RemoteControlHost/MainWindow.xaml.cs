using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using RemoteControlHost.Library;

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
        private RemoteControlRepository _remoteControlRepository;

        public MainWindow()
        {
            InitializeComponent();


        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _remoteControlRepository = new RemoteControlRepository();

            _udpThread = new Thread(ReceiveUdpPackets);
            _udpThread.IsBackground = true;
            _udpThread.Start();

            _httpThread = new Thread(HttpServeThead);
            _httpThread.IsBackground = true;
            _httpThread.Start();
        }

        private XDocument GetXmlSetup()
        {
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var moduleRoot = new XElement("modules");
            document.Add(moduleRoot);
            foreach (var module in _remoteControlRepository.Modules)
            {
                var moduleXml = new XElement("module", new XAttribute("name", module.Key));
                foreach (var command in module.Value.Commands)
                    moduleXml.Add(new XElement("command", new XAttribute("cmd", module.Key + ":::" + command.CommandName), command.CommandText));
                moduleRoot.Add(moduleXml);
            }
            return document;
        }

        /// <summary>
        /// HttpServeThread answers requests for setup
        /// </summary>
        private void HttpServeThead()
        {
            _httpServer = new HttpListener();
            _httpServer.Prefixes.Add("http://192.168.0.25:50004/");
            _httpServer.Start();
            while (true)
            {
                var context = _httpServer.GetContext();

                // TODO: Do something module related depending on request
                HttpListenerRequest request = context.Request;

                var doc = GetXmlSetup();
                var response = context.Response;
                doc.Save(response.OutputStream);
                response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Receive commands over Udp
        /// </summary>
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

        /// <summary>
        /// Handle messages received over udp (invoked such that it is run on GUI thread).
        /// </summary>
        /// <param name="messageText"></param>
        private void HandleMessage(string messageText)
        {
            var parts = messageText.Split(new string[] { ":::" }, 2, StringSplitOptions.None);
            IRemoteControlModule module;
            if (_remoteControlRepository.Modules.TryGetValue(parts[0], out module))
            {
                var command = module.Commands.Find(c => c.CommandName == parts[1]);
                if (command != null)
                {
                    command.ExecuteCommand();
                }
            }
        }

    }
}
