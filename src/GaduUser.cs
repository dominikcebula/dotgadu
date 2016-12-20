/*
 * DotGadu 2007/2008
 * WWW: http://dotgadu.sf.net/
 * E-MAIL: dominikcebula@gmail.com
 *
 * Projekt biblioteki, ktorej celem jest obsluga protokolu GaduGadu z poziomu .NET. Opis protokolu zostal
 * zaczerpniety z libgadu, mozna go znalezc tutaj: http://ekg.chmurka.net/docs/protocol.html
*/

using System;
using System.Collections.Generic;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany podczas bledu operacji na uzytkowniku
    /// </summary>
    public class GaduUserException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany podczas bledu operacji na uzytkowniku
        /// </summary>
        public GaduUserException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa reperezentujaca uzytkownika Gadu-Gadu
    /// </summary>
    public class GaduUser
    {
        /// <summary>
        /// Imie
        /// </summary>
        private String name;
        /// <summary>
        /// Nazwisko
        /// </summary>
        private String lastname;
        /// <summary>
        /// Pseudonim
        /// </summary>
        private String nick;
        /// <summary>
        /// Pseudonim wyswietlany
        /// </summary>
        private String visiblenick;
        /// <summary>
        /// Telefon komorkowy
        /// </summary>
        private int mobile_phone;
        /// <summary>
        /// Grupa
        /// </summary>
        private String group;
        /// <summary>
        /// Numer gg
        /// </summary>
        private int uin;
        /// <summary>
        /// Adres email
        /// </summary>
        private String email;
        /// <summary>
        /// Okresla dzwiek zwiazany z pojawieniem sie danej osoby, mozliwe stany:
        /// 0 - ustawienia globalne
        /// 1 - dzwiek powiadomienia wylaczony
        /// 2 - zostanie odtworzony dzwiek z pola podanego w polu path_avail
        /// </summary>
        private byte avail;
        /// <summary>
        /// Sciezka do dzieku ktory zostanie odtworzony gdy dany numerek zmieni status oraz
        /// jezeli zmienna avail ustawiona jest na 2
        /// </summary>
        private String path_avail;
        /// <summary>
        /// Okresla dzwiek zwiazany z odebraniem wiadomosci od danej osoby, mozliwe stany:
        /// 0 - ustawienia globalne
        /// 1 - dzwiek powiadomienia wylaczony
        /// 2 - zostanie odtworzony dzwiek z pola podanego w polu path_message
        /// </summary>
        private String message;
        /// <summary>
        /// Sciezka do dzieku ktory zostanie odtworzony po otrzymaniu wiadomosci od danego numerku
        /// jezeli zmienna message ustawiona jest na 2
        /// </summary>
        private String path_message;
        /// <summary>
        /// Okresla czy bedziemy dostepni dla danej osoby w trybie "tylko dla przyjaciol", mozliwe stany:
        /// 0 - dostepni
        /// 1 - niedostepni
        /// </summary>
        private byte hidden;
        /// <summary>
        /// Telefon domowy
        /// </summary>
        private int phone;

        /// <summary>
        /// Rok urodzenia - do katalogu publicznego
        /// </summary>
        private int birthyear;
        /// <summary>
        /// Miasto zamieszkania - do katalogu publicznego
        /// </summary>
        private String city;
        /// <summary>
        /// Plec - do katalogu publicznego:
        /// 1 - kobieta
        /// 2 - mezczyzna
        /// </summary>
        private int gender;
        /// <summary>
        /// Jesli szukamy tylko aktywnych ma wartosc 1 - do katalogu publicznego
        /// </summary>
        private int activeonly;
        /// <summary>
        /// Nazwisko panienskie - do katalogu publicznego
        /// </summary>
        private String familyname;
        /// <summary>
        /// Miejscowosc pochodzenia - do katalogu publicznego
        /// </summary>
        private String familycity;
        /// <summary>
        /// Numer, od ktorego rozpoczac wyszukiwanie. Ma znaczenie, gdy kontynuujemy wyszukiwanie - do katalogu publicznego
        /// </summary>
        private int fmstart;

        /// <summary>
        /// Imie
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Nazwisko
        /// </summary>
        public String LastName
        {
            get
            {
                return lastname;
            }
            set
            {
                lastname = value;
            }
        }

        /// <summary>
        /// Pseudonim
        /// </summary>
        public String Nick
        {
            get
            {
                return nick;
            }
            set
            {
                nick = value;
            }
        }

        /// <summary>
        /// Pseudonim wyswietlany
        /// </summary>
        public String VisibleNick
        {
            get
            {
                return visiblenick;
            }
            set
            {
                visiblenick = value;
            }
        }

        /// <summary>
        /// Telefon komorkowy
        /// </summary>
        public int MobilePhone
        {
            get
            {
                return mobile_phone;
            }
            set
            {
                mobile_phone = value;
            }
        }

        /// <summary>
        /// Grupa
        /// </summary>
        public String Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
            }
        }

        /// <summary>
        /// Numer gg
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
        /// Adres email
        /// </summary>
        public String Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        /// <summary>
        /// Okresla dzwiek zwiazany z pojawieniem sie danej osoby, mozliwe stany:
        /// 0 - ustawienia globalne
        /// 1 - dzwiek powiadomienia wylaczony
        /// 2 - zostanie odtworzony dzwiek z pola podanego w polu path_avail
        /// </summary>
        public byte Avail
        {
            get
            {
                return avail;
            }
            set
            {
                avail = value;
            }
        }

        /// <summary>
        /// Sciezka do dzieku ktory zostanie odtworzony gdy dany numerek zmieni status oraz
        /// jezeli zmienna avail ustawiona jest na 2
        /// </summary>
        public String PathAvail
        {
            get
            {
                return path_avail;
            }
            set
            {
                path_avail = value;
            }
        }

        /// <summary>
        /// Okresla dzwiek zwiazany z odebraniem wiadomosci od danej osoby, mozliwe stany:
        /// 0 - ustawienia globalne
        /// 1 - dzwiek powiadomienia wylaczony
        /// 2 - zostanie odtworzony dzwiek z pola podanego w polu path_message
        /// </summary>
        public String Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Sciezka do dzieku ktory zostanie odtworzony po otrzymaniu wiadomosci od danego numerku
        /// jezeli zmienna message ustawiona jest na 2
        /// </summary>
        public String PathMessage
        {
            get
            {
                return path_message;
            }
            set
            {
                path_message = value;
            }
        }

        /// <summary>
        /// Okresla czy bedziemy dostepni dla danej osoby w trybie "tylko dla przyjaciol", mozliwe stany:
        /// 0 - dostepni
        /// 1 - niedostepni
        /// </summary>
        public byte Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }

        /// <summary>
        /// Telefon domowy
        /// </summary>
        public int Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
            }
        }

        /// <summary>
        /// Rok urodzenia - do katalogu publicznego
        /// </summary>
        public int BirthYear
        {
            get
            {
                return birthyear;
            }
            set
            {
                birthyear = value;
            }
        }

        /// <summary>
        /// Miasto zamieszkania - do katalogu publicznego
        /// </summary>
        public String City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
            }
        }

        /// <summary>
        /// Plec - do katalogu publicznego:
        /// 1 - kobieta
        /// 2 - mezczyzna
        /// </summary>
        public int Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        /// <summary>
        /// Jesli szukamy tylko aktywnych ma wartosc 1 - do katalogu publicznego
        /// </summary>
        public int ActiveOnly
        {
            get
            {
                return activeonly;
            }
            set
            {
                activeonly = value;
            }
        }

        /// <summary>
        /// Nazwisko panienskie - do katalogu publicznego
        /// </summary>
        public String FamilyName
        {
            get
            {
                return familyname;
            }
            set
            {
                familyname = value;
            }
        }

        /// <summary>
        /// Miejscowosc pochodzenia - do katalogu publicznego
        /// </summary>
        public String FamilyCity
        {
            get
            {
                return familycity;
            }
            set
            {
                familycity = value;
            }
        }

        /// <summary>
        /// Numer, od ktorego rozpoczac wyszukiwanie. Ma znaczenie, gdy kontynuujemy wyszukiwanie - do katalogu publicznego
        /// </summary>
        public int FmStart
        {
            get
            {
                return fmstart;
            }
            set
            {
                fmstart = value;
            }
        }

        /// <summary>
        /// Zwraca string nadajacy sie do wyslania do servera gg - lista kontaktow
        /// </summary>
        /// <returns>String ktory mozemy wyslac do servera gg</returns>
        public String getUserListString()
        {
            String ret;
            ret = name + ";";
            ret += lastname + ";";
            ret += nick + ";";
            ret += visiblenick + ";";
            ret += mobile_phone + ";";
            ret += group + ";";
            ret += uin + ";";
            ret += email + ";";
            ret += avail + ";";
            ret += path_avail + ";";
            ret += message + ";";
            ret += path_message + ";";
            ret += hidden + ";";
            ret += phone;
            return ret;
        }


        /// <summary>
        /// Parsuje cala list kontaktow zwrocona w postaci jednego duzego zlepka charow
        /// </summary>
        /// <param name="s">String z, ktorego pobierzemy dane</param>
        /// <returns>Tablica obiektow klasy GaduUser</returns>
        public static List<GaduUser> ParseUserListString(String s)
        {
            try
            {
                List<GaduUser> list = new List<GaduUser>();

                String[] vals = s.Split(';');
                String user;
                for (int i = 0; i < vals.Length; i += 13)
                {
                    user = String.Empty;
                    for (int j = i; j < ((i + 13) >= vals.Length ? vals.Length : i + 13); j++)
                        user += vals[j] + ';';

                    GaduUser gaduUser = new GaduUser();
                    string[] vals2 = user.Split(';');
                    if (vals2.Length > 13)     // kazdy uzytkownik musi miec przynajmiej 14 pol, jezeli tyle nie ma
                    {                           // uznajemy ze cos jest nie tak i nie dodajemy go
                        gaduUser.Name = vals2[0];
                        gaduUser.LastName = vals2[1];
                        gaduUser.Nick = vals2[2];
                        gaduUser.VisibleNick = vals2[3];
                        Int32.TryParse(vals2[4], out gaduUser.mobile_phone);
                        gaduUser.Group = vals2[5];
                        Int32.TryParse(vals2[6], out gaduUser.uin);
                        gaduUser.Email = vals2[7];
                        Byte.TryParse(vals2[8], out gaduUser.avail);
                        gaduUser.PathAvail = vals2[9];
                        gaduUser.Message = vals2[10];
                        gaduUser.PathMessage = vals2[11];
                        Byte.TryParse(vals2[12], out gaduUser.hidden);
                        Int32.TryParse(vals2[13], out gaduUser.phone);

                        list.Add(gaduUser);
                    }
                }

                return list;
            }
            catch (Exception exp)
            {
                throw new GaduUserException(exp.Message);
            }
        }

        /// <summary>
        /// Zwraca string nadajacy sie do wyslania do servera gg - katalog publiczny
        /// </summary>
        /// <returns>String ktory mozemy wyslac do servera gg</returns>
        public String getPubDirString()
        {
            String ret = "";
            if (uin > 0)
                ret += "FmNumber" + '\0' + uin + '\0';
            if (name != null && name.Length > 0)
                ret += "firstname" + '\0' + name + '\0';
            if (lastname != null && lastname.Length > 0)
                ret += "lastname" + '\0' + lastname + '\0';
            if (nick != null && nick.Length > 0)
                ret += "nickname" + '\0' + nick + '\0';
            if (birthyear > 0)
                ret += "birthyear" + '\0' + birthyear + '\0';
            if (gender > 0)
                ret += "gender" + '\0' + gender + '\0';
            if (activeonly == 1)
                ret += "ActiveOnly" + '\0' + activeonly + '\0';
            if (familyname != null && familyname.Length > 0)
                ret += "familyname" + '\0' + familyname + '\0';
            if (familycity != null && familycity.Length > 0)
                ret += "familycity" + '\0' + familycity + '\0';
            if (fmstart > 0)
                ret += "fmstart" + '\0' + fmstart + '\0';
            return ret;
        }

        /// <summary>
        /// Parsuje string tyczacy sie katalogu publicznego i zwraca liste obiektow klasy GaduUser
        /// </summary>
        /// <param name="s">String z, ktorego pobierzemy dane</param>
        /// <returns>Lista obiektow klasy GaduUser</returns>
        public static List<GaduUser> ParsePubDirString(String s)
        {
            try
            {
                List<GaduUser> list = new List<GaduUser>();
                GaduUser gaduUser;
                String[] users;
                String[] vars;
                users = s.Split(new String[] { "\0\0" }, StringSplitOptions.None);

                for (int i = 0; i < users.Length; i++)
                {
                    gaduUser = new GaduUser();
                    vars = users[i].Split('\0');
                    for (int j = 0; j < vars.Length; j += 2)
                    {
                        switch (vars[j])
                        {
                            case "FmNumber":
                                gaduUser.Uin = Convert.ToInt32(vars[j + 1]);
                                break;
                            case "firstname":
                                gaduUser.Name = vars[j + 1];
                                break;
                            case "lastname":
                                gaduUser.LastName = vars[j + 1];
                                break;
                            case "nickname":
                                gaduUser.Nick = vars[j + 1];
                                break;
                            case "birhyear":
                                gaduUser.BirthYear = Convert.ToInt32(vars[j + 1]);
                                break;
                            case "city":
                                gaduUser.City = vars[j + 1];
                                break;
                            case "gender":
                                gaduUser.Gender = Convert.ToInt32(vars[j + 1]);
                                break;
                            case "ActiveOnly":
                                gaduUser.ActiveOnly = Convert.ToInt32(vars[j + 1]);
                                break;
                            case "familyname":
                                gaduUser.FamilyName = vars[j + 1];
                                break;
                            case "familycity":
                                gaduUser.FamilyCity = vars[j + 1];
                                break;
                            case "fmstart":
                                gaduUser.FmStart = Convert.ToInt32(vars[j + 1]);
                                break;
                        }
                    }
                    list.Add(gaduUser);
                }

                return list;
            }
            catch (Exception exp)
            {
                throw new GaduUserException(exp.Message);
            }
        }
    }
}
