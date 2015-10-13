using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace EmulationStation
{
    class InputWrapper
    {
        public InputWrapper()
        {
            System.Windows.Forms.MessageBox.Show(
            numberConnected().ToString());
        }

        private int numberConnected() {
            int connected = 0;
            for (int i = 0; i <= 10; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                    connected++;
            }
            return connected;
        }

        private string[] buttonArray(GamePadState g)
        {
            if (g.Buttons.A == ButtonState.Pressed) ;
                
        }
    }
}
