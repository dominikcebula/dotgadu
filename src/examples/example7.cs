// DotGadu - example7
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
                String email, OldPass, NewPass;

                Console.WriteLine("Odczytana wartosc z tokenu:");
                tokenval = Console.ReadLine();

                Console.WriteLine("Numer gg:");
                uin = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("E-Mail:");
                email = Console.ReadLine();
                Console.WriteLine("Stare haslo:");
                OldPass = Console.ReadLine();
                Console.WriteLine("Nowe haslo:");
                NewPass = Console.ReadLine();

                gaduHttp.changePassword(uin, email, OldPass, NewPass, tokenid, tokenval);
                Console.WriteLine("Haslo zmienione poprawnie");
            }
            catch (GaduChangePasswordException e)
            {
                Console.WriteLine("Blad zmiany hasla " + e.Message);
            }
        }
    }
}
