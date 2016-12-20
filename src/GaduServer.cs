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
    /// Wyjatek zwracany podczas pobierania adresu ip i portu z appmsg.gadu-gadu.pl
    /// </summary>
    public class GaduServerException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas pobierania adresu ip i portu z appmsg.gadu-gadu.pl
        /// </summary>
        public GaduServerException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa, ktora pobiera ip oraz port z appmsg.gadu-gadu.pl. Istnieje rowniez mozliwosc podania tych danych
    /// tak aby nie trzeba bylo ich pobierac z servera.
    /// </summary>
    public class GaduServer
    {
        private TcpClient tcpClient;    // gniazdo
        private String host;    // adres ip servera gg, ktory zostanie pobrany z appmsg.gadu-gadu.pl
        private int port;       // port na ktory mamy sie polaczyc
        private int uin;        // nr. gg dla, ktorego chcemy pobrac adres ip servera
        private StreamReader socketIn;  // za pomoca StreamReader bedziemy czytac informacje z gniazda, glownie ReadLine() :)
        private NetworkStream socketOut;    // do wysylania danych uzyjemy NetworkStream i funkcji WriteByte(), aby wyslac znak w formacie ASCII, a nie UTF-8

        private int timeOut;        // czas na polaczenie do servera

        /// <summary>
        /// Adres ip servera gadu-gadu
        /// </summary>
        /// <value>Adres ip servera gadu-gadu</value>
        public String Host  // zwracanie Host-a
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
        public int Port // zwracanie Portu
        {
            get
            {
                return port;
            }
        }

        /// <summary>
        /// Czas po ktorym uznajemy ze dany server gadu-gadu nie odpowiada, jest martwy.
        /// </summary>
        public int TimeOut  // zwracanie/ustawianie timeOut
        {
            set
            {
                TimeOut = value;
                tcpClient.ReceiveTimeout = timeOut;
                tcpClient.SendTimeout = timeOut;
            }
            get
            {
                return timeOut;
            }
        }

        /// <summary>
        /// UWAGA wystepuje PROBLEM z pobieraniem adresow z hub gg, ZALECA sie uzywanie konstruktora GaduServer(String host, int port)
        /// 
        /// Konstruktor do ktorego podajemy numer gg, na podstawie nr. gg appmsg.gadu-gadu.pl wybiera nam odpowiedni
        /// server
        /// </summary>
        /// <param name="uin">Numer gg</param>
        public GaduServer(int uin)  // konstruktor w ktorym podajemy uin dla ktorego adres ip hosta i portu zostanie pobrany z servera
        {
            tcpClient = new TcpClient();    // tworzymy gniazdo
            this.uin = uin;   // ustawiamy uin

            tcpClient.ReceiveTimeout = 5000;    // ustawiamy nasz domyslny timeOut
            tcpClient.SendTimeout = 5000;
            timeOut = 5000;
            getServer();    // pobieramy adres ip servera gadu-gadu dla naszego uin
        }

        /// <summary>
        /// Konstruktor, ktory wpisze dane o adres ip server i port, nie sa one wiec pobierane z appmsg.gadu-gadu.pl
        /// </summary>
        /// <param name="host">Adres ip servera gadu-gadu</param>
        /// <param name="port">Port servera gadu-gadu</param>
        public GaduServer(String host, int port)    // jezeli chcemy podac adres servera gadu-gadu i jego port, a nie pobierac tych danych z appmsg.gadu-gadu.pl
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Pobiera adres ip oraz port z servera gadu-gadu
        /// </summary>
        /// <exception cref="GaduServerException">GaduServerException</exception>
        private void getServer()
        {
            try
            {
                String line;
                tcpClient.Connect("appmsg.gadu-gadu.pl", 80);   // laczymy sie na port 80 do appmsg.gadu-gadu.pl
                socketIn = new StreamReader(tcpClient.GetStream()); // wezmiemy sobie referencje do NetworkStream, oraz stworzymy StreamReader
                socketOut = tcpClient.GetStream();  // pobieramy referencje do NetworkStream
                line = "GET /appsvc/appmsg4.asp?fmnumber=" + uin + "&version=5.0.3.107 HTTP/1.0\r\n\r\n"; // nasze zapytanie do servera gadu-gadu, wersja HTTP/1.0 wystarczy, nie widze potrzeby aby wysylac dodatkowe informacje
                for (int i = 0; i < line.Length; i++)   // zamieniamy naszego Stringa na kody ASCII (1 bajt) z kodow UTF(2 bajty)
                    socketOut.WriteByte(Convert.ToByte(line[i]));   // wysylanie na gniazdo
                socketIn.ReadLine();    // tutaj przec zytamy HTTP/1.0 OK
                socketIn.ReadLine();    // pusta linia
                line = socketIn.ReadLine(); // nasze cenne dane :)
                tcpClient.Close();  // zamykamy polaczenie, juz nie bedzie potrzebne

                String[] vals;  // tablica do ktorej wlozymy rozdzielone stringi
                vals = line.Split(' '); // rozdzielamy wynik naszego zapytania otrzymany z servera gg
                if (vals[0] != "0") // jezeli pierwszy czlon jest rozny od zera to znaczy ze mamy blad
                    throw new GaduServerException(vals[2]); // a wiec wyjatek

                line = vals[2]; // jezeli doszlismy tu znaczy ze nie ma zadnego bledu, teraz sobie pobierzemy ip i port :)
                vals = line.Split(':'); // rozdzialamy stringa, ip i port dostajemy w formie ip:port
                host = vals[0]; // pierwsza wartosc to host
                port = Convert.ToInt32(vals[1]);    // druga wartosc to port :)
            }
            catch (Exception e)
            {
                throw new GaduServerException(e.Message);
            }
        }
    }
}
