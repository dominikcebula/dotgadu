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

namespace DotGadu
{
    /// <summary>
    /// Zawiera stale uzyte poczas komunikacji sieciowej
    /// </summary>
    public class GaduPacketConstans
    {
        /// <summary>
        /// pakiet na dzien dobry, to dostajemy zaraz po polaczeniu sie z serverem
        /// </summary>
        public const int GG_WELCOME = 0x0001;
        /// <summary>
        /// logowanie na server w v7.0
        /// </summary>
        public const int GG_LOGIN70 = 0x0019;
        /// <summary>
        /// logowanie sie powiodlo
        /// </summary>
        public const int GG_LOGIN_OK = 0x0003;
        /// <summary>
        /// user musi potwierdzic e-mail swojego konta
        /// </summary>
        public const int GG_NEED_EMAIL = 0x0014;
        /// <summary>
        /// logowanie sie nie powiodlo
        /// </summary>
        public const int GG_LOGIN_FAILED = 0x0009;
        /// <summary>
        /// autoryzacja przez gg_hash
        /// </summary>
        public const byte GG_LOGIN_HASH_GG32 = 0x01;
        /// <summary>
        /// autoryzacja przez sha1_hash
        /// </summary>
        public const byte GG_LOGIN_HASH_SHA1 = 0x02;
        /// <summary>
        /// ver. klienta
        /// </summary>
        public const int GG_VERSION = 0x2a;

        /// <summary>
        /// bedziemy ustawiac nowy status
        /// </summary>
        public const int GG_NEW_STATUS = 0x0002;

        /// <summary>
        /// Niedostepny
        /// </summary>
        public const int GG_STATUS_NOT_AVAIL = 0x0001;
        /// <summary>
        /// Niedostepny (z opisem)
        /// </summary>
        public const int GG_STATUS_NOT_AVAIL_DESCR = 0x0015;
        /// <summary>
        /// Dostepny
        /// </summary>
        public const int GG_STATUS_AVAIL = 0x0002;
        /// <summary>
        /// Dostepny (z opisem)
        /// </summary>
        public const int GG_STATUS_AVAIL_DESCR = 0x0004;
        /// <summary>
        /// Zajety
        /// </summary>
        public const int GG_STATUS_BUSY = 0x0003;
        /// <summary>
        /// Zajety (z opisem)
        /// </summary>
        public const int GG_STATUS_BUSY_DESCR = 0x0005;
        /// <summary>
        /// Niewidoczny
        /// </summary>
        public const int GG_STATUS_INVISIBLE = 0x0014;
        /// <summary>
        /// Niewidoczny z opisem
        /// </summary>
        public const int GG_STATUS_INVISIBLE_DESCR = 0x0016;
        /// <summary>
        /// Zablokowany
        /// </summary>
        public const int GG_STATUS_BLOCKED = 0x0006;
        /// <summary>
        /// Maska bitowa oznaczajaca tryb tylko dla przyjaciol
        /// </summary>
        public const int GG_STATUS_FRIENDS_MASK = 0x8000;

        /// <summary>
        /// Ping wysylany do servera Gadu-Gadu
        /// </summary>
        public const int GG_PING = 0x0008;
        /// <summary>
        /// Odpowiedz z servera Gadu-Gadu (server nie zawsze wysyla ten pakiet)
        /// </summary>
        public const int GG_PONG = 0x0007;

        /// <summary>
        /// wysylanie wiadomosci
        /// </summary>
        public const int GG_SEND_MSG = 0x000b;
        /// <summary>
        /// potwierdzenie wiadomosci
        /// </summary>
        public const int GG_SEND_MSG_ACK = 0x0005;
        /// <summary>
        /// odbieramy wiadomosc
        /// </summary>
        public const int GG_RECV_MSG = 0x000a;

        /// <summary>
        /// Bit ustawiany wylacznie przy odbiorze wiadomosci, gdy wiadomosc zostala wczesniej zakolejkowania z powodu nieobecnosci
        /// </summary>
        public const int GG_CLASS_QUEUED = 0x0001;
        /// <summary>
        /// Wiadomosc ma sie pojawic w osobnym okienku
        /// </summary>
        public const int GG_CLASS_MSG = 0x0004;
        /// <summary>
        /// Wiadomosc jest czescia toczacej sie rozmowy i zostanie wyswietlona w istniejacym okienku
        /// </summary>
        public const int GG_CLASS_CHAT = 0x0008;
        /// <summary>
        /// Wiadomosc jest przeznaczona dla klienta Gadu-Gadu i nie powinna byc wyswietlona uzytkownikowi.
        /// </summary>
        public const int GG_CLASS_CTCP = 0x0010;
        /// <summary>
        /// Klient nie zyczy sobie potwierdzenia wiadomosci.
        /// </summary>
        public const int GG_CLASS_ACK = 0x0020;

        /// <summary>
        /// Wiadomosci nie przeslano (zdarza sie przy wiadomosciach zawierajacych adresy internetowe blokowanych przez serwer GG gdy odbiorca nie ma nas na liscie)
        /// </summary>
        public const int GG_ACK_BLOCKED = 0x0001;
        /// <summary>
        /// Wiadomosc dostarczono
        /// </summary>
        public const int GG_ACK_DELIVERED = 0x0002;
        /// <summary>
        /// Wiadomosc zakolejkowano
        /// </summary>
        public const int GG_ACK_QUEUED = 0x0003;
        /// <summary>
        /// Wiadomosci nie dostarczono. Skrzynka odbiorcza na serwerze jest pelna (20 wiadomosci maks). Wystepuje tylko w trybie offline
        /// </summary>
        public const int GG_ACK_MBOXFULL = 0x0004;
        /// <summary>
        /// Wiadomosci nie dostarczono. Odpowiedz ta wystepuje tylko w przypadku wiadomosci klasy GG_CLASS_CTCP
        /// </summary>
        public const int GG_ACK_NOT_DELIVERED = 0x0006;

        /// <summary>
        /// Mamy pusta liste
        /// </summary>
        public const int GG_LIST_EMPTY = 0x0012;
        /// <summary>
        /// Pierwszy wpis w naszej liscie kontaktow
        /// </summary>
        public const int GG_NOTIFY_FIRST = 0x000f;
        /// <summary>
        /// Ostatni wpis w naszej liscie kontaktow
        /// </summary>
        public const int GG_NOTIFY_LAST = 0x0010;

        /// <summary>
        /// Kazdy uzytkownik dodany do listy kontaktow
        /// </summary>
        public const byte GG_USER_BUDDY = 0x01;
        /// <summary>
        /// Uzytkownik dla ktorego jestesmy wydoczni w trybie tylko dla przyjaciol
        /// </summary>
        public const byte GG_USER_FRIEND = 0x02;
        /// <summary>
        /// Uzytkownik od ktorego wiadomosci nie chcemy otrzymywac
        /// </summary>
        public const byte GG_USER_BLOCKED = 0x04;

        /// <summary>
        /// Dostalismy odpowiedz od servera, informacje o danym uzytkowniku, jego statusie
        /// </summary>
        public const int GG_NOTIFY_REPLY77 = 0x0018;

        /// <summary>
        /// Nieznane
        /// </summary>
        public const byte GG_UINFLAG_UNKNOWN1 = 0x10;
        /// <summary>
        /// Flaga spotykana, gdy u¿ytkownik staje siê niedostêpny
        /// </summary>
        public const byte GG_UINFLAG_UNKNOWN2 = 0x20;
        /// <summary>
        /// U¿ytkownik mo¿e prowadziæ rozmowy g³osowe
        /// </summary>
        public const byte GG_UINFLAG_VOICE = 0x40;
        /// <summary>
        /// U¿ytkownik ³¹czy siê przez bramkê Era Omnix
        /// </summary>
        public const byte GG_UINFLAG_ERA_OMNIX = 0x08;

        /// <summary>
        /// Ktos z listy zmienil status
        /// </summary>
        public const int GG_STATUS77 = 0x0017;

        /// <summary>
        /// Dodawanie danego numerku w czasie pracy
        /// </summary>
        public const int GG_ADD_NOTIFY = 0x000d;
        /// <summary>
        /// Usuwanie danego numerku w czasie pracy
        /// </summary>
        public const int GG_REMOVE_NOTIFY = 0x000e;

        /// <summary>
        /// Pakiet bedzie dotyczy listy kontaktow
        /// </summary>
        public const int GG_USERLIST_REQUEST = 0x0016;
        /// <summary>
        /// Poczatek eksportu listy
        /// </summary>
        public const byte GG_USERLIST_PUT = 0x00;
        /// <summary>
        /// Dalsza czesc eksportu listy
        /// </summary>
        public const byte GG_USERLIST_PUT_MORE = 0x01;
        /// <summary>
        /// Import listy
        /// </summary>
        public const byte GG_USERLIST_GET = 0x02;

        /// <summary>
        /// Odpowiedz servera gg dotyczaca listy kontaktow
        /// </summary>
        public const int GG_USERLIST_REPLY = 0x0010;
        /// <summary>
        /// poczatek eksportu listy
        /// </summary>
        public const byte GG_USERLIST_PUT_REPLY = 0x00;
        /// <summary>
        /// kontynuacja
        /// </summary>
        public const byte GG_USERLIST_PUT_MORE_REPLY = 0x02;
        /// <summary>
        /// poczatek importu listy 
        /// </summary>
        public const byte GG_USERLIST_GET_MORE_REPLY = 0x04;
        /// <summary>
        /// ostatnia czesc importu
        /// </summary>
        public const byte GG_USERLIST_GET_REPLY = 0x06;

        /// <summary>
        /// Pakiet bedzie dotyczyl zapytania do katalogu publicznego
        /// </summary>
        public const int GG_PUBDIR50_REQUEST = 0x0014;
        /// <summary>
        /// Zapisujemy nowe inforamcje do katalogu publicznego
        /// </summary>
        public const byte GG_PUBDIR50_WRITE = 0x01;
        /// <summary>
        /// Czytamy informacje z katalogu publicznego
        /// </summary>
        public const byte GG_PUBDIR50_READ = 0x02;
        /// <summary>
        /// Szukamy jakiejs osoby
        /// </summary>
        public const byte GG_PUBDIR50_SEARCH = 0x03;

        /// <summary>
        /// Server zwrocil wynik zapytania do katalogu publicznego
        /// </summary>
        public const int GG_PUBDIR50_REPLY = 0x000e;
        /// <summary>
        /// Server zwrocil wynik zapytania do katalogu publicznego
        /// </summary>
        public const byte GG_PUBDIR50_SEARCH_REPLY = 0x05;

        /// <summary>
        /// Server GG chce nas rozlaczyc
        /// </summary>
        public const int GG_DISCONNECTING = 0x000b;
        /// <summary>
        /// Server GG chce nas rozlaczyc
        /// </summary>
        public const int GG_DISCONNECTING2 = 0x000d;
    }

    /// <summary>
    /// Interfejs ktory bedzie implementowany przez dane pakiety wysylane do servera gadu-gadu
    /// </summary>
    public interface IGaduPacket
    {
        /// <summary>
        /// Funkcja zlicza rozmiar danego pakietu
        /// </summary>
        /// <returns>Rozmiar pakietu</returns>
        int getSize();   // pobieranie rozmiaru danego pakiety
        /// <summary>
        /// Funkcja czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z TcpClient</param>
        /// <param name="readSize">Rozmiar pakietu, ktory nalezy wczytac</param>
        void read(NetworkStream ins, int readSize); // czyta pakiet z NetworkStream
        /// <summary>
        /// Funkcja wpisuje pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z TcpClient</param>
        void write(NetworkStream outs); // wysylanie pakiety na NetworkStream
    }

    /// <summary>
    /// Naglowek kazdego pakietu
    /// Przed wyslaniem dowolnego pakietu najpierw musimy wyslac ten, zawiera on typ wyslanego pakietu
    /// oraz jego dlugosc, dzieki temu server gadu-gadu wie ile bajtow ma od nasz przeczytac oraz co one
    /// oznaczaja.
    /// </summary>
    public class GaduPacketHeader : IGaduPacket
    {
        /// <summary>
        /// Typ pakietu
        /// </summary>
        private int type;
        /// <summary>
        /// Rozmiar pakietu
        /// </summary>
        private int length;

        /// <summary>
        /// Typ pakietu
        /// </summary>
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Rozmiar pakietu
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu.
        /// </summary>
        /// <returns>Rozmiar pakietu</returns>
        public int getSize()
        {
            return 2 * sizeof(int);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda</param>
        /// <param name="readSize">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readSize)
        {
            BinaryReader bin = new BinaryReader(ins);   // musimy stworzyc sobie BinaryReader
            type = bin.ReadInt32();   // cztamy typ pakietu
            length = bin.ReadInt32();   // czytamy dlugosc pakietu
        }

        /// <summary>
        /// Wypisuje pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda, na ten stream pujda dane</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter bin = new BinaryWriter(outs);  // tworzymy BinaryWriter
            bin.Write(type);    // wpisujemy dane
            bin.Write(length);
        }
    }

    /// <summary>
    /// Pakiet, ktory sluzy do logowania sie
    /// </summary>
    public class GaduPacketLogin : IGaduPacket
    {
        private int uin;              /* moj numerek */
        private byte hash_type = GaduPacketConstans.GG_LOGIN_HASH_GG32;      /* rodzaj funkcji skrotu hasla */
        private byte[] hash;        /* skrot hasla - length 64 */
        private int status = 2;           /* status na dzien dobry */
        private int version = GaduPacketConstans.GG_VERSION;  /* moja wersja klienta */
        private byte unknown1 = 0x00;        /* 0x00 */
        private int local_ip = 0;         /* moj adres ip */
        private short local_port = 8074;     /* port, na ktorym slucham */
        private int external_ip = 0;      /* zewnetrzny adres ip */
        private short external_port = 8074;  /* zewnetrzny port */
        private byte image_size = 0;      /* maksymalny rozmiar grafiki w KB */
        private byte unknown2 = 0xbe;        /* 0xbe */
        private byte[] description;   /* opis, nie musi wystapic */
        private int time = 0;             /* czas, nie musi wystapic */

        /// <summary>
        /// Nasz numer gg
        /// </summary>
        public int Uin
        {
            get
            {
                return uin;
            }
            set
            {
                uin = value;
            }
        }

        /// <summary>
        /// Typ Hash, moze to byc GG32Hash albo SHA1 (SHA512)
        /// </summary>
        public byte HashType
        {
            set
            {
                hash_type = value;
            }
            get
            {
                return hash_type;
            }
        }

        /// <summary>
        /// Zaszyfrowane haslo
        /// Hash ma dlugosc 64 bajtow, z tym ze jezeli nasz hash wyjdzie krotszy to reszte
        /// uzupelnimy zerami
        /// </summary>
        public byte[] Hash
        {
            get
            {
                return hash;
            }
            set
            {
                hash = new byte[64];    // hash o dlugosci 54 bajtow
                for (int i = 0; i < value.Length; i++)  // przepisujemy tylko tyle bajtow ile wyszedl nasz hash
                    hash[i] = value[i];                 // reszta bedzie zerami
            }
        }

        /// <summary>
        /// Status gg
        /// </summary>
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        /// <summary>
        /// Wersja protokolu/klienta gg
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// Lokalne ip
        /// </summary>
        public int LocalIp
        {
            get
            {
                return local_ip;
            }
            set
            {
                local_ip = value;
            }
        }

        /// <summary>
        /// Loklany Port
        /// </summary>
        public short LocalPort
        {
            get
            {
                return local_port;
            }
            set
            {
                local_port = value;
            }
        }

        /// <summary>
        /// Zewnetrzne Ip
        /// </summary>
        public int ExternalIp
        {
            get
            {
                return external_ip;
            }
            set
            {
                external_ip = value;
            }
        }

        /// <summary>
        /// Zewnetrzny port
        /// </summary>
        public short ExternalPort
        {
            get
            {
                return external_port;
            }
            set
            {
                external_port = value;
            }
        }

        /// <summary>
        /// Max rozmiar rysunku jaki przyjmujemy
        /// </summary>
        public byte ImageSize
        {
            get
            {
                return image_size;
            }
            set
            {
                image_size = value;
            }
        }

        /// <summary>
        /// Opis GG
        /// </summary>
        public byte[] Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        /// <summary>
        /// Czas logowania
        /// </summary>
        public int Time
        {
            get
            {
                return time;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            int size;
            size = 5 * sizeof(int) + 4 * sizeof(byte) + 64 * sizeof(byte) + 2 * sizeof(short);  // obliczamy rozmiar
            return size;
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readSize">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readSize)
        {
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter bw = new BinaryWriter(outs);
            bw.Write(uin);      // wysylamy poszczegolne dane na gniazdo
            outs.WriteByte(hash_type);
            for (int i = 0; i < hash.Length; i++)
                outs.WriteByte(hash[i]);
            bw.Write(status);
            bw.Write(version);
            bw.Write(unknown1);
            bw.Write(local_ip);
            bw.Write(local_port);
            bw.Write(external_ip);
            bw.Write(external_port);
            bw.Write(image_size);
            bw.Write(unknown2);
            //bw.Write(description);
            //bw.Write(time);
        }
    }

    /// <summary>
    /// Pakiet sluzacy do ustawiania statusu
    /// </summary>
    public class GaduPacketStatus : IGaduPacket
    {
        private int status; // Numer statusu
        private byte[] desc;    // opis

        /// <summary>
        /// Numer statusu
        /// </summary>
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        /// <summary>
        /// Opis gg
        /// </summary>
        public String Description
        {
            get
            {
                //String ret;
                //ret = "";
                //for (int i = 0; i < desc.Length; i++)
                //    ret += (char)desc[i];
                //return ret;
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, desc));
            }
            set
            {
                //desc = new byte[value.Length + 1];
                //for (int i = 0; i < value.Length; i++)
                //    desc[i] = Convert.ToByte(value[i]);
                //desc[value.Length] = (byte)'\0';
                desc = new byte[value.Length + 1];
                Encoding enc = Encoding.GetEncoding("windows-1250");
                byte[] cp1250;
                cp1250 = enc.GetBytes(value);
                for (int i = 0; i < value.Length; i++)
                    desc[i] = cp1250[i];
                desc[value.Length] = (byte)'\0';
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return sizeof(int) + desc.Length;
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter bw = new BinaryWriter(outs);
            bw.Write(status);
            for (int i = 0; i < desc.Length; i++)
                outs.WriteByte(desc[i]);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu do wczytania</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);
            status = br.ReadInt32();
            readByte -= sizeof(int);
            desc = new byte[readByte];
            for (int i = 0; i < readByte; i++)
                desc[i] = (byte)ins.ReadByte();
        }
    }

    /// <summary>
    /// Pakiet sluzy do wysylania wiadomosci
    /// </summary>
    public class GaduPacketMessage : IGaduPacket
    {
        private int recepient;  // odbiorca
        private int seq = 0;    // sequence
        private int klasa;  // klasa wiadomosci
        private byte[] message; // wiadomosc w bajtach

        /// <summary>
        /// Odbiorca wiadomosci
        /// </summary>
        public int Recepient
        {
            set
            {
                recepient = value;
            }
            get
            {
                return recepient;
            }
        }

        /// <summary>
        /// Klasa wiadomosci
        /// </summary>
        public int Klasa
        {
            set
            {
                klasa = value;
            }
            get
            {
                return klasa;
            }
        }

        /// <summary>
        /// Wiadomosc
        /// </summary>
        public String Message
        {
            get
            {
                //String ret;
                //ret = "";
                //for (int i = 0; i < message.Length; i++)
                //    ret += (char)message[i];
                //return ret;
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, message));
            }
            set
            {
                //message = new byte[value.Length + 1];
                //for (int i = 0; i < value.Length; i++)
                //    message[i] = Convert.ToByte(value[i]);
                //message[value.Length] = (byte)'\0';
                message = new byte[value.Length + 1];
                Encoding enc = Encoding.GetEncoding("windows-1250");
                byte[] cp1250;
                cp1250 = enc.GetBytes(value);
                for (int i = 0; i < value.Length; i++)
                    message[i] = cp1250[i];
                message[value.Length] = (byte)'\0';
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return 3 * sizeof(int) + message.Length;
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter bw = new BinaryWriter(outs);
            bw.Write(recepient);
            bw.Write(seq);
            bw.Write(klasa);
            for (int i = 0; i < message.Length; i++)
                outs.WriteByte(message[i]);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu do wczytania</param>
        public void read(NetworkStream ins, int readByte)
        {

        }
    }

    /// <summary>
    /// Potwierdzenie, ktore dostajemy z server po wyslaniu wiadomosci
    /// </summary>
    public class GaduPacketMessageAck : IGaduPacket
    {
        private int status;
        private int recepient;
        private int seq;

        /// <summary>
        /// Status wiadomosci
        /// </summary>
        public int Status
        {
            set
            {
                status = value;
            }
            get
            {
                return status;
            }
        }

        /// <summary>
        /// Do kogo byla wyslana wiadomosc
        /// </summary>
        public int Recepient
        {
            set
            {
                recepient = value;
            }
            get
            {
                return recepient;
            }
        }

        /// <summary>
        /// Seeuence
        /// </summary>
        public int Sequence
        {
            set
            {
                seq = value;
            }
            get
            {
                return seq;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return 3 * sizeof(int);
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {

        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu do wczytania</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);
            status = br.ReadInt32();
            recepient = br.ReadInt32();
            seq = br.ReadInt32();
        }
    }

    /// <summary>
    /// Pakiet, ktory otrzumujemy kiedy ktos przesle do nas wiadomosc
    /// </summary>
    public class GaduPacketRecieveMessage : IGaduPacket
    {
        private int sender;		/* numer nadawcy */
        private int seq;		/* numer sekwencyjny */
        private int time;		/* czas nadania */
        private int klasa;		/* klasa wiadomosci */
        private byte[] message;		/* tresc wiadomosci */

        /// <summary>
        /// Od kogo pochodzi wiadomosc
        /// </summary>
        public int Sender
        {
            get
            {
                return sender;
            }
        }

        /// <summary>
        /// Sequence
        /// </summary>
        public int Sequence
        {
            get
            {
                return seq;
            }
        }

        /// <summary>
        /// Kiedy wiadomosc zostala wyslana
        /// </summary>
        public int Time
        {
            get
            {
                return time;
            }
        }

        /// <summary>
        /// Klasa wiadomosci
        /// </summary>
        public int Class
        {
            get
            {
                return klasa;
            }
        }

        /// <summary>
        /// Wiadomosc
        /// </summary>
        public String Message
        {
            get
            {
                //String ret;
                //ret = "";
                //for (int i = 0; i < message.Length; i++)
                //    ret += (char)message[i];
                //return ret;
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, message));
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return 4 * sizeof(int) + message.Length;
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {

        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);
            sender = br.ReadInt32();
            seq = br.ReadInt32();
            time = br.ReadInt32();
            klasa = br.ReadInt32();
            readByte -= 4 * sizeof(int);
            message = new byte[readByte];
            for (int i = 0; i < readByte; i++)
                message[i] = (byte)ins.ReadByte();
        }
    }

    /// <summary>
    /// Klasa reprezentujaca pakiet wysylany do servera gg,
    /// jest to pojedynczy element listy
    /// </summary>
    public class GaduPacketNotify : IGaduPacket
    {
        /// <summary>
        /// Numer danej osoby
        /// </summary>
        private int uin;
        /// <summary>
        /// GG_USER_BUDDY lub GG_USER_FRIEND lub GG_USER_BLOCKED
        /// </summary>
        private byte type;

        /// <summary>
        /// Numer danej osoby
        /// </summary>
        public int Uin
        {
            get
            {
                return uin;
            }
            set
            {
                uin = value;
            }
        }

        /// <summary>
        /// GG_USER_BUDDY lub GG_USER_FRIEND lub GG_USER_BLOCKED
        /// </summary>
        public byte Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return sizeof(int) + sizeof(byte);
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter bw = new BinaryWriter(outs);
            bw.Write(uin);
            bw.Write(type);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {

        }
    }

    /// <summary>
    /// Pakiet zawierajacy informacje o stanie danego numeru gg
    /// </summary>
    public class GaduPacketNotifyReplay77 : IGaduPacket
    {
        /// <summary>
        /// numerek plus flagi w najstarszym bajcie
        /// </summary>
        private int uin;
        /// <summary>
        /// status danej osoby
        /// </summary>
        private byte status;
        /// <summary>
        /// adres IP bezposrednich polaczen
        /// </summary>
        private int remote_ip;
        /// <summary>
        /// port bezposrednich polaczen
        /// </summary>
        private short remote_port;
        /// <summary>
        /// wersja klienta
        /// </summary>
        private byte version;
        /// <summary>
        /// maksymalny rozmiar obrazkow w KB
        /// </summary>
        private byte image_size;
        /// <summary>
        /// 0x00
        /// </summary>
        private byte unknown1;
        /// <summary>
        /// 0x00000000
        /// </summary>
        private int unknown2;
        /// <summary>
        /// rozmiar opisu i czasu, nie musi wystapic
        /// </summary>
        private byte description_size;
        /// <summary>
        /// opis, nie musi wystapic
        /// </summary>
        private byte[] description;
        /// <summary>
        /// czas, nie musi wystapic
        /// </summary>
        private int time = 0;

        /// <summary>
        /// Flaga z UIN
        /// </summary>
        public int Flaga
        {
            get
            {
                byte[] uinb;
                uinb = BitConverter.GetBytes(uin);
                return uinb[3];
            }
        }

        /// <summary>
        /// Numerek
        /// </summary>
        public int Uin
        {
            get
            {
                byte[] uinb;
                uinb = BitConverter.GetBytes(uin);
                uinb[3] = 0x00;
                return BitConverter.ToInt32(uinb, 0);
            }
        }

        /// <summary>
        /// status danej osoby
        /// </summary>
        public byte Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// adres IP bezposrednich polaczen
        /// </summary>
        public int RemoteIP
        {
            get
            {
                return remote_ip;
            }
        }

        /// <summary>
        /// port bezposrednich polaczen
        /// </summary>
        public short RemotePort
        {
            get
            {
                return remote_port;
            }
        }

        /// <summary>
        /// wersja klienta
        /// </summary>
        public byte Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// maksymalny rozmiar obrazkow w KB
        /// </summary>
        public byte ImageSize
        {
            get
            {
                return image_size;
            }
        }

        /// <summary>
        /// 0x00
        /// </summary>
        public byte Unknown1
        {
            get
            {
                return unknown1;
            }
        }

        /// <summary>
        /// 0x00000000
        /// </summary>
        public int Unknown2
        {
            get
            {
                return unknown2;
            }
        }

        /// <summary>
        /// rozmiar opisu i czasu, nie musi wystapic
        /// </summary>
        public int DescriptionSize
        {
            get
            {
                return description_size;
            }
        }

        /// <summary>
        /// opis, nie musi wystapic
        /// </summary>
        public String Description
        {
            get
            {
                if (description == null)
                    return "";
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, description));
            }
        }

        /// <summary>
        /// czas, nie musi wystapic
        /// </summary>
        public int Time
        {
            get
            {
                return time;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return 3 * sizeof(int) + 4 * sizeof(byte) + sizeof(short) + sizeof(byte) + description_size;
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {

        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);

            uin = br.ReadInt32();
            readByte -= sizeof(int);

            status = br.ReadByte();
            readByte -= sizeof(byte);

            remote_ip = br.ReadInt32();
            readByte -= sizeof(int);

            remote_port = br.ReadInt16();
            readByte -= sizeof(short);

            version = br.ReadByte();
            readByte -= sizeof(byte);

            image_size = br.ReadByte();
            readByte -= sizeof(byte);

            unknown1 = br.ReadByte();
            readByte -= sizeof(byte);

            unknown2 = br.ReadInt32();
            readByte -= sizeof(int);

            if (readByte <= 0)
                return;
            description_size = br.ReadByte();
            readByte -= sizeof(byte);

            if (readByte <= 0)
                return;
            description = new byte[description_size];
            for (int i = 0; i < description_size; i++)
                description[i] = br.ReadByte();
        }
    }

    /// <summary>
    /// Pakiet zawierajacy informacje o stanie danego numeru gg
    /// </summary>
    public class GaduPacketStatus77 : IGaduPacket
    {
        /// <summary>
        /// numerek plus flagi w najstarszym bajcie
        /// </summary>
        private int uin;
        /// <summary>
        /// status danej osoby
        /// </summary>
        private byte status;
        /// <summary>
        /// adres IP bezposrednich polaczen
        /// </summary>
        private int remote_ip;
        /// <summary>
        /// port bezposrednich polaczen
        /// </summary>
        private short remote_port;
        /// <summary>
        /// wersja klienta
        /// </summary>
        private byte version;
        /// <summary>
        /// maksymalny rozmiar obrazkow w KB
        /// </summary>
        private byte image_size;
        /// <summary>
        /// 0x00
        /// </summary>
        private byte unknown1;
        /// <summary>
        /// 0x00000000
        /// </summary>
        private int unknown2;
        /// <summary>
        /// opis, nie musi wystapic
        /// </summary>
        private byte[] description;
        /// <summary>
        /// czas, nie musi wystapic
        /// </summary>
        private int time = 0;

        /// <summary>
        /// Flaga z UIN
        /// </summary>
        public int Flaga
        {
            get
            {
                byte[] uinb;
                uinb = BitConverter.GetBytes(uin);
                return uinb[3];
            }
        }

        /// <summary>
        /// Numerek
        /// </summary>
        public int Uin
        {
            get
            {
                byte[] uinb;
                uinb = BitConverter.GetBytes(uin);
                uinb[3] = 0x00;
                return BitConverter.ToInt32(uinb, 0);
            }
        }

        /// <summary>
        /// status danej osoby
        /// </summary>
        public byte Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// adres IP bezposrednich polaczen
        /// </summary>
        public int RemoteIP
        {
            get
            {
                return remote_ip;
            }
        }

        /// <summary>
        /// port bezposrednich polaczen
        /// </summary>
        public short RemotePort
        {
            get
            {
                return remote_port;
            }
        }

        /// <summary>
        /// wersja klienta
        /// </summary>
        public byte Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// maksymalny rozmiar obrazkow w KB
        /// </summary>
        public byte ImageSize
        {
            get
            {
                return image_size;
            }
        }

        /// <summary>
        /// 0x00
        /// </summary>
        public byte Unknown1
        {
            get
            {
                return unknown1;
            }
        }

        /// <summary>
        /// 0x00000000
        /// </summary>
        public int Unknown2
        {
            get
            {
                return unknown2;
            }
        }

        /// <summary>
        /// opis, nie musi wystapic
        /// </summary>
        public String Description
        {
            get
            {
                if (description == null)
                    return "";
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, description));
            }
        }

        /// <summary>
        /// czas, nie musi wystapic
        /// </summary>
        public int Time
        {
            get
            {
                return time;
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return 3 * sizeof(int) + 4 * sizeof(byte) + sizeof(short);
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {

        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);

            uin = br.ReadInt32();
            readByte -= sizeof(int);

            status = br.ReadByte();
            readByte -= sizeof(byte);

            remote_ip = br.ReadInt32();
            readByte -= sizeof(int);

            remote_port = br.ReadInt16();
            readByte -= sizeof(short);

            version = br.ReadByte();
            readByte -= sizeof(byte);

            image_size = br.ReadByte();
            readByte -= sizeof(byte);

            unknown1 = br.ReadByte();
            readByte -= sizeof(byte);

            unknown2 = br.ReadInt32();
            readByte -= sizeof(int);

            if (readByte <= 0)
                return;
            description = new byte[readByte];
            for (int i = 0; i < readByte; i++)
                description[i] = br.ReadByte();
        }
    }

    /// <summary>
    /// Pakiet dokonujacych zmian na liscie kontaktow na serwerze
    /// </summary>
    public class GaduPacketUserListRequest : IGaduPacket
    {
        /// <summary>
        /// Rodzaj zapytania
        /// </summary>
        private byte type;
        /// <summary>
        /// Tresc zapytania, nie musi wystapic
        /// </summary>
        private byte[] request;

        /// <summary>
        /// Rodzaj zapytania
        /// </summary>
        public byte Type
        {
            set
            {
                type = value;
            }
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Tresc zapytania, nie musi wystapic
        /// </summary>
        public String Request
        {
            set
            {
                request = new byte[value.Length + 1];
                Encoding enc = Encoding.GetEncoding("windows-1250");
                byte[] cp1250;
                cp1250 = enc.GetBytes(value);
                for (int i = 0; i < value.Length; i++)
                    request[i] = cp1250[i];
                request[value.Length] = (byte)'\0';
            }
            get
            {
                if (request == null)
                    return "";
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, request));
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return sizeof(byte) + (request == null ? 0 : request.Length);
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            outs.WriteByte(type);
            if (request == null)
                return;
            for (int i = 0; i < request.Length; i++)
                outs.WriteByte(request[i]);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {
            type = (byte)ins.ReadByte();
            if (readByte - 1 <= 0)
                return;
            request = new byte[readByte - 1];       // rozmiar readByte-1 poniewaz wczytalismy juz 1B - typ pakietu
            for (int i = 0; i < readByte - 1; i++)
                request[i] = (byte)ins.ReadByte();
        }
    }

    /// <summary>
    /// Pakiet sluzacy do wymiany informacji z katalogiem publicznym
    /// </summary>
    public class GaduPacketPubDir50 : IGaduPacket
    {
        /// <summary>
        /// Rodzaj zapytania
        /// </summary>
        private byte type;
        /// <summary>
        /// Numer sekwencyjny
        /// </summary>
        private int seq;
        /// <summary>
        /// Tresc zapytania lub odpowiedzi
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Rodzaj zapytania
        /// </summary>
        public byte Type
        {
            set
            {
                type = value;
            }
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Numer sekwencyjny
        /// </summary>
        public int Seq
        {
            set
            {
                seq = value;
            }
            get
            {
                return seq;
            }
        }

        /// <summary>
        /// Tresc zapytania
        /// </summary>
        public String Data
        {
            set
            {
                data = new byte[value.Length + 1];
                Encoding enc = Encoding.GetEncoding("windows-1250");
                byte[] cp1250;
                cp1250 = enc.GetBytes(value);
                for (int i = 0; i < value.Length; i++)
                    data[i] = cp1250[i];
                data[value.Length] = (byte)'\0';
            }
            get
            {
                if (data == null)
                    return "";
                Encoding enc = Encoding.Unicode;
                return enc.GetString(Encoding.Convert(Encoding.GetEncoding("windows-1250"), Encoding.Unicode, data));
            }
        }

        /// <summary>
        /// Zwraca rozmiar pakietu
        /// </summary>
        /// <returns>rozmiar pakietu</returns>
        public int getSize()
        {
            return sizeof(byte) + sizeof(int) + (data == null ? 0 : data.Length);
        }

        /// <summary>
        /// Wysyla pakiet na NetworkStream
        /// </summary>
        /// <param name="outs">NetworkStream pobrany z gniazda</param>
        public void write(NetworkStream outs)
        {
            BinaryWriter br = new BinaryWriter(outs);
            br.Write(type);
            br.Write(seq);
            br.Write(data);
        }

        /// <summary>
        /// Czyta pakiet z NetworkStream
        /// </summary>
        /// <param name="ins">NetworkStream pobrany z gniazda, z niego zostanie wczytany pakiet</param>
        /// <param name="readByte">Rozmiar pakietu, mowi o tym ile wczytac bajtow</param>
        public void read(NetworkStream ins, int readByte)
        {
            BinaryReader br = new BinaryReader(ins);
            type = br.ReadByte();
            readByte -= sizeof(byte);
            if (readByte <= 0)
                return;

            seq = br.ReadInt32();
            readByte -= sizeof(int);
            if (readByte <= 0)
                return;

            data = br.ReadBytes(readByte);
        }
    }
}
