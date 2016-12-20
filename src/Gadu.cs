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
    /// Delegacja, ktora nalezy podstawic pod event OnGaduCriticalError;
    /// </summary>
    public delegate void OnGaduCriticalErrorHandler();

    /// <summary>
    /// Wyjatek zwracany podczas nieudanej proby polaczenia z serverem gadu-gadu
    /// </summary>
    public class GaduConnectionException : Exception   // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas nieudanej proby polaczenia z serverem gadu-gadu
        /// </summary>
        public GaduConnectionException(String message) : base(message) { }   // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany gdy sprobujemy sie zalogowac do gg bez nawiazanego polaczenie z serverem Gadu-Gadu
    /// </summary>
    public class GaduNotConnectedException : Exception   // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany gdy sprobujemy sie zalogowac do gg bez nawiazanego polaczenie z serverem Gadu-Gadu
        /// </summary>
        public GaduNotConnectedException(String message) : base(message) { }   // dziedziczenie konstruktora
    }

    /// <summary>
    /// Najwazniejsza klasa z punktu widzenia uzytkownika, to za pomoca niej laczymy sie z serverem gg, logujemy sie
    /// oraz wysylamy wiadomosci
    /// </summary>
    public class Gadu
    {
        /// <summary>
        /// Event, ktory jest wywolywany podczas wszelkich nieprzewidzianych bledow podczas
        /// odczytu/zapisu na/z gniazda.
        /// </summary>
        public static event OnGaduCriticalErrorHandler OnGaduCriticalError;
        /// <summary>
        /// Adres ip servera
        /// </summary>
        private String host;    // adres ip servera gadu-gadu
        /// <summary>
        /// Port Servera gg
        /// </summary>
        private int port;       // port servera gadu-gadu
        /// <summary>
        /// BinaryReader podlaczony do naszego socketa
        /// </summary>
        private BinaryReader socketIn;  // za pomoca BinaryReader bedziemy czytali z gniazda
        /// <summary>
        /// BinaryWriter podlaczony do naszego socketa
        /// </summary>
        private BinaryWriter socketOut; // za pomoca BinaryReader bedziemy wysylali na gniazdo dane
        /// <summary>
        /// Gniazdo podlaczone do servera Gadu-Gadu
        /// </summary>
        private TcpClient tcpClient;    // gniazdo
        /// <summary>
        /// Czas oczekiwania na odpowiedz od servera Gadu-Gadu
        /// </summary>
        private int timeOut;    // czas po ktorym uznajemy ze dany server padl
        /// <summary>
        /// Ziarno zwracane przez servera Gadu-Gadu zaraz po polaczeniu, potrzebne do wyliczenia hashu hasla
        /// </summary>
        private uint seed;
        /// <summary>
        /// Klasa udostepniajaca mechanizm logowania
        /// </summary>
        private GaduLogin gaduLogin;    // aktualna sesja gadu-gadu
        /// <summary>
        /// Klasa udostepniajaca mechanizm zmiany statusu
        /// </summary>
        private GaduStatus gaduStatus;  // sluzy do zmieniania statusu gg
        /// <summary>
        /// Klasa umozliwiajaca wysylanie wiadomisci
        /// </summary>
        private GaduMessage gaduMessage;    // wysylanie wiadomosci
        /// <summary>
        /// Klasa w, ktorej dziala watek odbierajacy pakiety z servera Gadu-Gadu
        /// </summary>
        private GaduReciever gaduReciever;  // klasa w ktorej znajduje sie watek ktory odbiera pakiety
        /// <summary>
        /// Klasa kolejkujaca pakiety do wyslania
        /// </summary>
        private GaduSender gaduSender;
        /// <summary>
        /// Co 2 min. wysyla pakiet ping do servera Gadu-Gadu
        /// </summary>
        private GaduPinger gaduPinger;
        /// <summary>
        /// Klasa realizujaca operacje powiadomien o uzytkownikach na liscie
        /// </summary>
        private GaduNotifier gaduNotifier;
        /// <summary>
        /// Klasa odpowiedzialna za zarzadzanie lista kontaktow
        /// </summary>
        private GaduUserList gaduUserList;
        /// <summary>
        /// Klasa dostarczajaca mechanizmow dzialania na katalogu publicznym
        /// </summary>
        private GaduPubDir gaduPubDir;

        /// <summary>
        /// Klasa dostarczajaca mechanizmow dzialania na katalogu publicznym
        /// </summary>
        public GaduPubDir PubDir
        {
            get
            {
                return gaduPubDir;
            }
        }

        /// <summary>
        /// Klasa odpowiedzialna za zarzadzanie lista kontaktow
        /// </summary>
        public GaduUserList UserList
        {
            get
            {
                return gaduUserList;
            }
        }

        /// <summary>
        /// Zwraca referencje do klasy GaduNotifier, dzieki niej mozemy wyslac liste
        /// do server gg i dowiedziec sie kto ma jaki status
        /// </summary>
        public GaduNotifier Notifier
        {
            get
            {
                return gaduNotifier;
            }
        }

        /// <summary>
        /// Zwraca referencje do klasy GaduReciever, dzieki niej mozemy dopisac obsluge eventow,
        /// ustawic parametry klasy
        /// </summary>
        public GaduReciever Reciever
        {
            get
            {
                return gaduReciever;
            }
        }

        /// <summary>
        /// Zwraca referencje do klasy GaduSender, dzieki niej mozemy dopisac obsluge eventow,
        /// ustawic parametry klasy
        /// </summary>
        public GaduSender Sender
        {
            get
            {
                return gaduSender;
            }
        }

        /// <summary>
        /// Zwraca referencje do klasy GaduPinger, dzieki niej mozemy dopisac obsluge eventow,
        /// ustawic parametry klasy
        /// </summary>
        public GaduPinger Pinger
        {
            get
            {
                return gaduPinger;
            }
        }

        /// <summary>
        /// Czas po ktorym uznajemy ze dany server gadu-gadu nie odpowiada, jest martwy.
        /// </summary>
        public int TimeOut  // pobieranie/ustawianie timeOut
        {
            set
            {
                timeOut = value;
                tcpClient.ReceiveTimeout = timeOut;
                tcpClient.SendTimeout = timeOut;
            }
            get
            {
                return timeOut;
            }
        }

        /// <summary>
        /// Adres ip servera gadu-gadu
        /// </summary>
        public String Host  // pobieranie Host
        {
            get
            {
                return host;
            }
        }

        /// <summary>
        /// Port servera gadu-gadu
        /// </summary>
        /// <value>Port servera gadu-gadu</value>
        public int Port // pobieranie Port
        {
            get
            {
                return port;
            }
        }

        /// <summary>
        /// Statyczna metoda, ktora wywoluje event OnGaduCriticalError, nie wywolywac samemu, prawo do wywolania
        /// maja tylko GaduSender, GaduReciever lub GaduPinger
        /// </summary>
        /// <returns>Stan mowiacy o powodzeniu lub niepowodzeniu wywolania eventu</returns>
        public static bool GaduCriticalError()
        {
            if (OnGaduCriticalError != null)
            {
                OnGaduCriticalError.BeginInvoke(null, null);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Tworzy gniazdo oraz ustawia domyslny timeOut, tworzy klasy GaduReciever,GaduSender,GaduPinger
        /// </summary>
        public Gadu()
        {
            tcpClient = new TcpClient();    // tworzymy gniazdo
            tcpClient.ReceiveTimeout = 5000;    // ustawiamy nasza domyslna wartosc na timeOut
            tcpClient.SendTimeout = 5000;
            timeOut = 5000;

            gaduReciever = new GaduReciever(tcpClient);
            gaduSender = new GaduSender(tcpClient);
            gaduPinger = new GaduPinger(gaduSender);
            gaduNotifier = new GaduNotifier(gaduSender);
            gaduUserList = new GaduUserList(gaduSender);
            gaduPubDir = new GaduPubDir(gaduSender);
        }

        /// <summary>
        /// Laczy sie z danym serverem gadu-gadu
        /// </summary>
        /// <param name="gaduServer">Server gadu-gadu</param>
        /// <example>
        /// Gadu gadu = new Gadu();
        /// gadu.Connect(new GaduServer("217.17.41.87",8074));
        /// </example>
        public void Connect(GaduServer gaduServer)  // laczymy sie z danym server gadu-gadu
        {
            try
            {
                host = gaduServer.Host; // przypisujemy adres ip
                port = gaduServer.Port; // przypisujemy port
                tcpClient.Connect(Host, port);  // laczymy sie
                socketIn = new BinaryReader(tcpClient.GetStream()); // pobieramy NetworkStream i tworzymy BinaryReader
                socketOut = new BinaryWriter(tcpClient.GetStream());    // pobieramy NetworkStream i tworzymy BinaryWriter

                socketIn.ReadInt32();
                socketIn.ReadInt32();
                seed = socketIn.ReadUInt32();  // pobieramy "ziarno", GG_WELCOME, potrzebne do wyliczenia hasha hasla

                gaduReciever.work = true;   // odebralismy co trzeba, teraz odbieraniem pakietow zajmie sie GaduReceiver
            }
            catch (Exception e)
            {
                throw new GaduConnectionException(e.Message);
            }
        }

        /// <summary>
        /// Konczymy polaczenie z gadu-gadu
        /// </summary>
        public void Disconnect()    // koniec polaczenia
        {
            gaduReciever.Terminate();
            gaduSender.Terminate();
            gaduPinger.Terminate();
            socketIn.Close();   // zamykamy BinaryReader
            socketOut.Close();  // zamykamy BinaryWriter
            tcpClient.Close();  // konczymy polaczenie
        }

        /// <summary>
        /// Logujemy sie do gg
        /// </summary>
        /// <param name="uin">Nasz numer gg</param>
        /// <param name="pass">Nasze haslo</param>
        public void Login(int uin, String pass)
        {
            if (!tcpClient.Connected)
                throw new GaduNotConnectedException("Connect to GaduServer first");

            gaduLogin = new GaduLogin(seed, gaduSender);   // tworzymy sesje
            gaduLogin.login(uin, pass);   // logujemy sie
        }

        /// <summary>
        /// Wylogowanie z gg
        /// </summary>
        public void Logout()
        {
            if (!tcpClient.Connected)
                throw new GaduNotConnectedException("Connect to GaduServer first");

            changeStatus(GaduPacketConstans.GG_STATUS_NOT_AVAIL, "");
        }

        /// <summary>
        /// Zmiana status, statusid nalezy podac z GaduPacketConstans.GG_STATUS_*
        /// </summary>
        /// <param name="statusid">Numerek statusu z GG_STATUS_*</param>
        /// <param name="descr">Jezeli wybralismy jakis status z opisem to tutaj dajemy opis</param>
        public void changeStatus(int statusid, string descr)
        {
            if (!tcpClient.Connected)
                throw new GaduNotConnectedException("Connect to GaduServer first");

            gaduStatus = new GaduStatus(gaduSender);
            gaduStatus.changeStatus(statusid, descr);
        }

        /// <summary>
        /// Wysyla wiadomosc pod dany numer gg
        /// </summary>
        /// <param name="uin">Numer odbiorcy</param>
        /// <param name="msg">Wiadomosc</param>
        /// <returns>Status wiadomosci, potwierdzenie odebrania lub kod bledu</returns>
        public void sendMessage(int uin, String msg)
        {
            if (!tcpClient.Connected)
                throw new GaduNotConnectedException("Connect to GaduServer first");

            gaduMessage = new GaduMessage(gaduSender);
            gaduMessage.sendMessage(uin, msg);
        }
    }
}
