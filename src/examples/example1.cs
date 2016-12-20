// DotGadu - example1
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using DotGadu;

namespace DotGaduTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            GaduServer gaduServer = new GaduServer("91.197.13.26", 8074);
            Gadu gadu = new Gadu();

            gadu.Reciever.OnLoginOK += new OnLoginOKHandler(loginok);
            gadu.Reciever.OnLoginFailed += new OnLoginFailedHandler(loginfailed);
            gadu.Reciever.OnLoginNeedEmail += new OnLoginNeedEmailHandler(loginneedemail);

            gadu.Sender.OnPacketSent += new OnPacketSentHandler(packet);

            gadu.Connect(gaduServer);

            gadu.Login(179824, "password");

            System.Threading.Thread.Sleep(5000);

            gadu.Logout();
            gadu.Disconnect();
        }

        public static void packet(IGaduPacket packet)
        {
            Console.WriteLine("Packet Sent...");
        }

        public static void loginok()
        {
            Console.WriteLine("Logowanie OK");
        }

        public static void loginfailed()
        {
            Console.WriteLine("Logowanie zakonczylo sie niepowodzeniem");
        }

        public static void loginneedemail()
        {
            Console.WriteLine("Trzeba uzupelnic email na koncie gg");
        }
    }
}
