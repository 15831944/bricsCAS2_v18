using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS.myUtilities.myString
{
    /// <summary>
    /// Inkrementieren eines Strings
    /// </summary>
    public enum Mode
    {
        AlphaNumeric = 1,
        Alpha = 2,
        Numeric = 3
    }

    public class MyString
    {
        public static string Increment(string text, Mode mode)
        {
            bool isDigit = int.TryParse(text, out int zahl);

            if (isDigit)
            {
                zahl += 1;
                return zahl.ToString();
            }
            else
            {
                var textArr = text.ToCharArray();

                // Add legal characters
                var characters = new List<char>();

                if (mode == Mode.AlphaNumeric || mode == Mode.Numeric)
                    for (char c = '0'; c <= '9'; c++)
                        characters.Add(c);

                if (mode == Mode.AlphaNumeric || mode == Mode.Alpha)
                    for (char c = 'a'; c <= 'z'; c++)
                        characters.Add(c);

                // Loop from end to beginning
                for (int i = textArr.Length - 1; i >= 0; i--)
                {
                    if (textArr[i] == characters.Last())
                    {
                        textArr[i] = characters.First();
                    }
                    else
                    {
                        textArr[i] = characters[characters.IndexOf(textArr[i]) + 1];
                        break;
                    }
                }

                return new string(textArr);
            }
        }

        //Zahlenstring
        public static int Precision(string String)
        {
            //',' gegen '.' tauschen
            String = String.Replace(',', '.');

            if (!String.Contains("."))
                return 0;

            String = String.Substring(String.IndexOf('.') + 1);
            string Nachkomma = String.Empty;

            foreach (char ch in String)
            {
                if (ch >= 48 && (int)ch <= 57)
                    Nachkomma += ch;
            }

            if (Nachkomma == "0")
                Nachkomma = String.Empty;

            return Nachkomma.Length;
        }

        //Formatstring
        public static string Formatstring(int Precision)
        {
            string Format = "0.";

            for (int i = 0; i < Precision; i++)
                Format += "0";

            return Format;
        }
    }
}

