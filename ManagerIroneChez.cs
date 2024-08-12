using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace IronDoneServer
{
    internal class ManagerIroneChez
    {
        WebSocketServer _wss;
        ConcurrentQueue<Missile> _missilesQueueChez;
        private readonly SemaphoreSlim _ironDomeSemaphore;

        public ManagerIroneChez(WebSocketServer wss, ConcurrentQueue<Missile> missilesQueueChez)
        {
            this._wss = wss;
            this._missilesQueueChez = missilesQueueChez;
            this._ironDomeSemaphore = new SemaphoreSlim(IronChezList.Count);
        }

        List<string> IronChezList = new List<string>() { "IronDoneChez1", "IronDoneChez2", };

        public void Start()
        {
            foreach (var name in IronChezList)
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
            IronChez ironChez = new IronChez(NameIron, 5);

            while (true)
            {
                if (ironChez.AmountMissiles <= 2)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                }
                else
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
                }
                this._ironDomeSemaphore.Wait();
                if (this._missilesQueueChez.TryDequeue(out Missile mis))
                {
                    bool result = await ironChez.handleMissile(mis);
                    var message = new
                    {
                        Missile = mis.Name,
                        intercepted = result,
                        by = ironChez.Name,
                        remaining = ironChez.AmountMissiles
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
