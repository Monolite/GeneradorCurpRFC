using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneradorCurpRfc
{
    public class Generador
    {
        protected readonly Dictionary<char, char> ReemplazoCaracteres = new Dictionary<char, char>()
        {
            { 'Ñ', 'X' }, { '.', 'X' }, { ',', 'X' }, { ';', 'X' },
            { '/', 'X' }, { '-', 'X' }, { 'Á', 'A' }, { 'Ä', 'A' },
            { 'À', 'A' }, { 'Â', 'A' }, { 'É', 'E' }, { 'Ë', 'E' },
            { 'È', 'E' }, { 'Ê', 'E' }, { 'Í', 'I' }, { 'Ï', 'I' },
            { 'Ì', 'I' }, { 'Î', 'I' }, { 'Ó', 'O' }, { 'Ö', 'O' },
            { 'Ò', 'O' }, { 'Ô', 'O' }, { 'Ú', 'U' }, { 'Ü', 'U' },
            { 'Ù', 'U' }, { 'Û', 'U' }

        };

        protected readonly Dictionary<string, string> ReemplazoPalabrasObsenas = new Dictionary<string, string>()
        {
            { "BACA", "BXCA"},  { "BAKA", "BXKA" },  { "BUEI", "BXEI"}, { "BUEY", "BXEY" },
            { "CACA", "CXCA" }, { "CACO", "CXCO" }, { "CAGO", "CXGO" }, { "CAKA", "CXKA" },
            { "CAKO", "CXKO" }, { "COGE", "CXGE" }, { "COGI", "CXGI" }, { "COJA", "CXJA" },
            { "COJE", "CXJE" }, { "COJI", "CXJI" }, { "COJO", "CXJO" }, { "COLA", "CXLA" },
            { "CULO", "CXLO" }, { "FALO", "FXLO" }, { "FETO", "FXTO" }, { "GUEI", "GXEI" },
            { "GUEY", "GXEY" }, { "JETA", "JXTA" }, { "JOTO", "JXTO" }, { "KACA", "KXCA" },
            { "KACO", "KXCO" }, { "KAGA", "KXGA" }, { "KAGO", "KXGO" }, { "KAKA", "KXKA" },
            { "KAKO", "KXKO" }, { "KOGE", "KXGE" }, { "KOGI", "KXGI" }, { "KOJA", "KXJA" },
            { "KOJE", "KXJE" }, { "KOJI", "KXJI" }, { "KOJO", "KXJO" }, { "KOLA", "KXLA" },
            { "KULO", "KXLO" }, { "LILO", "LXLO" }, { "LOCA", "LXCA" }, { "LOCO", "LXCO" },
            { "LOKA", "LXKA" }, { "MAME", "MXME" }, { "MAMO", "MXMO" }, { "MEAR", "MXAR" },
            { "MEAS", "MXAS" }, { "MEON", "MXON" }, { "MIAR", "MXAR" }, { "MION", "MXON" },
            { "MOCO", "MXCO" }, { "MOKO", "MXKO" }, { "MULA", "MXLA" }, { "MULO", "MXLO" },
            { "NACA", "NXCA" }, { "NACO", "NXCO" }, { "PEDA", "PXDA" }, { "PEDO", "PXDO" },
            { "PENE", "PXNE" }, { "PIPI", "PXPI" }, { "PITO", "PXTO" }, { "POPO", "PXPO" },
            { "PUTA", "PXTA" }, { "PUTO", "PXTO" }, { "QULO", "QXLO" }, { "RATA", "RXTA" },
            { "ROBA", "RXBA" }, { "ROBE", "RXBE" }, { "ROBO", "RXBO" }, { "RUIN", "RXIN" },
            { "SENO", "SXNO" }, { "TETA", "TXTA" }, { "VACA", "VXCA" }, { "VAGA", "VXGA" },
            { "VAGO", "VXGO" }, { "VAKA", "VXKA" }, { "VUEI", "VXEI" }, { "VUEY", "VXEY" },
            { "WUEI", "WXEI" }, { "WUEY", "WXEY" }
        };

        protected readonly List<string> OmisionPalabras = new List<string>
        {
            "DA",  "DAS", "DE",  "DEL", "DER",
            "DI",  "DIE", "DD",  "EL",  "LA",
            "LOS", "LAS", "LE",  "LES", "MAC",
            "MC",  "VAN", "VON", "Y"
        };

        protected readonly List<string> Entidades = new List<string>
        {
            "AS", "BC", "BS", "CC", "CL", "CM", "CS", "CH",
            "DF", "DG", "GT", "GR", "HG", "JC", "MC", "MN",
            "MS", "NT", "NL", "OC", "PL", "QT", "QR", "SP",
            "SL", "SR", "TC", "TS", "TL", "VZ", "YN", "ZS"
        };

        protected readonly char[] Vocales = { 'A', 'E', 'I', 'O', 'U' };

        protected readonly string[] OmisionPrimerNombre = { "MARIA", "MA.", "MA", "JOSE", "J", "J." };

        public static string GenerarCURP(string PrimerApellido, string SegundoApellido, string Nombres, string Sexo,
                                      DateTime FechaNacimiento, string EntidadNacimiento)
        {
            Generador bg = new Generador();

            // limpiar los datos
            PrimerApellido = bg.ProcesarCadena(PrimerApellido);
            SegundoApellido = bg.ProcesarCadena(SegundoApellido);
            Nombres = bg.ProcesarCadena(Nombres);

            if (Sexo != "H" && Sexo != "M")
                throw new InvalidOperationException("Sexo debe de ser H (hombre) o M (mujer)");

            if (!bg.Entidades.Contains(EntidadNacimiento))
                throw new InvalidOperationException("Entidad de nacimiento inválida");

            // generar Curp
            StringBuilder curp = new StringBuilder();

            //Posición 0
            curp.Append(bg.PrimeraLetra(PrimerApellido));

            //Posición 1
            curp.Append(bg.PrimeraVocalInterna(PrimerApellido));

            //Posición 2
            curp.Append(bg.PrimeraLetra(SegundoApellido));

            //Posición 3
            curp.Append(bg.PrimeraLetra(bg.SegundoNombre(Nombres)));

            //Posición 4 - 9 
            curp.Append(FechaNacimiento.ToString("yyMMdd"));

            //Posición 10
            curp.Append(Sexo);

            //Posición 11, 12
            curp.Append(EntidadNacimiento);

            //Posición 13
            curp.Append(bg.PrimeraConsonante(PrimerApellido.Substring(1)));
            //Posición 14
            curp.Append(bg.PrimeraConsonante(SegundoApellido.Substring(1)));
            //Posición 15
            curp.Append(bg.PrimeraConsonante(bg.SegundoNombre(Nombres).Substring(1)));

            //Posición 16
            curp.Append("0");

            //curp.Append(random.nextInt(10));

            string curp_res = curp.ToString();

            string verif_curp = curp_res.Substring(0, 4);

            if (bg.ReemplazoPalabrasObsenas.ContainsKey(verif_curp))
            {
                string reemp = bg.ReemplazoPalabrasObsenas[verif_curp];

                curp_res = curp_res.Replace(verif_curp, reemp);
            }

            return curp_res;
        }

        public static string GenerarRFC(string PrimerApellido, string SegundoApellido, string Nombres, DateTime FechaNacimiento)
        {
            return GenerarCURP(PrimerApellido, SegundoApellido, Nombres, "H", FechaNacimiento, "DF").Substring(0, 10);
        }

        protected string ProcesarCadena(string s)
        {
            string res = s.Trim().ToUpper();

            string expr_omisiones = string.Join("|", OmisionPalabras);

            Regex regex_omisiones = new Regex(string.Format(@"\b({0})\b", expr_omisiones));

            res = regex_omisiones.Replace(res, "").Trim();

            Regex regex_espacios = new Regex("[ ]{2,}");

            res = regex_espacios.Replace(res, " ");


            for (int i = 0; i < res.Length; i++)
            {
                if (ReemplazoCaracteres.ContainsKey(res[i]))
                {
                    res = res.Replace(res[i], ReemplazoCaracteres[res[i]]);
                }
            }

            return res;
        }

        protected char PrimeraLetra(string s)
        {
            char? r = s.ToCharArray().FirstOrDefault();

            return r ?? 'X';
        }

        protected char PrimeraConsonante(string s)
        {
            char? r = s.ToCharArray().Where(c => !Vocales.Contains(c)).FirstOrDefault();

            return r ?? 'X';
        }

        protected char SegundaConsonante(string s)
        {
            char? r = s.ToCharArray().Where(c => !Vocales.Contains(c)).Skip(1).FirstOrDefault();

            return r ?? 'X';
        }

        protected char PrimeraVocalInterna(string s)
        {
            int i = s.IndexOfAny(Vocales);

            if (i >= 0)
            {
                return s[i];
            }
            return 'X';
        }

        protected string SegundoNombre(string s)
        {
            string[] arr = s.Split(' ');

            if (arr.Length > 1)
            {
                if (OmisionPrimerNombre.Contains(arr[0]))
                    return arr[1];
            }

            return s;
        }

    }
}
