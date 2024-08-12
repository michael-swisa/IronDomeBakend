using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IronDoneServer;
using WebSocketSharp.Server;

namespace IronDoneServer
{
    class Program
    {
        // Missiles Queue
        public static ConcurrentQueue<Missile> missilesQueueDone = new ConcurrentQueue<Missile>();
        public static ConcurrentQueue<Missile> missilesQueueChez = new ConcurrentQueue<Missile>();

        static void Main(string[] args)
        {
            string port = "3108";
            WebSocketServer wss = new WebSocketServer($"ws://localhost:{port}");
            wss.AddWebSocketService<MissileHandler>(
                "/MissileHandler",
                () => new MissileHandler(wss, missilesQueueDone, missilesQueueChez)
            );

            ManagerIroneDone ManagerIronDone = new ManagerIroneDone(wss, missilesQueueDone);
            ManagerIronDone.Start();
            ManagerIroneChez ManagerIronChez = new ManagerIroneChez(wss, missilesQueueChez);
            ManagerIronChez.Start();

            wss.Start();
            Console.WriteLine("Backend server is running. Press Enter to exit...");
            Console.ReadLine();
            wss.Stop();
        }
    }
}
