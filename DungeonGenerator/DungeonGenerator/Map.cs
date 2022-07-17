using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator
{
    class Map
    {
        private Cell[,] grid;
        private Random rng;
        private int mapSize;

        public Map(int mapSize)
        {
            this.mapSize = mapSize;
            if (this.mapSize < 5)
            {
                this.mapSize = 5;
            }
            this.rng = new Random();
            GenerateMap();
        }

        private void GenerateMap()
        {
            grid = new Cell[mapSize, mapSize];
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    grid[x, y] = new Cell();
                }
            }
            Console.WriteLine("Map created");
            SeedKeyRooms();
        }

        private void SeedKeyRooms()
        {
            //Boss
            int bossX = rng.Next(mapSize);
            int bossY = rng.Next(mapSize);
            grid[bossX, bossY].IsBoss = true;
            grid[bossX, bossY].IsActive = true;

            //Vault
            int vaultX;
            int vaultY;
            do
            {
                vaultX = rng.Next(mapSize);
                vaultY = rng.Next(mapSize);
            } while ((vaultX == bossX - 1 || vaultX == bossX || vaultX == bossX + 1) && 
                    (vaultY == bossY - 1 || vaultY == bossY || vaultY == bossY + 1));
            grid[vaultX, vaultY].IsVault = true;
            grid[vaultX, vaultY].IsActive = true;

            //Shop
            int shopX;
            int shopY;
            do
            {
                shopX = rng.Next(mapSize);
                shopY = rng.Next(mapSize);
            } while (((shopX == bossX - 1 || shopX == bossX || shopX == bossX + 1) &&
                      (shopY == bossY - 1 || shopY == bossY || shopY == bossY + 1))  
                    || 
                    ((shopX == vaultX - 1 || shopX == vaultX || shopX == vaultX + 1) &&
                     (shopY == vaultY - 1 || shopY == vaultY || shopY == vaultY + 1)));
            grid[shopX, shopY].IsShop = true;
            grid[shopX, shopY].IsActive = true;
            Console.WriteLine("Key rooms Seeded");
        }

        public void Print()
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    grid[x, y].Print();
                }
                Console.WriteLine();
            }
        }
    }
}
