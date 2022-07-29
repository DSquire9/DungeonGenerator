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
        Cell boss;
        Cell vault;
        Cell shop;
        Cell start;
        private int mapSize;


        public Map(int mapSize)
        {
            this.mapSize = mapSize;
            if (this.mapSize < 5)
            {
                this.mapSize = 5;
            }
            this.rng = new Random();
            boss = new Cell();
            vault = new Cell();
            shop = new Cell();
            start = new Cell();
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
            MakePath(start, boss);
            MakePath(start, vault);
            MakePath(start, shop);
        }

        private void SeedKeyRooms()
        {
            //Boss
            
            boss.X = rng.Next(mapSize);
            boss.Y = rng.Next(mapSize);
            grid[boss.X, boss.Y] = boss;
            boss.IsBoss = true;
            boss.IsActive = true;
            //DisableSurrounding(boss.X, boss.Y);

            //Vault
            do
            {
                vault.X = rng.Next(mapSize);
                vault.Y = rng.Next(mapSize);
            } while ((vault.X == boss.X - 1 || vault.X == boss.X || vault.X == boss.X + 1) && 
                    (vault.Y == boss.Y - 1 || vault.Y == boss.Y || vault.Y == boss.Y + 1));
            grid[vault.X, vault.Y] = vault;
            vault.IsVault = true;
            vault.IsActive = true;
            //DisableSurrounding(vault.X, vault.Y);

            //Shop
            do
            {
                shop.X = rng.Next(mapSize);
                shop.Y = rng.Next(mapSize);
            } while (((shop.X == boss.X - 1 || shop.X == boss.X || shop.X == boss.X + 1) &&
                      (shop.Y == boss.Y - 1 || shop.Y == boss.Y || shop.Y == boss.Y + 1))  
                    || 
                    ((shop.X == vault.X - 1 || shop.X == vault.X || shop.X == vault.X + 1) &&
                     (shop.Y == vault.Y - 1 || shop.Y == vault.Y || shop.Y == vault.Y + 1)));
            grid[shop.X, shop.Y] = shop;
            shop.IsShop = true;
            shop.IsActive = true;
            //DisableSurrounding(shop.X, shop.Y);


            do
            {
                start.X = rng.Next(mapSize);
                start.Y = rng.Next(mapSize);
            } while ((start.X == vault.X && start.Y == vault.Y)
                    ||
                    ((start.X == boss.X - 1 || start.X == boss.X || start.X == boss.X + 1) &&
                      (start.Y == boss.Y - 1 || start.Y == boss.Y || start.Y == boss.Y + 1))
                    ||
                    (start.X == shop.X && start.Y == shop.Y)
                    );
            grid[start.X, start.Y] = start;
            start.IsStart = true;
            start.IsActive = true;
            Console.WriteLine("Key rooms Seeded");
        }

        private void MakePath(Cell startpoint, Cell endpoint)
        {
            startpoint.SetDistance(endpoint.X, endpoint.Y);
            List<Cell> activeCells = new List<Cell>();
            activeCells.Add(startpoint);
            List<Cell> visitedCells = new List<Cell>();

            while(activeCells.Any())
            {
                Cell checkCell = activeCells.OrderBy(x => x.CostDistance).First();
                
                // Stop if we are at the endpoint
                if (checkCell.X == endpoint.X && checkCell.Y == endpoint.Y)
                {
                    Cell cell = checkCell;
                    if (endpoint.IsBoss)
                    {
                        cell.IsBoss = true;
                    }
                    if (endpoint.IsShop)
                    {
                        cell.IsShop = true;
                    }
                    if (endpoint.IsVault)
                    {
                        cell.IsVault = true;
                    }
                    while (true)
                    {
                        cell.IsActive = true;
                        grid[cell.X, cell.Y] = cell;
                        cell = cell.Parent;
                        if (cell.Parent == null)
                        {
                            return;
                        }
                    }
                }

                visitedCells.Add(checkCell);
                activeCells.Remove(checkCell);

                List<Cell> walkableCells = GetWalkableCells(checkCell, endpoint);

                foreach(Cell walkableCell in walkableCells)
                {
                    if (visitedCells.Any(x => x.X == walkableCell.X && x.Y == walkableCell.Y))
                        continue;
                    

                    if (activeCells.Any(x => x.X == walkableCell.X && x.Y == walkableCell.Y))
                    {
                        Cell existingCell = activeCells.First(x => x.X == walkableCell.X && x.Y == walkableCell.Y);
                        if (existingCell.CostDistance > checkCell.CostDistance)
                        {
                            activeCells.Remove(existingCell);
                            activeCells.Add(walkableCell);
                        }
                    }
                    else
                    {
                        activeCells.Add(walkableCell);
                    }
                }
            }

            Console.WriteLine("Pathfinding error");
        }

        private List<Cell> GetWalkableCells(Cell current, Cell target)
        {
            List<Cell> possibleCells = new List<Cell>()
            {
                new Cell{X = current.X, Y = current.Y - 1, Parent = current, Cost = current.Cost + 1 },
                new Cell{X = current.X, Y = current.Y + 1, Parent = current, Cost = current.Cost + 1 },
                new Cell{X = current.X - 1, Y = current.Y, Parent = current, Cost = current.Cost + 1 },
                new Cell{X = current.X + 1, Y = current.Y, Parent = current, Cost = current.Cost + 1 }
            };

            possibleCells.ForEach(cell => cell.SetDistance(target.X, target.Y));


            return possibleCells
                .Where(cell => cell.X >= 0 && cell.X < mapSize)
                .Where(cell => cell.Y >= 0 && cell.Y < mapSize)
                .Where(cell => cell.IsShop == false && cell.IsVault == false && cell.IsBoss == false)
                .ToList();
        }

        private void DisableSurrounding(int x, int y)
        {

            /*
            if (x - 1 > -1)
            {
                grid[x - 1, y].IsActive = false;
            }
            if (x + 1 < mapSize)
            {
                grid[x + 1, y].IsActive = false;
            }
            if (y - 1 > -1)
            {
                grid[x, y - 1].IsActive = false;
            }
            if (y + 1 < mapSize)
            {
                grid[x, y + 1].IsActive = false;
            }
            */



            bool again = true;
            do
            {
                int num = rng.Next(4);
                switch (num)
                {
                    case 0:
                        if (x - 1 > -1)
                        {
                            grid[x - 1, y].IsActive = true;
                            again = false;
                        }
                        break;
                    case 1:
                        if (x + 1 < mapSize)
                        {
                            grid[x + 1, y].IsActive = true;
                            again = false;
                        }
                        break;
                    case 2:
                        if (y - 1 > -1)
                        {
                            grid[x, y - 1].IsActive = true;
                            again = false;
                        }
                        break;
                    case 3:
                        if (y + 1 < mapSize)
                        {
                            grid[x, y + 1].IsActive = true;
                            again = false;
                        }
                        break;
                    default:
                        Console.WriteLine("Error in disable switch");
                        break;
                }
            } while (again);
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
