using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KULUMBA_Francis_TDK
{
    class Pixel
    {
        public int R;
        public int G;
        public int B;

        public Pixel(int R, int G, int B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public int Moyenne()
        {
            return (R + G + B) / 3;
        }
        
        public string toString()
        {
            string value = "";
            value += R;
            if (R < 10) value += "  ; ";
            if (R < 100 && R >= 10) value += " ; ";
            else if (R >= 100) value += "; ";

            value += G;
            if (G < 10) value += "  ; ";
            if (G < 100 && G >= 10) value += " ; ";
            else if (G >= 100) value += "; ";

            value += B;

            return value;
        }
    }
}
