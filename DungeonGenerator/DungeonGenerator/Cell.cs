using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator
{
    class Cell
    {
        // Room properties
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsBoss { get; set; }
        public bool IsShop { get; set; }
        public bool IsVault { get; set; }
        public bool IsStart { get; set; }

        // A* Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Cell Parent { get; set; }

        // Constructor
        public Cell()
        {
            IsActive = false;
            IsBlocked = false;
            IsBoss = false;
            IsShop = false;
            IsStart = false;
            IsVault = false;
        }
        
        // Using Manhattan Distance 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }

        public void Print()
        {
            if (IsBoss)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            else if (IsShop)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else if (IsVault)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            }
            else if (IsStart)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
            }
            else if (IsActive)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
