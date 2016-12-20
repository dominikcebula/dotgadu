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
using System.Collections.Generic;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu operacji na liscie uzytkownikow
    /// </summary>
    public class GaduUserListException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu operacji na liscie uzytkownikow
        /// </summary>
        public GaduUserListException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Zarzadza lista konaktow na serwerze
    /// </summary>
    public class GaduUserList : List<GaduUser>
    {
        /// <summary>
        /// Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Konstruktor GaduUserList
        /// </summary>
        /// <param name="gaduSender">Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu</param>
        public GaduUserList(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Przesyla cala liste na serwer
        /// </summary>
        public void sendAll()
        {
            try
            {
                GaduPacketHeader gaduPacketHeader;
                GaduPacketUserListRequest gaduPacketUserlistRequest;
                for (int i = 0; i < this.Count; i++)
                {
                    gaduPacketHeader = new GaduPacketHeader();
                    gaduPacketUserlistRequest = new GaduPacketUserListRequest();

                    gaduPacketHeader.Type = GaduPacketConstans.GG_USERLIST_REQUEST;

                    gaduPacketUserlistRequest.Type = (i == 0 ? GaduPacketConstans.GG_USERLIST_PUT : GaduPacketConstans.GG_USERLIST_PUT_MORE);
                    gaduPacketUserlistRequest.Request = this[i].getUserListString();

                    gaduPacketHeader.Length = gaduPacketUserlistRequest.getSize();

                    gaduSender.sendPacket(gaduPacketHeader);
                    gaduSender.sendPacket(gaduPacketUserlistRequest);
                }
            }
            catch (Exception exp)
            {
                throw new GaduUserListException(exp.Message);
            }
        }

        /// <summary>
        /// Wysyla pakiet do servera, ktory sprawia ze ten przesle nam liste kontaktow
        /// </summary>
        public void getListFromServer()
        {
            try
            {
                GaduPacketHeader gaduPacketHeader = new GaduPacketHeader();
                GaduPacketUserListRequest gaduPacketUserlistRequest = new GaduPacketUserListRequest();

                gaduPacketHeader.Type = GaduPacketConstans.GG_USERLIST_REQUEST;
                gaduPacketHeader.Length = 1;
                gaduPacketUserlistRequest.Type = GaduPacketConstans.GG_USERLIST_GET;

                gaduSender.sendPacket(gaduPacketHeader);
                gaduSender.sendPacket(gaduPacketUserlistRequest);
            }
            catch (Exception exp)
            {
                throw new GaduUserListException(exp.Message);
            }
        }
    }
}
