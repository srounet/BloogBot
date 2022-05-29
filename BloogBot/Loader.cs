using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BloogBot
{
    internal class Loader
    {
        private static Thread thread;

        private static int Load(string args)
        {
            thread = new Thread(App.Main);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return 1;
        }
    }
}