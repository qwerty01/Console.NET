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
    /// Event info for looping
    /// </summary>
    public class LoopEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the loop should continue.
        /// </summary>
        public bool Continue { get; set; }
        /// <summary>
        /// Whether or not the console should continue parsing commands
        /// </summary>
        public bool Parse { get; set; }
        /// <summary>
        /// Gets command line arguments
        /// Arguments[0] = arguments
        /// Arguments[1] = command
        /// Arguments[2...N] = argument 1...N-1
        /// 
        /// Example:
        /// echo test1 test2 test3 "test4 test5"
        /// Arguments = {"test1 test2 test3", "echo", "test1", "test2", "test3", "test4 test5"}
        /// </summary>
        public string[] Arguments { get; protected set; }
        /// <summary>
        /// Contains the full command
        /// </summary>
        public string FullCommand { get; protected set; }
        /// <summary>
        /// Constructs new event args
        /// </summary>
        /// <param name="cont">Initial state of continue condition (default: true)</param>
        /// <param name="parse">Whether or not to continue parsing commands</param>
        public LoopEventArgs(bool cont = true, bool parse = true)
        {
            Continue = cont;
            Parse = parse;
        }
        /// <summary>
        /// Creates an array from command-line style args
        /// </summary>
        /// <param name="args">Entire argument string</param>
        public void SetArguments(string args)
        {
            FullCommand = args;
            Arguments = GetArguments(args);
        }
        /// <summary>
        /// Returns an array from command-line style args
        /// </summary>
        /// <param name="args">Entire argument string</param>
        /// <returns>Argument array</returns>
        public static string[] GetArguments(string args)
        {
            List<string> cmdArgs = new List<string>(); // List of arguments
            string curr = ""; // Current argument
            string arguments = ""; // Full string containing arguments (making echo easier)
            bool isArg = false; // Whether or not we're on the argument list
            bool inString = false; // Whether or not we're in a string

            // Check if there is any content
            if (!args.Any(x => char.IsLetterOrDigit(x)))
            {
                // Nope, set arguments to empty
                cmdArgs.Add("");
                cmdArgs.Add("");
                return cmdArgs.ToArray();
            }

            // Loop through the arguments
            // Note: One interesting thing to point out is you can have spaces in commands:
            //   "test command" asdf asdf
            // will call the command "test command"
            for (int i = 0; i < args.Length; i++)
            {
                if (isArg) // Add to the arguments string if we're in the arguments section
                    arguments += args[i];
                if (args[i] == '"') // There's a quote, treat everything as one argument
                    inString = !inString;
                else if (!inString && args[i] == ' ') // Separate arguments by spaces (unless we're in a string)
                {
                    cmdArgs.Add(curr); // Add the past argument to the list
                    curr = ""; // New argument
                    isArg = true; // We're definitely about to process an argument
                }
                else
                    curr += args[i]; // Add content to the current argument
            }

            cmdArgs.Add(curr); // Add the last command to the list
            cmdArgs.RemoveAll(x => x == ""); // Remove all empty arguments
            cmdArgs.Insert(0, arguments); // Add in the full argument string to args[0]
            return cmdArgs.ToArray(); // Convert the arguments to an array
        }
    }
}
