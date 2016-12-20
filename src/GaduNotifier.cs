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
using System.Text;
using System.Collections.Generic;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu typu gg_notify
    /// </summary>
    public class GaduNotifierException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu typu gg_notify
        /// </summary>
        public GaduNotifierException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa realizujaca operacje powiadomien o uzytkownikach na liscie
    /// </summary>
    public class GaduNotifier : List<GaduPacketNotify>
    {
        /// <summary>
        /// Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Konstruktor GaduNotifier
        /// </summary>
        /// <param name="gaduSender">Referencja do klasy wyslajaca pakiety do servera Gadu-Gadu</param>
        public GaduNotifier(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
        }

        /// <summary>
        /// Dodawanie numerku w czasie pracy
        /// </summary>
        /// <param name="gaduPacketNotify">Numer do dodania reprezentowany przez pakiet GaduPacketNotify</param>
        public void addNotify(GaduPacketNotify gaduPacketNotify)
        {
            GaduPacketHeader gaduPacketHeader = new GaduPacketHeader(); // naglowek
            gaduPacketHeader.Type = GaduPacketConstans.GG_ADD_NOTIFY;   // ustawiamy odpowiedni typ pakiety
            gaduPacketHeader.Length = gaduPacketNotify.getSize();   // rozmiar pakiety
            gaduSender.sendPacket(gaduPacketHeader);    // wysylamy naglowek
            gaduSender.sendPacket(gaduPacketNotify);    // wysylamy pakiet
        }

        /// <summary>
        /// Usuwanie numerku w czasie pracy
        /// </summary>
        /// <param name="gaduPacketNotify">Numer do usuniecia reprezentowany przez pakiet GaduPacketNotify</param>
        public void delNotify(GaduPacketNotify gaduPacketNotify)
        {
            GaduPacketHeader gaduPacketHeader = new GaduPacketHeader(); // naglowek
            gaduPacketHeader.Type = GaduPacketConstans.GG_REMOVE_NOTIFY;    // ustawiamy odpowiedni typ pakiety
            gaduPacketHeader.Length = gaduPacketNotify.getSize();       // rozmiar pakiety
            gaduSender.sendPacket(gaduPacketHeader);        // wysylamy naglowek
            gaduSender.sendPacket(gaduPacketNotify);        // wysylamy pakiet
        }

        /// <summary>
        /// Wysyla wszystkie pakiety na server
        /// </summary>
        public void sendNotify()
        {
            try
            {
                if (this.Count == 0)    // jezeli nasza lista jest pusta
                {
                    GaduPacketHeader header = new GaduPacketHeader();
                    header.Type = GaduPacketConstans.GG_LIST_EMPTY; // wysylamy tylko pakiet typu GG_LIST_EMPTY
                    header.Length = 0;
                    gaduSender.sendPacket(header);
                }
                else if (this.Count < 400) // jezeli lista ma mniej niz 400 wpisow, nie musimy jej dzielic
                {
                    GaduPacketHeader header = new GaduPacketHeader();
                    header.Type = GaduPacketConstans.GG_NOTIFY_LAST;    // pierwszy pakiet wysylamy jako ostatni
                    header.Length = this.Count * this[0].getSize(); // rozmiar = suma rozmiarow wszystkich pakietow, a wiec wysylamy
                    gaduSender.sendPacket(header);                  // wszystko jako jeden zlepek
                    for (int i = 0; i < this.Count; i++)        // wysylamy kolejne pakiety
                        gaduSender.sendPacket(this[i]);
                }
                else    // w tym trybie mamy wiecej niz 400 pakietow, musimy dzielic je po 400 wpisow
                {
                    GaduPacketHeader header;
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (i % 400 == 0 && i + 400 >= this.Count)  // jezeli to juz ostatnie 400 pakietow
                        {
                            header = new GaduPacketHeader();
                            header.Type = GaduPacketConstans.GG_NOTIFY_LAST;    // wyslij pakiety jako ostatni zlepek
                            header.Length = (this.Count - i) * this[0].getSize();
                            gaduSender.sendPacket(header);
                        }
                        else if (i % 400 == 0)  // co 400 pakietow trzeba wyslac nowy naglowek
                        {
                            header = new GaduPacketHeader();
                            header.Type = GaduPacketConstans.GG_NOTIFY_FIRST;
                            header.Length = 400 * this[0].getSize();
                            gaduSender.sendPacket(header);
                        }
                        gaduSender.sendPacket(this[i]); // wysylamy dany pakiet
                    }
                }
            }
            catch (Exception exp)
            {
                throw new GaduNotifierException(exp.Message);
            }
        }
    }
}
