using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 50; i++)
            {
                Map dungeon = new Map(10);
                dungeon.Print();
                Console.WriteLine("----------");
            }
            
        }
    }
}
