using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RemoteControlHost.Library
{
    [Export(typeof(IRemoteControlModule))]
    public class VlcControlModule : IRemoteControlModule
    {
        public string ModuleName { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public List<IRemoteControlCommand> Commands { get; private set; }

        public VlcControlModule()
        {
            ModuleName = "VLC";
            Commands = new List<IRemoteControlCommand>()
                {
                    new MediaControlCommand("start/stop","Start/stop",StartStop,0,0),
                    new MediaControlCommand("fullscreen","Fullscreen on/off",FullScreen,0,1),
                };
            Rows = 1;
            Columns = 2;
        }

        private void FullScreen()
        {
            SendKeys.SendWait("f");
        }

        private void StartStop()
        {
            // Select window
            // SelectVlcWindow();
            
            SendKeys.SendWait(" ");
        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void SelectVlcWindow()
        {
            var process = Process.GetProcessesByName("vlc");
            
            if (process.Any())
            {
                var handle = FindWindow("QWidget", null);
                //ShowWindow(handle, SW_RESTORE);
                SetForegroundWindow(handle);
            }
        }

       
    }
}
