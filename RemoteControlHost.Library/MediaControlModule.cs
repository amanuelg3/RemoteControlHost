using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace RemoteControlHost.Library
{
    [Export(typeof(IRemoteControlModule))]
    public class MediaControlModule : IRemoteControlModule
    {
        public string ModuleName { get; private set; }
        public List<IRemoteControlCommand> Commands { get; private set; }

        public MediaControlModule()
        {
            ModuleName = "Media Control Buttons";

            Commands = new List<IRemoteControlCommand>()
                {
                    new MediaControlCommand("prev","Previous",Previous),
                    new MediaControlCommand("playpause", "Play/Pause", PlayPause),
                    new MediaControlCommand("next","Next",Next),
                };
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

        public MediaControlCommand(string name, string text, Action action)
        {
            CommandName = name;
            CommandText = text;
            ExecuteCommand = action;
        }
    }
}
