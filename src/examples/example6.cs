// DotGadu - example6
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
                String passwd;
                Console.WriteLine("Odczytana wartosc z tokenu:");
                tokenval = Console.ReadLine();
                Console.WriteLine("Numer gg do usuniecia:");
                uin = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Haslo:");
                passwd = Console.ReadLine();
                gaduHttp.deleteAccount(uin, passwd, tokenid, tokenval);
                Console.WriteLine("Konto usuniete poprawnie");
            }
            catch (GaduDeleteException e)
            {
                Console.WriteLine("Blad usuwania konta: " + e.Message);
            }
        }
    }
}

