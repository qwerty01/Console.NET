#region The MIT License (MIT) - Copyright (c) 2014 Qwerty01 (qw3rty01@gmail.com, http://github.com/qwerty01)
/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014 Qwerty01 (qw3rty01@gmail.com, http://github.com/qwerty01)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion
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
        /// Contains the full command
        /// </summary>
        public string FullCommand { get; protected set; }
        /// <summary>
        /// Creates a new command event arg
        /// </summary>
        /// <param name="args">Command line argument list</param>
        /// <param name="fullCommand">Full console command</param>
        /// <param name="help">Whether or not function was called from help</param>
        /// <param name="quit">True if the window should exit</param>
        public CommandEventArgs(string[] args, string fullCommand, bool help = false, bool quit = false)
        {
            Quit = quit;
            Help = help;
            Arguments = args; // should this be deep copied?
            FullCommand = fullCommand;
        }
    }
}
