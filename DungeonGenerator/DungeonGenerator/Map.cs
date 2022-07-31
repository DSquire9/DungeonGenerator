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
            List<Cell> bossPath = RandomizePath(MakePath(start, boss), start, boss);
            List<Cell> vaultPath = RandomizePath(MakePath(start, vault), start, vault);
            List<Cell> shopPath = RandomizePath(MakePath(start, shop), start, shop);

            SetPath(bossPath);
            SetPath(vaultPath);
            SetPath(shopPath);
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

        private List<Cell> MakePath(Cell startpoint, Cell endpoint)
        {
            List<Cell> witness = new List<Cell>();

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
                        //cell.IsActive = true;
                    }
                    if (endpoint.IsShop)
                    {
                        cell.IsShop = true;
                        //cell.IsActive = true;
                    }
                    if (endpoint.IsVault)
                    {
                        cell.IsVault = true;
                        //cell.IsActive = true;
                    }
                    while (true)
                    {
                        witness.Add(cell);
                        //cell.IsActive = true;
                        grid[cell.X, cell.Y] = cell;
                        cell = cell.Parent;
                        if (cell.Parent == null)
                        {
                            return witness;
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

            //Console.WriteLine("Pathfinding error");
            return null;
        }

        private List<Cell> GetWalkableCells(Cell current, Cell target)
        {
            // Figure out what the target is so we don't cross it with another point
            bool isShop = false;
            if (target.X == shop.X && target.Y == shop.Y)
            {
                isShop = true;
            }
            bool isBoss = false;
            if (target.X == boss.X && target.Y == boss.Y)
            {
                isBoss = true;
            }
            bool isVault = false;
            if (target.X == vault.X && target.Y == vault.Y)
            {
                isVault = true;
            }


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
                .Where(cell => grid[cell.X, cell.Y].IsBlocked == false)
                .Where(cell => (grid[cell.X, cell.Y].IsShop == false || isShop) && (grid[cell.X, cell.Y].IsVault == false || isVault) && (grid[cell.X, cell.Y].IsBoss == false || isBoss))
                .ToList();
        }

        private List<Cell> RandomizePath(List<Cell> witness, Cell startpoint, Cell endpoint)
        {
            // Get a list of open cells
            List<Cell> open = new List<Cell>();
            List<Cell> temp = new List<Cell>();
            
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (!grid[x,y].IsBlocked && !grid[x,y].IsShop && !grid[x, y].IsBoss && !grid[x, y].IsVault)
                    {
                        open.Add(grid[x, y]);
                    }
                }
            }

            while (true)
            {
                if (!open.Any())
                {
                    // Reset for next path
                    foreach (Cell cell in temp)
                    {
                        cell.IsBlocked = false;
                    }
                    return witness;
                }

                // Get a random open cell and remove it from open
                Cell c = open[rng.Next(open.Count)];
                open.Remove(c);
                temp.Add(c);
                // Set C to blocked
                c.IsBlocked = true;

                if (witness.Contains(c))
                {
                    List<Cell> new_path = MakePath(startpoint, endpoint);
                    if (new_path == null)
                    {
                        //c.IsActive = true;
                        c.IsBlocked = false;

                    }
                    else
                    {
                        witness = new_path;
                    }
                }




            }
        }

        private void SetPath(List<Cell> finalPath)
        {
            foreach (Cell cell in finalPath)
            {
                grid[cell.X, cell.Y].IsActive = true;
            }
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
