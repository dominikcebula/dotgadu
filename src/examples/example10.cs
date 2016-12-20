// DotGadu - example10
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        private static bool end = false;
        private static Gadu gadu;

        public static void Main(string[] args)
        {
            GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
            gadu = new Gadu();

            gadu.Reciever.OnLoginOK += new OnLoginOKHandler(LoginOK);
            gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(LoginFailed);
            gadu.Reciever.OnGaduNotifyReplay += new OnGaduNotifyReplayHandler(OnNotify);

            gadu.Connect(gaduServer);
            gadu.Login(179824, "");

            while (end == false)
                System.Threading.Thread.Sleep(100);

            gadu.Disconnect();
        }

        public static void OnNotify(GaduPacketNotifyReplay77 gaduPacketNotifyReplay77)
        {
            String strstatus = "";

            switch (gaduPacketNotifyReplay77.Status)
            {
                case GaduPacketConstans.GG_STATUS_AVAIL:
                    strstatus = "dostepny";
                    break;
                case GaduPacketConstans.GG_STATUS_AVAIL_DESCR:
                    strstatus = "dostepny z opisem";
                    break;
                case GaduPacketConstans.GG_STATUS_BUSY:
                    strstatus = "zajety";
                    break;
                case GaduPacketConstans.GG_STATUS_BUSY_DESCR:
                    strstatus = "zajety z opisem";
                    break;
                case GaduPacketConstans.GG_STATUS_INVISIBLE:
                    strstatus = "niewidoczny";
                    break;
                case GaduPacketConstans.GG_STATUS_INVISIBLE_DESCR:
                    strstatus = "niewidoczny z opisem";
                    break;
            }

            Console.WriteLine(gaduPacketNotifyReplay77.Uin + " " + strstatus);
        }

        public static void LoginOK()
        {
            Console.WriteLine("Login OK");
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_BUSY, "");

            GaduPacketNotify gpn = new GaduPacketNotify();
            gpn.Uin = 3837462;
            gpn.Type = GaduPacketConstans.GG_USER_BUDDY;
            gadu.Notifier.Add(gpn);
            gadu.Notifier.sendNotify();

            gpn = new GaduPacketNotify();
            gpn.Uin = 123;
            gpn.Type = GaduPacketConstans.GG_USER_BUDDY;
            gadu.Notifier.addNotify(gpn);
        }

        public static void LoginFailed()
        {
            Console.WriteLine("Login Failed");
            end = true;
        }
    }
}
