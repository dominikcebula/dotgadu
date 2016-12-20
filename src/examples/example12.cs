// DotGadu - example12
// Dominik Cebula
// dominikcebula@gmail.com

using System;
using System.Collections.Generic;
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
            gadu.Reciever.OnPubDirReplay += new OnPubDirReplayHandler(PubDirReplay);

            gadu.Connect(gaduServer);
            gadu.Login(179824, "");

            while (end == false)
                System.Threading.Thread.Sleep(100);

            gadu.Disconnect();
        }

        public static void PubDirReplay(GaduPacketPubDir50 gaduPacketPubDirReplay50)
        {
            List<GaduUser> list;
            list = GaduUser.ParsePubDirString(gaduPacketPubDirReplay50.Data);
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine(list[i].Nick + " " + list[i].Uin);
        }

        public static void LoginOK()
        {
            Console.WriteLine("Login OK");
            gadu.changeStatus(GaduPacketConstans.GG_STATUS_BUSY, "");

            GaduUser gaduUser = new GaduUser();
            gaduUser.Name = "Tomek";

            gadu.PubDir.Search(gaduUser);
        }

        public static void LoginFailed()
        {
            Console.WriteLine("Login Failed");
            end = true;
        }
    }
}

