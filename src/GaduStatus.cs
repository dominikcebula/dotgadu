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

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu zmiany statusu
    /// </summary>
    public class GaduStatusException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu zmiany statusu
        /// </summary>
        public GaduStatusException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa odpowiedzialna za zmiane statusu uzytkownika gg
    /// </summary>
    public class GaduStatus
    {
        /// <summary>
        /// Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Konstruktor GaduStatus
        /// </summary>
        /// <param name="gaduSender">Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu</param>
        public GaduStatus(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Zmienia status uzytkownika gg
        /// </summary>
        /// <param name="statusid">Numer statusu nalezy pobrac z tad GaduPacketConstans.GG_STATUS_*</param>
        /// <param name="descr">Opis, jezeli ustawilismy status, ktory nie jest typu *_DESCR, dajemy pusty string</param>
        public void changeStatus(int statusid, string descr)
        {
            try
            {
                GaduPacketHeader header = new GaduPacketHeader();   // naglowek
                GaduPacketStatus status = new GaduPacketStatus();   // pakiet zmieniajacy status
                header.Type = GaduPacketConstans.GG_NEW_STATUS;     // typ pakietu to zmiana status
                status.Status = statusid;   // przypisujemy status id
                status.Description = descr;    // narazie nie mamy zadnego opisu
                header.Length = status.getSize();   // pobieramy rozmiar pakietu
                gaduSender.sendPacket(header);    // wysylamy naglowek
                gaduSender.sendPacket(status);    // wysylamy wlasciwiy pakiet
            }
            catch (Exception exp)
            {
                throw new GaduStatusException(exp.Message);
            }
        }
    }
}
