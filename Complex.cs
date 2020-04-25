using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KULUMBA_Francis_TDK
{
    class Complex
    {
        public double a; //Partie réelle
        public double b; //Partie imaginaire
        //z = a + jb

        public Complex(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public void JCarre()
        {
            double temporaire = (a * a) - (b * b);
            b = 2 * a * b;
            a = temporaire;
        }
        
        public double JModule()
        {
            return Math.Sqrt((a * a) + (b * b));
        }

        public void JAddtion(Complex z)
        {
            a += z.a;
            b += z.b;
        }

    }
}
