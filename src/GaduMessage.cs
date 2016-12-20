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
    /// Wyjatek zwracany w przypadku bledu podczas wysylania wiadomosci
    /// </summary>
    public class GaduMessageException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu podczas wysylania wiadomosci
        /// </summary>
        public GaduMessageException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Sluzy do wysylania wiadomosci
    /// </summary>
    public class GaduMessage
    {
        /// <summary>
        /// Klasa za pomoca, ktorej bedziemy wysylac wiadomosci
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Konsruktor GaduMessage
        /// </summary>
        /// <param name="gaduSender">
        /// Referencja do klasu GaduSender za pomoca, ktorej bedziemy wysylac pakiety
        /// </param>
        public GaduMessage(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Wysyla wiadomosc pod dany numer
        /// </summary>
        /// <param name="uin">Numer odbiorcy</param>
        /// <param name="msg">Wiadomosc</param>
        public void sendMessage(int uin, string msg)
        {
            try
            {
                GaduPacketHeader header = new GaduPacketHeader();   // naglowek
                GaduPacketMessage message = new GaduPacketMessage();    //wiadomosc
                header.Type = GaduPacketConstans.GG_SEND_MSG;   // bedziemy wysylac wiadomosc
                message.Recepient = uin;  // numer odbiorcy
                message.Klasa = GaduPacketConstans.GG_CLASS_MSG; // klasa wiadomosci
                message.Message = msg;  // wiadomosc, string zostanie zamieniony na ciag bajtow
                header.Length = message.getSize();   // wpisujemy wielkosc pakietu
                gaduSender.sendPacket(header);    // przesylamy najpierw naglowek
                gaduSender.sendPacket(message);   // potem przesylamy wiadomosc
            }
            catch (Exception exp)
            {
                throw new GaduMessageException(exp.Message);
            }
        }
    }
}
