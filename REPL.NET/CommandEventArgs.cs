using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.NET
{
    /// <summary>
    /// Event arguments for running a command
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not to close the window (Default: false)
        /// </summary>
        public bool Quit { get; set; }
        /// <summary>
        /// Whether or not the command was called with help
        /// </summary>
        public bool Help { get; protected set; }
        /// <summary>
        /// Argument list passed to the function
        /// 
        /// Layout:
        /// Arguments[0] = string containing arguments
        /// Arguments[1] = command that was run
        /// Arguments[2] = First argument
        /// Arguments[N] = N-1th argument
        /// 
        /// Example:
        /// command arg1 "arg2 arg2" arg3
        /// Arguments[0] = "arg2 arg3 \"arg4 arg4\" arg5"
        /// Arguments[1] = "command"
        /// Arguments[2] = "arg1"
        /// Arguments[3] = "arg2 arg2"
        /// Arguments[4] = "arg3"
        /// </summary>
        public string[] Arguments { get; protected set; }
        /// <summary>
        /// Creates a new command event arg
        /// </summary>
        /// <param name="args">Command line argument list</param>
        /// <param name="help">Whether or not function was called from help</param>
        /// <param name="quit">True if the window should exit</param>
        public CommandEventArgs(string[] args, bool help = false, bool quit = false)
        {
            Quit = quit;
            Help = help;
            Arguments = args; // should this be deep copied?
        }
    }
}
