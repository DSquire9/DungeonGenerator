using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator
{
    class Cell
    {
        private bool isActive;
        private bool isBoss;
        private bool isShop;
        private bool isVault;

        public Cell()
        {
            isActive = false;
            isBoss = false;
            isShop = false;
            isVault = false;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public bool IsBoss
        {
            get { return isBoss; }
            set { isBoss = value; }
        }
        public bool IsShop
        {
            get { return isShop; }
            set { isShop = value; }
        }
        public bool IsVault
        {
            get { return isVault; }
            set { isVault = value; }
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
            else if (isActive)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
