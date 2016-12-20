// DotGadu - example5
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
                uin = gaduHttp.registerAccount("passwd", "mail@mail.com", tokenid, tokenval);
                Console.WriteLine("Konto zarejestrowane poprawnie, twoj numer to: " + uin);
            }
            catch (GaduRegisterException e)
            {
                Console.WriteLine("Blad rejestorwania konta: " + e.Message);
            }
        }
    }
}
