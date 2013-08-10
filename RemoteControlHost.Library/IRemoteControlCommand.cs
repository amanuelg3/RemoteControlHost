using System;

namespace RemoteControlHost.Library
{
    /// <summary>
    /// Command for remote execution
    /// </summary>
    public interface IRemoteControlCommand
    {
        /// <summary>
        /// Name of command - must be unique for module
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Text to be shown on client
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// Action to be executed when activated via client
        /// </summary>
        Action ExecuteCommand { get; }

        /// <summary>
        /// Row to display control in
        /// </summary>
        int Row { get; }

        /// <summary>
        /// Column to displa control in
        /// </summary>
        int Column { get; }
    }
}