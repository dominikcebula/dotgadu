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

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu odbierania pakietu
    /// </summary>
    public class GaduRecieverException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu odbierania pakietu
        /// </summary>
        public GaduRecieverException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnRecieveMessage
    /// </summary>
    /// <param name="msg">Otrzymana Wiadomosc</param>
    public delegate void OnRecieveMessageHandler(GaduPacketRecieveMessage msg);
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnRecieveMessageAck
    /// </summary>
    /// <param name="ack">
    /// Pakiet, ktory jest potwierdzeniem wyslanej wiadomosci
    /// </param>
    public delegate void OnRecieveMessageAckHandler(GaduPacketMessageAck ack);
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnLoginOK
    /// </summary>
    public delegate void OnLoginOKHandler();
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnLoginNeedEmail
    /// </summary>
    public delegate void OnLoginNeedEmailHandler();
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnLoginFailed
    /// </summary>
    public delegate void OnLoginFailedHandler();
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnPong
    /// </summary>
    public delegate void OnPongHandler();
    /// <summary>
    /// Delegacja, ktora nalezy podstawic pod event OnUnhandledPacket
    /// </summary>
    /// <param name="header">Naglowek nieobsluzonego pakietu</param>
    /// <param name="packet">Pakiet w bajtach</param>
    public delegate void OnUnhandledPackethandler(GaduPacketHeader header, byte[] packet);
    /// <summary>
    /// Otrzymalismy pakiet z informacja o statusie ktoregos uzytkownika na naszej liscie
    /// </summary>
    /// <param name="gaduPacketNotifyReplay77">Pakiet z informacjami o statusie</param>
    public delegate void OnGaduNotifyReplayHandler(GaduPacketNotifyReplay77 gaduPacketNotifyReplay77);
    /// <summary>
    /// Ktos zmienil status
    /// </summary>
    /// <param name="gaduPacketStatus77">Pakiet z informacjami o nowym statusie</param>
    public delegate void OnGaduStatusHandler(GaduPacketStatus77 gaduPacketStatus77);
    /// <summary>
    /// Otrzymalismy jakis pakiet
    /// </summary>
    /// <param name="gaduPacketHeader">Naglowek pakietu</param>
    public delegate void OnPacketHandler(GaduPacketHeader gaduPacketHeader);
    /// <summary>
    /// Otrzymalismy odpowiedz tyczaca sie listy uzytkownikow
    /// </summary>
    /// <param name="gaduPacketUserlistReplay">Pakiet zawierajacy informacje dotyczace uzytkownika na naszej liscie</param>
    public delegate void OnUserListReplayHandler(GaduPacketUserListRequest gaduPacketUserlistReplay);
    /// <summary>
    /// Odpowiedz z servera - katalog publiczny
    /// </summary>
    /// <param name="gaduPacketPubDir50">Pakiet zawierajacy odpowiedz servera</param>
    public delegate void OnPubDirReplayHandler(GaduPacketPubDir50 gaduPacketPubDir50);
    /// <summary>
    /// Server GG chce nas rozlaczyc
    /// </summary>
    public delegate void OnGaduDisconnectingHandler();

    /// <summary>
    /// Klasa zajmuje sie odbieraniem pakietow z servera Gadu-Gadu
    /// </summary>
    public class GaduReciever
    {
        /// <summary>
        /// watek ktory zajmie sie odbieraniem pakietow
        /// </summary>
        private Thread thread;
        /// <summary>
        /// gniazdo z ktorego bedziemy czytac
        /// </summary>
        private TcpClient tcpClient;
        /// <summary>
        /// Wskazuje co ile watek ma sprawdzac czy na gniezdzie sa dane
        /// </summary>
        private int sleepTime;

        /// <summary>
        /// Event, ktory zostaje wywolany kiedy otrzymamy wiadomosc
        /// </summary>
        public event OnRecieveMessageHandler OnRecieveMessage;
        /// <summary>
        /// Event, ktory zostaje wywolany kiedy otrzymamy potwierdzenie wyslanej wiadomosci
        /// </summary>
        public event OnRecieveMessageAckHandler OnRecieveMessageAck;
        /// <summary>
        /// Event, ktory zostaje wywolany kiedy logowanie sie powiedzie
        /// </summary>
        public event OnLoginOKHandler OnLoginOK;
        /// <summary>
        /// Event, ktory zostaje wywolany kiedy musimy uzupelnic email na naszym koncie
        /// </summary>
        public event OnLoginNeedEmailHandler OnLoginNeedEmail;
        /// <summary>
        /// Event, ktory zostaje wywolany kiedy logownie sie nie powiedzie
        /// </summary>
        public event OnLoginFailedHandler OnLoginFailed;
        /// <summary>
        /// Event, ktory zostaje wywolany kiedy otrzymamy pakiet GG_PONG z servera
        /// </summary>
        public event OnPongHandler OnPong;
        /// <summary>
        /// Event, ktory zostaje wywolany jezeli biblioteka wychwyci pakiet nie przechwycony
        /// </summary>
        public event OnUnhandledPackethandler OnUnhandledPacket;
        /// <summary>
        /// Otrzymalismy pakiet z informacja o statusie ktoregos uzytkownika na naszej liscie
        /// </summary>
        public event OnGaduNotifyReplayHandler OnGaduNotifyReplay;
        /// <summary>
        /// Ktos z naszej listy zmienil status
        /// </summary>
        public event OnGaduStatusHandler OnGaduStatus;
        /// <summary>
        /// Otrzymalismy jakis pakiet
        /// </summary>
        public event OnPacketHandler OnPacket;
        /// <summary>
        /// Otrzymalismy odpowiedz tyczaca sie listy uzytkownikow
        /// </summary>
        public event OnUserListReplayHandler OnUserListReplay;
        /// <summary>
        /// Odpowiedz z servera - katalog publiczny
        /// </summary>
        public event OnPubDirReplayHandler OnPubDirReplay;
        /// <summary>
        /// Server GG chce nas rozlaczyc
        /// </summary>
        public event OnGaduDisconnectingHandler OnGaduDisconnecting;
        /// <summary>
        /// Ta zmienna blokuje wykonywanie sie watku odbierajacego pakiety, wskazuje czy pracowac czy nie.
        /// Jest wykorzystana w Gadu.cs w metodzie Connect()
        /// </summary>
        public bool work = false;

        /// <summary>
        /// Wskazuje co ile watek ma sprawdzac czy na gniezdzie sa dane
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
        /// Konstruktor GaduReciever
        /// </summary>
        /// <param name="tcpClient">gniazdo podlaczone do servera</param>
        public GaduReciever(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            thread = new Thread(new ThreadStart(go));   // towrzymy nowy watek
            sleepTime = 250;    // sprawdzanie czy cos jest na gniezdzie co 250 ms wystarczy
            thread.Start(); // uruchamiamy watek
        }

        /// <summary>
        /// Przerwanie dzialania watku, ktory odbiera pakiety
        /// </summary>
        public void Terminate()
        {
            thread.Abort();    // zakoncz watek
        }

        /// <summary>
        /// Zajmuje sie sprawdzaniem czy cos jest na gniezdzie, wczytywaniem naglowka
        /// oraz calych pakietow
        /// </summary>
        private void go()
        {
            try
            {
                GaduPacketHeader header = new GaduPacketHeader();   // naglowek
                while (true)
                {
                    if (!tcpClient.Connected || tcpClient.GetStream().DataAvailable == false || work == false)
                    { // jezeli na gniezdzie nie ma 8 pelnych bajtow lub gniazdo nie jest podlaczone to
                        Thread.Sleep(sleepTime);	// czekamy na nowe pakiety
                        continue;   // i przechodzimy do nastepnej iteracji
                    }
                    header.read(tcpClient.GetStream(), 8);  // czytamy naglowek
                    if (OnPacket != null)
                        OnPacket.BeginInvoke(header, null, null);
                    switch (header.Type)
                    {
                        case GaduPacketConstans.GG_RECV_MSG:    // ktos przyslal nam wiadomosc
                            GaduPacketRecieveMessage msg = new GaduPacketRecieveMessage();
                            msg.read(tcpClient.GetStream(), header.Length); // wczytujemy wiadomosc
                            if (OnRecieveMessage != null) // jezeli ktos podstawil swoja funkcje pod event
                                OnRecieveMessage.BeginInvoke(msg, null, null);  // to ja wywolujemy podstawiajac jako argument pakiet z wiadomoscia
                            break;
                        case GaduPacketConstans.GG_SEND_MSG_ACK:	// odebralismy pakiet potwierdzajacy wiadomosc
                            GaduPacketMessageAck ack = new GaduPacketMessageAck();
                            ack.read(tcpClient.GetStream(), header.Length);	// wczytaj pakiet
                            if (OnRecieveMessageAck != null)
                                OnRecieveMessageAck.BeginInvoke(ack, null, null);	// wywolaj event
                            break;
                        case GaduPacketConstans.GG_PONG:	// otrzymalismy pakiet GG_PONG z servera
                            if (OnPong != null)
                                OnPong.BeginInvoke(null, null);
                            break;
                        case GaduPacketConstans.GG_LOGIN_OK:	// otrzymalismy pakiet informujacy nas o tym ze logowanie sie powiodlo
                            if (header.Length == 1)
                                tcpClient.GetStream().ReadByte();	// jeden bajt, ktory nie zawsze musi sie pojawic, przeznaczenie nie poznane
                            if (OnLoginOK != null)
                                OnLoginOK.BeginInvoke(null, null);	// wywolaj event
                            break;
                        case GaduPacketConstans.GG_NEED_EMAIL:	// server chce abysmy uzupelnili e-mail w naszych danych
                            if (OnLoginNeedEmail != null)
                                OnLoginNeedEmail.BeginInvoke(null, null);	// wywolaj event
                            break;
                        case GaduPacketConstans.GG_LOGIN_FAILED:	// logowanie sie nie powiodlo
                            if (OnLoginFailed != null)
                                OnLoginFailed.BeginInvoke(null, null);	// wywolaj event
                            break;
                        case GaduPacketConstans.GG_NOTIFY_REPLY77:  // otrzymalismy pakiet mowiacy o tym ze ktos zmienil status
                            GaduPacketNotifyReplay77 gaduPacketNotifyReplay77;
                            while (header.Length > 0)   // najprawdopodobnie bedzie to zlepek pakietow, a wiec bedziemy je wczytywac po kolei
                            {
                                gaduPacketNotifyReplay77 = new GaduPacketNotifyReplay77();
                                gaduPacketNotifyReplay77.read(tcpClient.GetStream(), header.Length);
                                header.Length -= gaduPacketNotifyReplay77.getSize();
                                if (OnGaduNotifyReplay != null)
                                    OnGaduNotifyReplay.BeginInvoke(gaduPacketNotifyReplay77, null, null);   // dla danego pakiety wywolaj event
                            }
                            break;
                        case GaduPacketConstans.GG_STATUS77:    // ktos zmienil status na gg
                            GaduPacketStatus77 gaduPacketStatus77 = new GaduPacketStatus77();
                            gaduPacketStatus77.read(tcpClient.GetStream(), header.Length);  // wczytujemy pakiet
                            if (OnGaduStatus != null)
                                OnGaduStatus.BeginInvoke(gaduPacketStatus77, null, null);   // wywolujemny event
                            break;
                        case GaduPacketConstans.GG_USERLIST_REPLY:
                            GaduPacketUserListRequest gaduPacketUserlistReplay = new GaduPacketUserListRequest();
                            gaduPacketUserlistReplay.read(tcpClient.GetStream(), header.Length);
                            if (OnUserListReplay != null)
                                OnUserListReplay.BeginInvoke(gaduPacketUserlistReplay, null, null);
                            break;
                        case GaduPacketConstans.GG_PUBDIR50_REPLY:
                            GaduPacketPubDir50 gaduPacketPubDir50 = new GaduPacketPubDir50();
                            gaduPacketPubDir50.read(tcpClient.GetStream(), header.Length);
                            if (OnPubDirReplay != null)
                                OnPubDirReplay.BeginInvoke(gaduPacketPubDir50, null, null);
                            break;
                        case GaduPacketConstans.GG_DISCONNECTING:
                            if (OnGaduDisconnecting != null)
                                OnGaduDisconnecting.BeginInvoke(null, null);
                            break;
                        case GaduPacketConstans.GG_DISCONNECTING2:
                            if (OnGaduDisconnecting != null)
                                OnGaduDisconnecting.BeginInvoke(null, null);
                            break;
                        default:	// jezeli otrzymalismy jakis pakiet, ktory jeszcze nie jest obslugiwany przez biblioteke
                            byte[] packet = new byte[header.Length];
                            for (int i = 0; i < header.Length; i++)	// to wczytaj tylko pakiet z gniazda i nic z nim nie rob
                                packet[i] = (byte)tcpClient.GetStream().ReadByte();
                            if (OnUnhandledPacket != null)
                                OnUnhandledPacket.BeginInvoke(header, packet, null, null);
                            break;
                    }
                }
            }
            catch (ThreadAbortException)
            {	// nie zadreczajmy uzytkownika tym wyjatkiem, w 99% powodowany przez Thread.Abort()
            }
            catch (Exception e)
            {
                if (Gadu.GaduCriticalError() == false)
                    throw new GaduRecieverException(e.Message);
            }
        }
    }
}
