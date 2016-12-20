// DotGadu - example8
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using System.Drawing;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            GaduHttp gaduHttp = new GaduHttp();
            String tokenid;
            Image token;
            gaduHttp.getToken(out tokenid, out token);
            token.Save("token.gif");

            try
            {
                String tokenval;
                int uin;

                Console.WriteLine("Odczytana wartosc z tokenu:");
                tokenval = Console.ReadLine();

                Console.WriteLine("Numer gg:");
                uin = Convert.ToInt32(Console.ReadLine());

                gaduHttp.remindPassword(uin, tokenid, tokenval);
                Console.WriteLine("Haslo wyslane poprawnie");
            }
            catch (GaduRemindException e)
            {
                Console.WriteLine("Blad wysylania hasla " + e.Message);
            }
        }
    }
}

