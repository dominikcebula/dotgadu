/*
 * DotGadu 2007/2008
 * WWW: http://dotgadu.sf.net/
 * E-MAIL: dominikcebula@gmail.com
 *
 * Projekt biblioteki, ktorej celem jest obsluga protokolu GaduGadu z poziomu .NET. Opis protokolu zostal
 * zaczerpniety z libgadu, mozna go znalezc tutaj: http://ekg.chmurka.net/docs/protocol.html
*/

using System;
using System.Threading;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu pingowania servera
    /// </summary>
    public class GaduPingerException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu pingowania servera
        /// </summary>
        public GaduPingerException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Nalezy podstawic pod event OnPing
    /// </summary>
    public delegate void OnPingHandler();

    /// <summary>
    /// Klasa, ktora co 2 min. wysla pakiet ping do servera Gadu-Gadu
    /// </summary>
    public class GaduPinger
    {
        /// <summary>
        /// Watek, ktory co 2 min.wysyla pakiet ping do servera Gadu-Gadu
        /// </summary>
        private Thread thread;
        /// <summary>
        /// Za pomoca tej klasy wyslamy pakiety do servera Gadu-Gadu
        /// </summary>
        private GaduSender gaduSender;

        /// <summary>
        /// Event wywolywany podczas wysylania pakietu ping do servera Gadu-Gadu
        /// </summary>
        public event OnPingHandler OnPing;

        /// <summary>
        /// Konstruktor klasy, tworzy watek zajmujacy sie pingowaniem servera
        /// </summary>
        /// <param name="gaduSender">
        /// Referencja do klasy GaduSender, za pomoca tej klasy bedziemy wysylac pakiety
        /// </param>
        public GaduPinger(GaduSender gaduSender)
        {
            this.gaduSender = gaduSender;
            thread = new Thread(new ThreadStart(go));
            thread.Start();
        }

        /// <summary>
        /// Zakoncz dzialanie watku
        /// </summary>
        public void Terminate()
        {
            thread.Abort();
        }

        /// <summary>
        /// Funkcja dzialajaca na rzecz watku
        /// </summary>
        private void go()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2 * 60 * 1000);	// odczekaj 2 min
                    GaduPacketHeader header = new GaduPacketHeader();	// przesylamy tylko naglowek
                    header.Type = GaduPacketConstans.GG_PING;		// typu GG_PING
                    header.Length = 0;	// po naglowku nic nie idzie, rozmiar 0
                    gaduSender.sendPacket(header);	// wyslij pakiet
                    if (OnPing != null)	// jezeli ktos podstawil jakas delegacje pod event
                        OnPing.BeginInvoke(null, null);	// to ja wywolaj
                }
            }
            catch (ThreadAbortException)
            {	// nie zadreczajmy uzytkownika tym wyjatkiem, w 99% powodowany przez Thread.Abort();
            }
            catch (Exception e)
            {	// jezeli cokolwiek poszlo nie tak
                if (Gadu.GaduCriticalError() == false)
                    throw new GaduPingerException(e.Message);
            }
        }
    }
}
