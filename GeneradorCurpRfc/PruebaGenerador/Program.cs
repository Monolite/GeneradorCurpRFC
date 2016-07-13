using GeneradorCurpRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaGenerador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
            Generador.GenerarCURP(
                "González"
                , "Reyes"
                , "José Augusto"
                , "H"
                , new DateTime(1985, 08, 23)
                , "DF"
            ));

            Console.WriteLine(
            Generador.GenerarRFC(
                "González"
                , "Reyes"
                , "José Augusto"
                //, "H"
                , new DateTime(1985, 08, 23)
                //, "DF"
            ));
        }
    }
}
