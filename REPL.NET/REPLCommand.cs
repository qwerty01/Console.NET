#region License: MIT, Copyright (c) 2014 Qwerty01 (qw3rty01@gmail.com, http://github.com/qwerty01)
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
    /// Object used to hold command info
    /// </summary>
    // TODO:
    // Add a "smarter" help system
    public class REPLCommand
    {
        /// <summary>
        /// Delegate for running a command
        /// </summary>
        /// <param name="sender">REPL that called the command</param>
        /// <param name="e">Event args for running a command. Important: Add a check if the command was called via help (more info in CommandEventArgs)</param>
        /// <returns>What to print to the console</returns>
        public delegate string CommandDelegate(ConsoleREPL sender, CommandEventArgs e);
        /// <summary>
        /// Name of the command
        /// Used to check against command line
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// REPL window calling the function
        /// </summary>
        public ConsoleREPL Parent { get; protected set; }
        /// <summary>
        /// Function called every time the command is used
        /// </summary>
        public CommandDelegate Callback { get; protected set; }
        /// <summary>
        /// String containing usage info for help
        /// 
        /// Standard Format:
        /// [required argument 1] (optional command 2) etc.
        /// Displays as:
        /// Usage: command [required argument 1] (optional command 2)
        /// </summary>
        public string Usage { get; protected set; }
        /// <summary>
        /// String containing command description for help
        /// 
        /// Displays as:
        /// Description: [description]
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// Creates a new command
        /// </summary>
        /// <param name="name">Name of the command, used to check against input</param>
        /// <param name="usage">Usage info for help</param>
        /// <param name="description">Description for help</param>
        /// <param name="callback">Function to call when this command is called</param>
        /// <param name="parent">REPL window that owns this command</param>
        public REPLCommand(string name, string usage, string description, CommandDelegate callback, ConsoleREPL parent)
        {
            Name = name;
            Callback = callback;
            Parent = parent;
            Usage = usage;
            Description = description;
        }
    }
}
