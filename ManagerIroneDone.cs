using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using static System.Net.Mime.MediaTypeNames;

namespace IronDoneServer
{
    internal class ManagerIroneDone
    {
        WebSocketServer _wss;
        ConcurrentQueue<Missile> _missilesQueueDone;
        private readonly SemaphoreSlim _ironDomeSemaphore;

        public ManagerIroneDone(WebSocketServer wss, ConcurrentQueue<Missile> missilesQueue)
        {
            this._wss = wss;
            this._missilesQueueDone = missilesQueue;
            this._ironDomeSemaphore = new SemaphoreSlim(IronDomeList.Count);
        }

        List<string> IronDomeList = new List<string>() { "IronDone1", "IronDone2", };

        public void Start()
        {
            foreach (var name in IronDomeList)
            {
                var InterceptorThread = new Thread(() => Interception(name))
                {
                    Name = name,
                    Priority = ThreadPriority.Normal
                };
                InterceptorThread.Start();
            }
        }

        public async void Interception(string NameIron)
        {
            IronDone ironDone = new IronDone(NameIron, 5);

            while (true)
            {
                if (ironDone.AmountMissiles <= 2)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                }
                else
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
                }
                this._ironDomeSemaphore.Wait();
                if (this._missilesQueueDone.TryDequeue(out Missile mis))
                {
                    bool result = await ironDone.handleMissile(mis);
                    var message = new
                    {
                        Id = mis.Id,
                        Missile = mis.Name,
                        intercepted = result,
                        by = ironDone.Name,
                        remaining = ironDone.AmountMissiles,
                        Damage = mis.Damage
                    };
                    Console.WriteLine(message.ToString());

                    var json = JsonSerializer.Serialize(message);

                    this._wss.WebSocketServices["/MissileHandler"].Sessions.Broadcast(json);
                }
                this._ironDomeSemaphore.Release();
            }
        }
    }
}
