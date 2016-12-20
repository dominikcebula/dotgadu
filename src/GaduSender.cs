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
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu wysylania pakietu
    /// </summary>
    public class GaduSenderException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu wysylania pakietu
        /// </summary>
        public GaduSenderException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnPacketSent
    /// </summary>
    /// <param name="packet">
    /// Pakiet, ktory zostal wyslany
    /// </param>
    public delegate void OnPacketSentHandler(IGaduPacket packet);

    /// <summary>
    /// Zajmuje sie kolejkowaniem i wysylaniem pakietow do servera Gadu-Gadu
    /// </summary>
    public class GaduSender
    {
        /// <summary>
        /// Event wywolany kiedy klasa wysle jakis pakiet
        /// </summary>
        public event OnPacketSentHandler OnPacketSent;
        /// <summary>
        /// Watek ktory jest tworzony kiedy kolejka jest pusta, wysyla pakiety do servera gg
        /// </summary>
        private Thread thread;
        /// <summary>
        /// Gniazdo podlaczone do servera gg
        /// </summary>
        private TcpClient tcpClient;
        /// <summary>
        /// Kolejka pakietow do wyslania
        /// </summary>
        private Queue queue;
        /// <summary>
        /// Semafor mowiacy o tym czy watek dziala czy nie
        /// </summary>
        private int sleepTime;

        /// <summary>
        /// Wykorzystywany kiedy gniazdo nie jest podlaczone, watek sprawdza co
        /// sleepTime czy gniazdo jest juz podlaczone
        /// </summary>
        public int SleepTime
        {
            get
            {
                return sleepTime;
            }
            set
            {
                sleepTime = value;
            }
        }

        /// <summary>
        /// Konstruktor klasy, inicjalizuje kolejke, gniazdo
        /// </summary>
        /// <param name="tcpClient">
        /// Gniazdo na ktorym jestesmy podlaczeni do servera gg
        /// </param>
        public GaduSender(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            queue = new Queue();
            sleepTime = 250;
            thread = new Thread(new ThreadStart(go));	// to utworz watek
            thread.Start();
        }

        /// <summary>
        /// Czeka na zakonczenie watku
        /// </summary>
        public void Terminate()
        {
            thread.Abort();
            queue.Clear();
        }

        /// <summary>
        /// Kolejkuje pakiet do wyslania
        /// </summary>
        /// <param name="packet">
        /// Pakiet do wyslania
        /// </param>
        public void sendPacket(IGaduPacket packet)
        {
            queue.Enqueue(packet);	// zakolejkuj pakiet
        }

        /// <summary>
        /// Funkcja dzialajaca na rzecz watku, pobiera i wysla pakiety z kolejki
        /// </summary>
        private void go()
        {
            try
            {
                IGaduPacket packet;	// pakiet do wyslania
                while (true)	// do poki kolejka nie jest pusa
                {
                    if (!tcpClient.Connected || queue.Count == 0)   // czy gniazdo jest podlaczone i czy mamy jakies pakiety do wyslania
                    {
                        Thread.Sleep(sleepTime);	// jezeli nie to czekaj az bedzie podlaczone
                        continue;
                    }
                    packet = (IGaduPacket)queue.Dequeue();	// pobierz pakiet z kolejki
                    packet.write(tcpClient.GetStream());	// wyslij pakiet
                    if (OnPacketSent != null)
                        OnPacketSent.BeginInvoke(packet, null, null);
                }
            }
            catch (ThreadAbortException)
            {	// nie zadreczajmy uzytkownika tym wyjatkiem, w 99% powodowany przez Thread.Abort()
            }
            catch (Exception e)	// jezeli cokolwiek poszlo nie tak
            {
                if (Gadu.GaduCriticalError() == false)
                    throw new GaduRecieverException(e.Message);
            }
        }
    }
}
