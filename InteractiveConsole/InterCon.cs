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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace InteractiveConsole
{
    public class InterCon
    {
        public delegate void InputDelegate(InterCon sender, InputEventArgs e);
        public static string Title { get { return Console.Title; } set { Console.Title = value; } }
        public static Size WindowSize { 
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
            set
            {
                Console.BufferWidth = value.Width;
                Console.BufferHeight = value.Height;
                Console.WindowWidth = value.Width;
                Console.WindowHeight = value.Height;
            }
        }
        public static int WindowWidth { get { return Console.WindowWidth; } set { Console.WindowWidth = value; } }
        public static int WindowHeight { get { return Console.WindowHeight; } set { Console.WindowHeight = value; } }

        public TextReader In { get; protected set; }
        public TextWriter Out { get; protected set; }
        public TextWriter Err { get; protected set; }
        public ConsoleKeyInfo FlushKey { get; set; }
        public ConsoleKeyInfo HistoryUp { get; set; }
        public ConsoleKeyInfo HistoryDown { get; set; }
        public ConsoleKeyInfo LeftKey { get; set; }
        public ConsoleKeyInfo RightKey { get; set; }
        public ConsoleKeyInfo EscapeKey { get; set; }
        public ConsoleKeyInfo ScrollUp { get; set; }
        public ConsoleKeyInfo ScrollDown { get; set; }
        public ConsoleKeyInfo Backspace { get; set; }
        public ConsoleKeyInfo Delete { get; set; }
        public ConsoleKeyInfo Insert { get; set; }
        public bool Active { get; protected set; }
        public string Status { get { return status; } set { status = value; statusChanged = true; } }

        protected string currInput { get { return inputHistory[inputIndex]; } set { inputHistory[inputIndex] = value; } }

        protected ConsoleEventArgs consoleEvent;
        protected InputDelegate inputCallback;
        protected MemoryStream inStream;
        protected MemoryStream outStream;
        protected MemoryStream errStream;
        protected TextWriter inWriter;
        protected TextReader outReader;
        protected TextReader errReader;
        protected List<string> inputHistory;
        protected List<string> outputHistory;
        protected string status;
        protected bool inputChanged;
        protected bool outputChanged;
        protected bool statusChanged;
        protected bool autoScroll;
        protected int inputIndex; // index of input history
        protected int inputCursor; // location of cursor
        protected int inputVisible; // index of visible part of input
        protected int outputIndex;

        public InterCon(InputDelegate inputCallback)
        {
            inStream = new MemoryStream();
            outStream = new MemoryStream();
            errStream = new MemoryStream();

            inWriter = new StreamWriter(inStream);
            outReader = new StreamReader(outStream);
            errReader = new StreamReader(errStream);

            In = new StreamReader(inStream);
            Out = new StreamWriter(outStream);
            Err = new StreamWriter(errStream);

            FlushKey = new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false);
            HistoryUp = new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false);
            HistoryDown = new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false);
            LeftKey = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
            RightKey = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
            EscapeKey = new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);
            ScrollUp = new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, false);
            ScrollDown = new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false);
            Backspace = new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false);
            Insert = new ConsoleKeyInfo('\0', ConsoleKey.Insert, false, false, false);
            Delete = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);

            inputHistory = new List<string>();
            outputHistory = new List<string>();
            inputHistory.Add("");

            this.inputCallback = inputCallback;

            inputIndex = 0;
            inputCursor = 0;
            inputVisible = 0;
            outputIndex = 0;

            inputChanged = true;
            outputChanged = true;
            statusChanged = true;
            autoScroll = true;
            Active = true;
            Status = "";
            consoleEvent = new ConsoleEventArgs();
        }

        public void Hook()
        {
            Console.BufferWidth = WindowWidth;
            Console.BufferHeight = WindowHeight;
            while(consoleEvent.Continue)
            {
                handleInput();
                handleOutput();
                Thread.Sleep(1);
            }
        }

        public void Wait()
        {
            while (consoleEvent.Continue)
                Thread.Sleep(100);
        }

        protected void handleInput()
        {
            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                if (k.Key == FlushKey.Key && k.Modifiers == FlushKey.Modifiers)
                {
                    inWriter.WriteLine(inputHistory[0]);
                    inWriter.Flush();
                    inputHistory.Insert(0, "");
                    inputVisible = 0;
                    inputCursor = 0;
                    inputChanged = true;
                }
                else if (k.Key == HistoryUp.Key && k.Modifiers == HistoryUp.Modifiers)
                {
                    inputCursor = 0;
                    inputIndex = inputIndex == inputHistory.Count - 1 ? inputHistory.Count - 1 : inputIndex + 1;
                    inputChanged = true;
                }
                else if (k.Key == HistoryDown.Key && k.Modifiers == HistoryDown.Modifiers)
                {
                    inputCursor = 0;
                    inputIndex = inputIndex == 0 ? 0 : inputIndex - 1;
                    inputChanged = true;
                }
                else if(k.Key == LeftKey.Key && k.Modifiers == LeftKey.Modifiers)
                {
                    inputCursor = inputCursor == 0 ? 0 : inputCursor - 1;
                    if (inputCursor - inputVisible == 1)
                        inputVisible = inputVisible - WindowWidth / 4 >= 0 ? inputVisible - WindowWidth / 4 : 0;
                    inputChanged = true;
                }
                else if(k.Key == RightKey.Key && k.Modifiers == RightKey.Modifiers)
                {
                    inputCursor = inputCursor == currInput.Length ? currInput.Length : inputCursor + 1;
                    if (inputCursor - inputVisible == WindowWidth - 2)
                        inputVisible += WindowWidth / 4;
                    inputChanged = true;
                }
                else if(k.Key == EscapeKey.Key && k.Modifiers == EscapeKey.Modifiers)
                {
                    inputIndex = 0;
                    inputCursor = 0;
                    currInput = "";
                    inputChanged = true;
                }
                else if(k.Key == ScrollUp.Key && k.Modifiers == ScrollUp.Modifiers)
                {
                    outputIndex = outputIndex <= 0 ? 0 : outputIndex - 1;
                    outputChanged = true;
                    autoScroll = false;
                }
                else if(k.Key == ScrollDown.Key && k.Modifiers == ScrollDown.Modifiers)
                {
                    outputIndex = outputIndex >= outputHistory.Count - 1 ? outputHistory.Count - 1 : outputIndex + 1;
                    outputChanged = true;
                    if (outputIndex == outputHistory.Count - 1)
                        autoScroll = true;
                }
                else if(k.Key == Backspace.Key && k.Modifiers == Backspace.Modifiers)
                {
                    if (inputCursor != 0)
                    {
                        currInput = currInput.Substring(0, --inputCursor) + currInput.Substring(inputCursor + 1);
                        if (inputCursor - inputVisible == 1)
                            inputVisible = inputVisible - WindowWidth / 4 >= 0 ? inputVisible - WindowWidth / 4 : 0;
                        inputChanged = true;
                    }
                }
                else if(k.Key == Delete.Key && k.Modifiers == Delete.Modifiers)
                {
                    if(inputCursor != currInput.Length)
                    {
                        currInput = currInput.Substring(0, inputCursor) + currInput.Substring(inputCursor + 1);
                        inputChanged = true;
                    }
                }
                else
                {
                    currInput = currInput.Substring(0, inputCursor) + k.KeyChar + currInput.Substring(inputCursor);
                    inputCursor++;
                    if (inputCursor - inputVisible == WindowWidth - 2)
                        inputVisible += WindowWidth / 4;
                    inputChanged = true;
                }
            }
        }

        protected void handleOutput()
        {
            string output = outReader.ReadToEnd();
            string error = errReader.ReadToEnd();
            if(output.Length != 0 || error.Length != 0)
            {
                outputChanged = true;
                foreach (string s in output.Split('\n'))
                {
                    outputHistory.Add(s);
                    if(autoScroll && outputIndex < outputHistory.Count - (WindowHeight - 2))
                        outputIndex = outputHistory.Count - (WindowHeight - 2);
                }
                foreach(string s in error.Split('\n'))
                {
                    outputHistory.Add(s);
                    if (autoScroll && outputIndex < outputHistory.Count - (WindowHeight - 2))
                        outputIndex = outputHistory.Count - (WindowHeight - 2);
                }
            }

            if(outputChanged)
            {
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                for (Console.CursorTop = 0; Console.CursorTop < WindowHeight - 2; Console.CursorTop++, Console.CursorLeft = 0)
                    if (outputHistory.Count > Console.CursorTop + outputIndex)
                        Console.Write(fillInput(outputHistory[Console.CursorTop + outputIndex]));
                    else
                        Console.Write(fillInput(""));
                outputChanged = false;
            }
            if(statusChanged)
            {
                Console.CursorTop = WindowHeight - 2;
                Console.CursorLeft = 0;
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(fillInput(status));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                statusChanged = false;
            }
            if(inputChanged)
            {
                Console.CursorTop = WindowHeight - 1;
                Console.CursorLeft = 0;
                string cInput = inputVisible + WindowWidth > currInput.Length ? fillInput(currInput.Substring(inputVisible)) : currInput.Substring(inputVisible, WindowWidth);
                for (int i = 0; i < WindowWidth - 1; i++)
                    if (inputVisible + i == inputCursor)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(currInput.Length > inputVisible + i ? currInput[inputVisible + i] : ' ');
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write(currInput.Length > inputVisible + i ? currInput[inputVisible + i] : ' ');
                inputChanged = false;
            }
        }

        private string fillInput(string input)
        {
            string ret = input;
            if (ret.Length < WindowWidth)
                for (int i = ret.Length; i < WindowWidth; i++)
                    ret += " ";
            return ret;
        }
    }
}
