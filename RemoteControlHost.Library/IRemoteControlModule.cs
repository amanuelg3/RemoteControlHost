using System.Collections.Generic;

namespace RemoteControlHost.Library
{
    /// <summary>
    /// Implement IRemoteControlModule interface and add Export(typeof(IRemoteControlModule)) 
    /// to addd the module to the remote control host.
    /// </summary>
    public interface IRemoteControlModule
    {
        /// <summary>
        /// Name of module - shown as 'tab' on clients
        /// </summary>
        string ModuleName { get;  }

        /// <summary>
        /// Number of rows for layout
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// Number of colums for layout
        /// </summary>
        int Columns { get; }
        
        /// <summary>
        /// List of commands to be shown on client
        /// </summary>
        List<IRemoteControlCommand> Commands { get; } 
    }
}
