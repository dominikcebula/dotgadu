// DotGadu - example3
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        public static bool exit = false;
        public static Gadu gadu;

        public static void Main(string[] args)
        {
            GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
            gadu = new Gadu();

            gadu.Reciever.OnLoginOK += new OnLoginOKHandler(loginok);
            gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(loginfailed);

            gadu.Sender.OnPacketSent += new OnPacketSentHandler(packet);

            gadu.Connect(gaduServer);

            gadu.Login(179824, "passwd");

            while (!exit)
                System.Threading.Thread.Sleep(1000);

            Console.WriteLine("Koncze dzialanie");

            gadu.Logout();
            gadu.Disconnect();
        }

        public static void packet(IGaduPacket packet)
        {
            Console.WriteLine("Packet Sent...");
        }

        public static void loginok()
        {
            Console.WriteLine("Logowanie OK :)");

            gadu.changeStatus(GaduPacketConstans.GG_STATUS_AVAIL, String.Empty);	// dostepny
            System.Threading.Thread.Sleep(5000);
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_AVAIL_DESCR, "Dostepny z opisem");	// dostepny z opisem
            System.Threading.Thread.Sleep(5000);
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_BUSY, String.Empty);	// zajety
            System.Threading.Thread.Sleep(5000);
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_BUSY_DESCR, "Zajety z opisem");	// zajety z opisem
            System.Threading.Thread.Sleep(5000);
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_INVISIBLE, String.Empty);	// niewidzialny
            System.Threading.Thread.Sleep(5000);
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_INVISIBLE_DESCR, "Nieiwdzialny z opisem");	// niewidzialny z opisem
            System.Threading.Thread.Sleep(5000);
            exit = true;
        }

        public static void loginfailed()
        {
            Console.WriteLine("Blad logowania :(");
            exit = true;	// nie udalo nam sie zalogowac, oznacz exit na true, program za chwile zakonczy dzialanie
        }
    }
}
