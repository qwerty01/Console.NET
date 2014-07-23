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
    #region Delegates
    /// <summary>
    /// Delegate for eval loop
    /// </summary>
    /// <param name="sender">REPL window that called the function</param>
    /// <param name="e">Event args relating to the loop</param>
    /// <returns>String to print (usually when a command hasn't been found)</returns>
    public delegate string LoopDelegate(ConsoleREPL sender, LoopEventArgs e);
    #endregion
    public class ConsoleREPL
    {
        #region Static Variables
        // Public Variables
        /// <summary>
        /// Current active window
        /// (used for multi-window support)
        /// </summary>
        public static int WindowId { get; protected set; }
        /// <summary>
        /// Gets the currently active window
        /// </summary>
        public static ConsoleREPL CurrentWindow { get { return windowList[WindowId]; } }
        /// <summary>
        /// List of windows
        /// </summary>
        public static ConsoleREPL[] WindowList { get { return windowList.ToArray(); } }

        // Private Varialbes
        /// <summary>
        /// List of windows
        /// </summary>
        protected static List<ConsoleREPL> windowList;
        #endregion

        #region Instance Variables
        // Public Variables
        /// <summary>
        /// Called every eval
        /// </summary>
        public LoopDelegate RunEval { get; set; }
        /// <summary>
        /// Prompt used for input
        /// Default: "> "
        /// </summary>
        public string ReadPrompt { get; set; }
        /// <summary>
        /// True if commands should be checked with case sensitivity
        /// Default: false
        /// </summary>
        public bool CaseSensitiveCmd { get; set; }
        /// <summary>
        /// Whether or not empty commands should be evaluated
        /// Default: False
        /// </summary>
        public bool ParseEmptyCommand { get; set; }
        /// <summary>
        /// Whether or not the REPL window should handle help
        /// Default: True
        /// </summary>
        public bool HandleHelp { get; set; }
        /// <summary>
        /// Window name
        /// </summary>
        public string WindowName { get; set; }
        
        //Private Variables
        /// <summary>
        /// List of commands this window handles
        /// </summary>
        protected List<REPLCommand> commands;
        /// <summary>
        /// Whether or not the console should be cleared
        /// </summary>
        protected bool clearRequired;
        #endregion

        #region Static Functions
        /// <summary>
        /// Initializes variables
        /// </summary>
        static ConsoleREPL()
        {
            windowList = new List<ConsoleREPL>();
        }

        /// <summary>
        /// Hooks the console window
        /// </summary>
        /// <param name="window">REPL window to bind to the console</param>
        public static void HookConsole()
        {
            LoopEventArgs e = new LoopEventArgs(); // Main loop event arguments
            while (windowList.Count > 0) // While eval says to continue and the window list isn't empty
            {
                e.Continue = true;
                ConsoleREPL active = CurrentWindow; // Current active window (if window is switched in eval, print will not be handled properly if "CurrentWindow" is used)
                if (active.clearRequired)
                    Console.Clear();
                active.clearRequired = false;
                active.Read(e); // Read using the current window
                if (!active.ParseEmptyCommand && e.Arguments[1] == "") // Ignore empty commands if specified
                    continue;
                active.Print(
                    active.Eval(e) // Evaluate the input
                    ); // Print the results from eval
                if (!e.Continue)
                {
                    RemoveWindow(active);
                    Console.Clear();
                }
            }
        }
        /// <summary>
        /// Adds the current window to the list and uses it to hook the console
        /// </summary>
        /// <param name="window">Window to add</param>
        public static void HookConsole(ConsoleREPL window)
        {
            windowList.Add(window); // Add the specified window
            WindowId = windowList.Count - 1;
            HookConsole(); // Hook the console
        }

        /// <summary>
        /// Switch the currently active window
        /// </summary>
        /// <param name="windowNum">Window to switch to</param>
        /// <returns>True if switch was successful</returns>
        public static bool SwitchWindow(int windowNum)
        {
            if (windowNum < 0) return false; // windowNum is too small
            if (windowNum >= windowList.Count) return false; // windowNum is too large
            CurrentWindow.clearRequired = true; // Clear the old window
            WindowId = windowNum; // windowNum is just right
            CurrentWindow.clearRequired = true; // clear the new window
            return true;
        }
        /// <summary>
        /// Adds a new window to the window list
        /// </summary>
        /// <param name="window">Window to add</param>
        /// <returns>True if adding was successful</returns>
        public static bool AddWindow(ConsoleREPL window)
        {
            if (windowList.Any(w => w.WindowName == window.WindowName))
                return false;
            windowList.Add(window); // Add the window
            return true;
        }
        /// <summary>
        /// Removes a window from the list
        /// </summary>
        /// <param name="window">Window to remove</param>
        /// <returns>True if removal was successful</returns>
        public static bool RemoveWindow(ConsoleREPL window)
        {
            if (WindowId >= (windowList.Count - 1))
                WindowId--; // Update the window ID if necessary
            return windowList.Remove(window); // Try to remove the window
        }
        /// <summary>
        /// Removes a window from the list
        /// </summary>
        /// <param name="index">Index to remove</param>
        /// <returns>True if removal was successful</returns>
        public static bool RemoveWindow(int index)
        {
            if (index < 0 || index >= windowList.Count) return false; // index isn't valid
            if (WindowId >= (windowList.Count - 1))
                WindowId--; // Update the window ID if necessary
            windowList.RemoveAt(index); // Remove at the index
            return true; // Successful
        }
        #endregion

        #region Instance Functions
        /// <summary>
        /// Create a new repl window
        /// Defaults:
        /// Prompt: " >"
        /// CaseSensitiveCmd: False
        /// ParseEmptyCommand: False
        /// HandleHelp: True
        /// 
        /// Note:
        /// eval is run prior to any processing, set e.Parse to false to prevent further processing
        /// </summary>
        /// <param name="eval">Function that is run every eval</param>
        /// <param name="windowName">Name of the window</param>
        /// <param name="prompt">Prompt to use for input</param>
        /// <param name="caseSensitiveCmd">Whether or not commands are case sensitive</param>
        /// <param name="parseEmptyCommand">Whether or not to handle commands that are empty</param>
        /// <param name="handleHelp">Whether or not to handle help</param>
        public ConsoleREPL(LoopDelegate eval, string windowName = "", string prompt = "> ", bool caseSensitiveCmd = false, bool parseEmptyCommand = false, bool handleHelp = true)
        {
            ReadPrompt = prompt;
            RunEval = eval;
            commands = new List<REPLCommand>();
            CaseSensitiveCmd = caseSensitiveCmd;
            ParseEmptyCommand = parseEmptyCommand;
            HandleHelp = handleHelp;
            WindowName = windowName;
        }

        /// <summary>
        /// Read from input
        /// </summary>
        /// <param name="e">Loop arguments</param>
        public void Read(LoopEventArgs e)
        {
            Console.Write(ReadPrompt); // Write the prompt
            e.SetArguments(Console.ReadLine()); // Read the input and set the arguments accordingly
        }

        /// <summary>
        /// Eval portion
        /// </summary>
        /// <param name="e">Loop arguments</param>
        /// <returns>String to print</returns>
        public string Eval(LoopEventArgs e)
        {
            e.Parse = true; // Reset parse
            string ret = RunEval(this, e); // Run user-defined eval
            if (e.Parse) // Check if they want us to handle anything
                if (HandleHelp && e.Arguments[1] == "help") // Handle help
                    return ShowHelp(e);
                else
                    foreach (REPLCommand c in commands) // Check which command was called
                        if ((CaseSensitiveCmd ? e.Arguments[1] : e.Arguments[1].ToLower()) == (CaseSensitiveCmd ? c.Name : c.Name.ToLower())) // If it's the current command
                        {
                            CommandEventArgs ee = new CommandEventArgs(e.Arguments); // create the arguments
                            ret = c.Callback(this, ee); // Call the command
                            e.Continue = !ee.Quit; // Exit if function says so
                            return ret; // print the function
                        }
            return ret; // Return text from RunEval
        }

        /// <summary>
        /// Print portion
        /// </summary>
        /// <param name="output">output to print</param>
        public void Print(string output)
        {
            Console.WriteLine(output);
        }

        /// <summary>
        /// Adds a command to the command list
        /// 
        /// Help Notes:
        /// Usage Standard Format:
        /// [required argument 1] (optional command 2) etc.
        /// 
        /// Displays:
        /// Usage: command [required argument 1] (optional command 2)
        /// Description: [description]
        /// </summary>
        /// <param name="command">Command to add</param>
        /// <param name="usage">Usage info for help</param>
        /// <param name="description">Description for help</param>
        /// <param name="callback">Function associated with command</param>
        /// <returns>True if command was added successfully</returns>
        public bool AddCommand(string command, string usage, string description, REPLCommand.CommandDelegate callback)
        {
            foreach (REPLCommand c in commands) // Check through all the commands...
                if (c.Name == command) // ...to see if it already exists
                    return false; // Already exists, add wasn't successful
            commands.Add(new REPLCommand(command, usage, description, callback, this)); // Add the command
            return true; // Successful
        }
        /// <summary>
        /// Removes a command from the list
        /// </summary>
        /// <param name="command">Command to remove</param>
        /// <returns>True if removal was successful</returns>
        public bool RemoveCommand(string command)
        {
            foreach (REPLCommand c in commands) // Check through the commands...
                if (c.Name == command) // ...to make sure it exists
                {
                    commands.Remove(c); // Remove it
                    return true; // Removal successful
                }
            return false; // Command isn't in list
        }
        /// <summary>
        /// Shows help from the given arguments
        /// </summary>
        /// <param name="e">Loop args</param>
        /// <returns>Help info</returns>
        public string ShowHelp(LoopEventArgs e)
        {
            if (e.Arguments[1] != "help")
                return "";
            string ret = "";
            string cmdList = "";
            if (e.Arguments.Length == 2) // help without arguments
            {
                ret = "Help:\nUsage: " + e.Arguments[1] + " (command)\n\nList of available commands:\n";
                foreach (REPLCommand c in commands) // Generate list of commands
                    ret += c.Name + "\n";
                return ret; // Return help
            }
            foreach (REPLCommand c in commands) // Check to see which command was asked about
            {
                if (c.Name == e.Arguments[2]) // Check if the help request is the current command
                {
                    ret = "Usage: " + e.Arguments[2] + " " + c.Usage + "\nDescription: " + c.Description + "\n"; // Display usage and description
                    CommandEventArgs ee = new CommandEventArgs(e.Arguments, true); // Setup command args
                    ret += c.Callback(this, ee); // Call the function in case it wants to add anything to the help text
                    e.Continue = !ee.Quit; // Exit if the function says so (would be kinda strange for a help call)
                    return ret;
                }
                cmdList += c.Name + "\n"; // Compile a list in case the command isn't found
            }
            return "Command \"" + e.Arguments[2] + "\" not found.\nAvailable commands:\n" + cmdList; // Display list of commands
        }
        /// <summary>
        /// Shows help from the given arguments
        /// </summary>
        /// <param name="cmd">Arguments</param>
        /// <returns>Help info</returns>
        public string ShowHelp(string cmd)
        {
            LoopEventArgs e = new LoopEventArgs();
            e.SetArguments("help "+cmd);
            return ShowHelp(e);
        }
        #endregion
    }
}
