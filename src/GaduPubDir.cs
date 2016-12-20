/*
 * DotGadu 2007/2008
 * WWW: http://dotgadu.sf.net/
 * E-MAIL: dominikcebula@gmail.com
 *
 * Projekt biblioteki, ktorej celem jest obsluga protokolu GaduGadu z poziomu .NET. Opis protokolu zostal
 * zaczerpniety z libgadu, mozna go znalezc tutaj: http://ekg.chmurka.net/docs/protocol.html
*/

using System;
using System.IO;
using System.Net.Sockets;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu dzialania na katalogu publicznym
    /// </summary>
    public class GaduPubDirException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu dzialania na katalogu publicznym
        /// </summary>
        public GaduPubDirException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa realizujaca operacje na katalogu publicznym
    /// </summary>
    public class GaduPubDir
    {
        /// <summary>
        /// Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Konstruktor GaduPubDir
        /// </summary>
        /// <param name="gaduSender">Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu</param>
        public GaduPubDir(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Czyta dane z katalogu publicznego dotyczace naszego konta, wystarczy ze w GaduUser
        /// wypelnimy tylko pole Uin
        /// </summary>
        /// <param name="gaduUser">Nasze dane</param>
        public void Read(GaduUser gaduUser)
        {
            GaduPacketHeader gaduPacketHeader = new GaduPacketHeader();
            GaduPacketPubDir50 gaduPacketPubDir50 = new GaduPacketPubDir50();
            gaduPacketHeader.Type = GaduPacketConstans.GG_PUBDIR50_REQUEST;
            gaduPacketPubDir50.Type = GaduPacketConstans.GG_PUBDIR50_READ;
            gaduPacketPubDir50.Seq = (int)DateTime.Now.Ticks;
            gaduPacketPubDir50.Data = gaduUser.getPubDirString();
            gaduPacketHeader.Length = gaduPacketPubDir50.getSize();
            gaduSender.sendPacket(gaduPacketHeader);
            gaduSender.sendPacket(gaduPacketPubDir50);
        }

        /// <summary>
        /// Zapisuje informacje do katalogu publicznego
        /// </summary>
        /// <param name="gaduUser">Nasze dane</param>
        public void Write(GaduUser gaduUser)
        {
            GaduPacketHeader gaduPacketHeader = new GaduPacketHeader();
            GaduPacketPubDir50 gaduPacketPubDir50 = new GaduPacketPubDir50();
            gaduPacketHeader.Type = GaduPacketConstans.GG_PUBDIR50_REQUEST;
            gaduPacketPubDir50.Type = GaduPacketConstans.GG_PUBDIR50_WRITE;
            gaduPacketPubDir50.Seq = (int)DateTime.Now.Ticks;
            gaduPacketPubDir50.Data = gaduUser.getPubDirString();
            gaduPacketHeader.Length = gaduPacketPubDir50.getSize();
            gaduSender.sendPacket(gaduPacketHeader);
            gaduSender.sendPacket(gaduPacketPubDir50);
        }

        /// <summary>
        /// Wysyla do servera zapytanie o uzytkownika
        /// </summary>
        /// <param name="gaduUser">Uzytkownik, ktorego chcemy znalezc</param>
        public void Search(GaduUser gaduUser)
        {
            GaduPacketHeader gaduPacketHeader = new GaduPacketHeader();
            GaduPacketPubDir50 gaduPacketPubDir50 = new GaduPacketPubDir50();
            gaduPacketHeader.Type = GaduPacketConstans.GG_PUBDIR50_REQUEST;
            gaduPacketPubDir50.Type = GaduPacketConstans.GG_PUBDIR50_SEARCH;
            gaduPacketPubDir50.Seq = (int)DateTime.Now.Ticks;
            gaduPacketPubDir50.Data = gaduUser.getPubDirString();
            gaduPacketHeader.Length = gaduPacketPubDir50.getSize();
            gaduSender.sendPacket(gaduPacketHeader);
            gaduSender.sendPacket(gaduPacketPubDir50);
        }
    }
}
