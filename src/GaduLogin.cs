/*
 * DotGadu 2007/2008
 * WWW: http://dotgadu.sf.net/
 * E-MAIL: dominikcebula@gmail.com
 *
 * Projekt biblioteki, ktorej celem jest obsluga protokolu GaduGadu z poziomu .NET. Opis protokolu zostal
 * zaczerpniety z libgadu, mozna go znalezc tutaj: http://ekg.chmurka.net/docs/protocol.html
*/

using System;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu w logowaniu
    /// </summary>
    public class GaduLoginException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu w logowaniu
        /// </summary>
        public GaduLoginException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa udostepniajaca mechanizm logowania
    /// </summary>
    public class GaduLogin
    {
        /// <summary>
        /// Ziarno pobrane z servera Gadu-Gadu, potrzebne do wyliczenia hashu hasla
        /// </summary>
        private uint seed;
        /// <summary>
        /// Za pomoca tej klasy wyslylamy pakiety
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Glowny konstruktor klasy GaduLogin
        /// </summary>
        /// <param name="seed">
        /// Ziarno pobrane z servera Gadu-Gadu
        /// </param>
        /// <param name="gaduSender">
        /// Klasa GaduSender za pomoca, ktorej bedziemy wysylac pakiety
        /// </param>
        public GaduLogin(uint seed, GaduSender gaduSender)
        {
            this.seed = seed;
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Logowanie do gg
        /// </summary>
        /// <param name="uin">Numer gg</param>
        /// <param name="pass">Haslo</param>
        public void login(int uin, String pass)
        {
            try
            {
                GaduPacketHeader header = new GaduPacketHeader();   // naglowek
                GaduPacketLogin loginPacket = new GaduPacketLogin();    // pakiet logowania
                header.Type = GaduPacketConstans.GG_LOGIN70;    // bedziemy sie logowac
                header.Length = loginPacket.getSize();  // dlugosc pakietu
                loginPacket.Uin = uin;      // nr. gg
                loginPacket.Hash = BitConverter.GetBytes(getHash(pass, seed));   // szyfrujemy haslo, potem konwertujemy na tablice bajtow
                gaduSender.sendPacket(header);	// najpierw wpisujemy naglowek
                gaduSender.sendPacket(loginPacket);	// teraz wpiszemy wlasciwy pakiet
            }
            catch (Exception e)
            {
                throw new GaduLoginException(e.Message);
            }
        }

        /// <summary>
        /// Funkcja hashuje haslo za pomoca SHA1
        /// </summary>
        /// <param name="password">Haslo do zaszyfrowania</param>
        /// <returns>Hash SHA1</returns>
        private byte[] getHash(String password)
        {
            byte[] pass = Encoding.UTF8.GetBytes(password); // pobieramy nasze haslo w bajtach, przeliczamy UFT8->cp1250
            byte[] hash;
            HashAlgorithm hashAlg = new SHA512Managed();    // inicjalizacja SHA1
            hash = hashAlg.ComputeHash(pass);   // liczymy hash
            return hash;    // zwracamy gotowy hash
        }

        /// <summary>
        /// Funkcja hashuje haslo algorytmem gadu-gadu
        /// </summary>
        /// <param name="password">Haslo do zaszyfrowania</param>
        /// <param name="seed">Ziarno pobrane z server gadu-gadu</param>
        /// <returns>Hash hasla</returns>
        private uint getHash(String password, uint seed)
        {
            uint x, y, z;

            y = seed;

            int p = 0;
            for (x = 0; p < password.Length; p++)
            {
                x = (x & 0xffffff00) | Convert.ToByte(password[p]);
                y ^= x;
                y += x;
                x <<= 8;
                y ^= x;
                x <<= 8;
                y -= x;
                x <<= 8;
                y ^= x;

                z = y & 0x1f;
                y = (uint)((uint)y << (int)z) | (uint)((uint)y >> (int)(32 - z));
            }
            return y;
        }
    }
}
