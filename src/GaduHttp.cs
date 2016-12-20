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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing;

namespace DotGadu
{
    /// <summary>
    /// Wyjatek zwracany w przypadku bledu pobierania danych przez protokol HTTP
    /// </summary>
    public class GaduHttpException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu pobierania danych przez protokol HTTP
        /// </summary>
        public GaduHttpException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany w przypadku bledu pobierania tokena z HTTP
    /// </summary>
    public class GaduTokenException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu pobierania tokena z HTTP
        /// </summary>
        public GaduTokenException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany w przypadku bledu rejestrowania konta
    /// </summary>
    public class GaduRegisterException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu rejestrowania konta
        /// </summary>
        public GaduRegisterException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany w przypadku bledu usuwania konta
    /// </summary>
    public class GaduDeleteException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu usuwania konta
        /// </summary>
        public GaduDeleteException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany w przypadku bledu zmiany hasla
    /// </summary>
    public class GaduChangePasswordException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu zmiany hasla
        /// </summary>
        public GaduChangePasswordException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Wyjatek zwracany w przypadku bledu przypomnienia hasla
    /// </summary>
    public class GaduRemindException : Exception    // nasz wlasny wyjatek
    {
        /// <summary>
        /// Wyjatek zwracany w przypadku bledu przypomnienia hasla
        /// </summary>
        public GaduRemindException(String message) : base(message) { }  // dziedziczenie konstruktora
    }

    /// <summary>
    /// Klasa udostepniajaca obsluge uslug HTTP
    /// </summary>
    public class GaduHttp
    {
        /// <summary>
        /// Laczy sie z serverem HTTP, wysyla zadzanie i zwraca odpowiedz
        /// </summary>
        /// <param name="url">Pelny adres URL</param>
        /// <param name="method">GET/POST</param>
        /// <returns></returns>
        private String getResponse(String url, String method)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98";
                req.Method = method;
                req.ContentLength = 0;
                req.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader streamReader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("windows-1250"));
                String ret = streamReader.ReadToEnd();
                streamReader.Close();
                resp.Close();
                return ret;
            }
            catch (Exception e)
            {
                throw new GaduHttpException(e.Message);
            }
        }

        /// <summary>
        /// Zwraca hash, potrzebny do operacji HTTP, wziete z Sharp THGG
        /// </summary>
        /// <param name="hashParams">Parametry, ktorych hash chcemy wyliczyc</param>
        /// <returns>Hash parametrow</returns>
        private int getHttpHash(string[] hashParams)
        {
            int b = -1, i, j;
            uint a, c;

            if (hashParams.Length == 0)
                return -1;

            for (i = 0; i < hashParams.Length; i++)
            {
                for (j = 0; j < hashParams[i].Length; j++)
                {
                    c = hashParams[i][j];
                    a = (uint)(c ^ b) + (c << 8);
                    b = (int)(a >> 24) | (int)(a << 8);
                }
            }
            return System.Math.Abs(b);
        }

        /// <summary>
        /// Pobiera i zwraca token
        /// </summary>
        /// <param name="tokenId">Tutaj wpisany zostanie id pobranego tokenu</param>
        /// <param name="tokenImage">Tutaj zostanie wpisany obrazek prezentujacy token</param>
        public void getToken(out String tokenId, out Image tokenImage)
        {
            try
            {
                String resp = getResponse("http://register.gadu-gadu.pl/appsvc/regtoken.asp", "POST");
                String[] info;
                info = resp.Split(new char[] { '\r', '\n' });
                WebClient webClient = new WebClient();
                tokenId = info[2];
                tokenImage = Image.FromStream(webClient.OpenRead(info[4] + "?tokenid=" + info[2]));
            }
            catch (Exception e)
            {
                throw new GaduTokenException(e.Message);
            }
        }

        /// <summary>
        /// Rejestruje nowe konto
        /// </summary>
        /// <param name="password">Haslo do konta</param>
        /// <param name="email">E-Mail konta</param>
        /// <param name="tokenid">Id pobranego wczesniej tokenu</param>
        /// <param name="tokenval">Odczytany ciag znakow z obrazka</param>
        /// <returns></returns>
        public int registerAccount(String password, String email, String tokenid, String tokenval)
        {
            try
            {
                String resp;
                String url = "http://register.gadu-gadu.pl/appsvc/fmregister3.asp";
                url += "?pwd=" + password;
                url += "&email=" + email;
                url += "&tokenid=" + tokenid;
                url += "&tokenval=" + tokenval;
                url += "&code=" + getHttpHash(new String[] { email, password }).ToString();
                resp = getResponse(url, "POST");
                if (resp == "bad_tokenval")
                    throw new GaduRegisterException(resp);
                else
                {
                    String[] vals;
                    vals = resp.Split(':');
                    return Convert.ToInt32(vals[1]);
                }
            }
            catch (Exception e)
            {
                throw new GaduRegisterException(e.Message);
            }
        }

        /// <summary>
        /// Usuwa konto
        /// </summary>
        /// <param name="uin">Numer gg</param>
        /// <param name="password">Haslo do konta</param>
        /// <param name="tokenid">Id pobranego wczesniej tokenu</param>
        /// <param name="tokenval">Odczytany ciag znakow z obrazka</param>
        public void deleteAccount(int uin, String password, String tokenid, String tokenval)
        {
            try
            {
                String resp, url;
                url = "http://register.gadu-gadu.pl/appsvc/fmregister3.asp";
                url += "?fmnumber=" + uin;
                url += "&fmpwd=" + password;
                url += "&delete=1";
                url += "&pwd=%2D388046464";
                url += "&email=deletedaccount@gadu-gadu.pl";
                url += "&tokenid=" + tokenid;
                url += "&tokenval=" + tokenval;
                url += "&code=" + getHttpHash(new String[] { "deletedaccount@gadu-gadu.pl", password }).ToString();
                resp = getResponse(url, "POST");
                if (resp != "reg_success:" + uin)
                    throw new GaduDeleteException(resp);
            }
            catch (Exception e)
            {
                throw new GaduDeleteException(e.Message);
            }
        }

        /// <summary>
        /// Zmienia haslo dla danego numeru gg
        /// </summary>
        /// <param name="uin">Numer gg</param>
        /// <param name="email">E-Mail naszego konta</param>
        /// <param name="OldPass">Stare haslo</param>
        /// <param name="NewPass">Nowe haslo</param>
        /// <param name="tokenid">Id pobranego tokenu</param>
        /// <param name="tokenval">Odczytany ciag znakow z obrazka</param>
        public void changePassword(int uin, String email, String OldPass, String NewPass, String tokenid, String tokenval)
        {
            try
            {
                String resp, url;
                url = "http://register.gadu-gadu.pl/appsvc/fmregister3.asp";
                url += "?fmnumber=" + uin;
                url += "&fmpwd=" + OldPass;
                url += "&pwd=" + NewPass;
                url += "&email=" + email;
                url += "&tokenid=" + tokenid;
                url += "&tokenval=" + tokenval;
                url += "&code=" + getHttpHash(new String[] { "deletedaccount@gadu-gadu.pl", NewPass }).ToString();
                resp = getResponse(url, "POST");
                if (resp != "reg_success:" + uin)
                    throw new GaduChangePasswordException(resp);
            }
            catch (Exception e)
            {
                throw new GaduChangePasswordException(e.Message);
            }
        }

        /// <summary>
        /// Przypomnienie hasla, wysyla haslo na e-mail
        /// </summary>
        /// <param name="uin">Numer gg</param>
        /// <param name="tokenid">Id pobranego tokenu</param>
        /// <param name="tokenval">Odczytany ciag znakow z obrazka</param>
        public void remindPassword(int uin, String tokenid, String tokenval)
        {
            try
            {
                String resp, url;
                url = "http://retr.gadu-gadu.pl/appsvc/fmsendpwd3.asp";
                url += "?userid=" + uin;
                url += "&tokenid=" + tokenid;
                url += "&tokenval=" + tokenval;
                url += "&code=" + getHttpHash(new String[] { uin.ToString() }).ToString();
                resp = getResponse(url, "POST");
                if (resp != "pwdsend_success")
                    throw new GaduRemindException(resp);
            }
            catch (Exception e)
            {
                throw new GaduRemindException(e.Message);
            }
        }
    }
}
