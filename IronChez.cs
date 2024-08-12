using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronDoneServer
{
    internal class IronChez
    {
        public string Name { get; set; }
        public int AmountMissiles { get; set; }

        public IronChez(string name, int amountMissiles)
        {
            Name = name;
            this.AmountMissiles = amountMissiles;
        }

        public async Task SetAmountPlus(int amountMissiles)
        {
            await Task.Delay(2000 * amountMissiles);
            this.AmountMissiles += amountMissiles;
        }

        public void setAmountMinus()
        {
            this.AmountMissiles--;
        }

        public async Task<bool> handleMissile(Missile missile)
        {
            if (this.AmountMissiles == 0)
            {
                await this.SetAmountPlus(5);
            }
            await Task.Delay(10000);
            Random random = new Random();
            bool intercepted = random.Next(0, 2) == 1;
            this.setAmountMinus();
            return intercepted;
        }
    }
}
