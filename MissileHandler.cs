using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace IronDoneServer
{
    internal class MissileHandler : WebSocketBehavior
    {
        private readonly WebSocketServer _wss;
        private readonly ConcurrentQueue<Missile> _missilesQueueDone;
        private readonly ConcurrentQueue<Missile> _missilesQueueChez;

        // Dictionary for
        Dictionary<string, string> WhoIntercepted = new Dictionary<string, string>()
        {
            { "Kasam", "IronDoneChez" },
            { "grad", "IronDone" },
            { "zalal", "IronDone" },
            { "Fatah", "IronDoneChez" }
        };

        // Dictionary for
        Dictionary<string, int> dictDamage = new Dictionary<string, int>()
        {
            { "Kasam", 50 },
            { "grad", 104 },
            { "zalal", 150 },
            { "Fatah", 100 }
        };

        public MissileHandler(
            WebSocketServer wss,
            ConcurrentQueue<Missile> missilesQueueDone,
            ConcurrentQueue<Missile> missilesQueueChez
        )
        {
            this._wss = wss;
            this._missilesQueueDone = missilesQueueDone;
            this._missilesQueueChez = missilesQueueChez;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Missile mis = JsonSerializer.Deserialize<Missile>(e.Data);

            mis.Damage = dictDamage[mis.Name];

            switch (WhoIntercepted[mis.Name])
            {
                case "IronDoneChez":
                    this._missilesQueueChez.Enqueue(mis);
                    break;
                case "IronDone":
                    this._missilesQueueDone.Enqueue(mis);
                    break;
            }

            // Print data to console
            Console.WriteLine("data got is: " + e.Data);
        }

        public void BroadcastStatus(string message)
        {
            this._wss.WebSocketServices["/MissileHandler"].Sessions.Broadcast(message);
        }
    }
}
