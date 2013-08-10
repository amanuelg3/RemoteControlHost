using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace RemoteControlHost.Library
{
    [Export(typeof(IRemoteControlModule))]
    public class MediaControlModule : IRemoteControlModule
    {
        public string ModuleName { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public List<IRemoteControlCommand> Commands { get; private set; }

        public MediaControlModule()
        {
            ModuleName = "Media Control Buttons";

            Commands = new List<IRemoteControlCommand>()
                {
                    new MediaControlCommand("prev","Previous",Previous,0,0),
                    new MediaControlCommand("playpause", "Play/Pause", PlayPause,0,1),
                    new MediaControlCommand("next","Next",Next,0,2),
                    new MediaControlCommand("pcspeakers","PC Speakers", PcSpeakers,1,0),
                    new MediaControlCommand("pcspeakers","Speakers", Speakers,2,0),
                    new MediaControlCommand("pcspeakers","TV Speakers", TvSpeakers,3,0),
                };
            Rows = 4;
            Columns = 3;
        }

        private void PcSpeakers()
        {
            throw new NotImplementedException();
        }

        private void Speakers()
        {
            throw new NotImplementedException();
        }

        private void TvSpeakers()
        {
            throw new NotImplementedException();
        }

        private void Next()
        {
            var keyboard = new KeyboardEvents();
            keyboard.NextCommand();
        }

        private void PlayPause()
        {
            var keyboard = new KeyboardEvents();
            keyboard.PlayPauseCommand();
        }

        private void Previous()
        {
            var keyboard = new KeyboardEvents();
            keyboard.PreviousCommand();
        }


    }

    public class MediaControlCommand : IRemoteControlCommand
    {
        public string CommandName { get; private set; }
        public string CommandText { get; private set; }
        public Action ExecuteCommand { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }


        public MediaControlCommand(string name, string text, Action action, int row, int column)
        {
            CommandName = name;
            CommandText = text;
            ExecuteCommand = action;
            Row = row;
            Column = column;
        }
    }
}
