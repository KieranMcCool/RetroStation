//The MIT License (MIT)
//
//Copyright (c) 2015 Kieran McCool

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using System.IO;

namespace RetroStation
{
    class ControllerInput
    {
        public ControllerInput()
        {
            readActions();
            Task.Factory.StartNew(mainLoop);
        }

        Dictionary<string, string[]> Actions = new Dictionary<string, string[]>();
        bool running = true;

        public delegate void ButtonPressedDelegate(string[] actions, string[] buttonsPressed);
        public event ButtonPressedDelegate ButtonPressed;

        private void readActions()
        {
            string path = Environment.CurrentDirectory + @"\Resources\Controls.cfg";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var lineArray = sr.ReadLine().Split(':');
                        Actions.Add(lineArray[0], lineArray[1].Split(','));
                    }
                }
            }
        }

        private void writeActions()
        {
            string path = Environment.CurrentDirectory + @"/Resources/Controls.cfg";
            using (StreamWriter sw = new StreamWriter(path))
                foreach(var v in Actions)
                    sw.WriteLine(v.Key + ":" + string.Join(",", v.Value));
        }

        private void mainLoop()
        {
            State gameState;
            var Controller = new Controller(UserIndex.One);
            while (running)
            {
                Controller.GetState(out gameState);
                string strState = gameState.Gamepad.Buttons.ToString().Replace(" ", "");
                if (strState != "None")
                {
                    ButtonPressed(getActions(strState), strState.Split(','));
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        private string[] getActions(string strState)
        {
            List<string> satisfied = new List<string>();
            foreach (var a in Actions)
                if (satisfiesAction(strState.Split(','), a.Value))
                    satisfied.Add(a.Key);
            return satisfied.ToArray();
        }   

        private bool satisfiesAction(string[] buttons, string[] action)
        {
            foreach (var b in action)
                if (!buttons.Contains(b))
                    return false;
            return true;
        }
    }
}
