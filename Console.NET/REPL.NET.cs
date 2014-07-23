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
using REPL.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.NET
{
    class REPLTester
    {
        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            ConsoleREPL main = new ConsoleREPL(ConsoleEval, "Main"); // Create a new console window
            main.AddCommand("addwindow", "[window name]", "Adds a new window with name [window name]", CmdAddWindow); // Add addwindow to the commands
            main.AddCommand("argtest", "(args)", "Displays argument information", CmdArgTest); // Add argtest to the commands
            main.AddCommand("echo", "[string]", "Echos [string] to the console", CmdEcho); // Add echo to the commands
            main.AddCommand("exit", "", "Exits the window", CmdExit); // Add exit to the commands
            main.AddCommand("quit", "", "Exits the window", CmdExit); // Add quit to the commands
            main.AddCommand("window", "[window]", "Switches to a different window", CmdWindow); // Add window to the commands
            ConsoleREPL.HookConsole(main); // Hook the console
        }
        /// <summary>
        /// Main eval callback
        /// </summary>
        /// <param name="sender">ConsoleREPL window that called the command</param>
        /// <param name="e">Loop arguments</param>
        /// <returns>String containing the responses</returns>
        static string ConsoleEval(ConsoleREPL sender, LoopEventArgs e)
        {
            // This text is used if a command is not found
            return "Unknown command " + e.Arguments[1];
        }
        /// <summary>
        /// Exit the window
        /// </summary>
        /// <param name="sender">ConsoleREPL window</param>
        /// <param name="e">Arguments</param>
        /// <returns>String containing the responses</returns>
        static string CmdExit(ConsoleREPL sender, CommandEventArgs e)
        {
            if (e.Help) // Check if help called it
                return ""; // No extra help.
            e.Quit = true; // Exit
            return "";
        }
        /// <summary>
        /// Echos the statement
        /// </summary>
        /// <param name="sender">Caller</param>
        /// <param name="e">Args</param>
        /// <returns>Echo'd statement</returns>
        static string CmdEcho(ConsoleREPL sender, CommandEventArgs e)
        {
            if (e.Help || e.Arguments.Length < 3)
                return "";
            return e.Arguments[0];
        }
        /// <summary>
        /// A test to demonstrate command-line like argument listing
        /// </summary>
        /// <param name="sender">ConsoleREPL window that called the function</param>
        /// <param name="e">Arguments</param>
        /// <returns></returns>
        static string CmdArgTest(ConsoleREPL sender, CommandEventArgs e)
        {
            if (e.Help) // Check if run from help
                return ""; // No extra help info
            string ret = "";
            // Loop through and print arguments
            for (int i = 0; i < e.Arguments.Length; i++)
                ret += "args[" + i + "]: " + e.Arguments[i] + "\n";
            return ret;
        }
        /// <summary>
        /// Window functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        static string CmdWindow(ConsoleREPL sender, CommandEventArgs e)
        {
            if(e.Help) // Check if called from help
                return ""; // No extra help
            if (e.Arguments.Length == 3) // If there's an argument
            {
                int newWindow = 0;
                if (int.TryParse(e.Arguments[2], out newWindow)) // Check if they used a number
                    return ConsoleREPL.SwitchWindow(newWindow) ? "Switch Successful!" : "Switch Unsuccessful"; // They did, indicate result
                else // They didn't use a number, check names
                    for (int i = 0; i < ConsoleREPL.WindowList.Length; i++)
                        if (e.Arguments[2] == ConsoleREPL.WindowList[i].WindowName) // If this is the window they specified
                            return ConsoleREPL.SwitchWindow(i) ? "Switch Successful!" : "Switch Unsuccessful"; // Indicate result
                return "Invalid window\n" + sender.ShowHelp(e.Arguments[1]); // Window doesn't exist
            }
            else // No arguments, print window list
            {
                string ret = "Window list:\n";
                ConsoleREPL[] windows = ConsoleREPL.WindowList; // Get list
                for (int i = 0; i < windows.Length; i++) // Iterate through
                    ret += (i == ConsoleREPL.WindowId ? "=" : " ") + i + ": " + windows[i].WindowName + "\n"; // Print each window
                return ret;
            }
        }
        /// <summary>
        /// Adds a window
        /// </summary>
        /// <param name="sender">Caller</param>
        /// <param name="e">Args</param>
        /// <returns>String indicating success</returns>
        static string CmdAddWindow(ConsoleREPL sender, CommandEventArgs e)
        {
            if (e.Help) // Check if it was run via help
                return "";
            if (e.Arguments.Length != 3) // Make sure it has the right number of arguments
                return sender.ShowHelp(e.Arguments[1]);
            ConsoleREPL window = new ConsoleREPL(ConsoleEval, e.Arguments[2]); // Create a new window
            window.AddCommand("addwindow", "[window name]", "Adds a new window with name [window name]", CmdAddWindow); // Add addwindow to the commands
            window.AddCommand("argtest", "(args)", "Displays argument information", CmdArgTest); // Add argtest to the commands
            window.AddCommand("echo", "[string]", "Echos [string] to the console", CmdEcho); // Add echo to the commands
            window.AddCommand("exit", "", "Exits the window", CmdExit); // Add exit to the commands
            window.AddCommand("quit", "", "Exits the window", CmdExit); // Add quit to the commands
            window.AddCommand("window", "[window]", "Switches to a different window", CmdWindow); // Add window to the commands
            return ConsoleREPL.AddWindow(window) ? "Added window!" : "Adding window failed"; // Add the window and return the result
        }
    }
}
