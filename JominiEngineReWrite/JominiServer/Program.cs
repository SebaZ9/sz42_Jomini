using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace JominiServer
{
    public static class Program
    {

        //private static Server _Server;
        private static JominiGame.Game game;

        public static void Main()
        {
            Console.WriteLine("Jomini Engine Started2");
            game = new JominiGame.Game();
            Server server = new(game);
            Console.WriteLine("Jomini Engine Ended");
            //_Server = new Server();

        }
    }
}