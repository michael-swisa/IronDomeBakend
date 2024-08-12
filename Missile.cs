using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace IronDoneServer
{
    internal class Missile
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public float Muss { get; set; }
        public Dictionary<string, int> Origin { get; set; }
        public Dictionary<string, int> Anjle { get; set; }
        public int Time { get; set; }
        public float Damage { get; set; }

        public Dictionary<string, float> dictDamage = new Dictionary<string, float>()
        {
            { "Kasam", 25 },
            { "grad", 65 },
            { "zalal", 85 },
            { "Fatah", 15 }
        };

        public Missile(
            string name,
            int speed,
            float muss,
            Dictionary<string, int> origin,
            Dictionary<string, int> anjle,
            int time
        )
        {
            Name = name;
            Speed = speed;
            Muss = muss;
            Origin = origin;
            Anjle = anjle;
            Time = time;
            Damage = dictDamage[name];
        }
    }
}
