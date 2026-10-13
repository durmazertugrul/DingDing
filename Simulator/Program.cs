namespace Match3_Console_Engine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
                Grid grid = new Grid(8, 8);
                BoardInitializer initializer = new BoardInitializer();
                MatchDetector detector = new MatchDetector();

                initializer.FillRandom(grid); //Açılışta tahta rastgele taşlarla dolar.

                Console.WriteLine("Initial Board:");
                grid.Print();

                bool matchFound;
                // Hiç eşleşme kalmayana kadar devam eder.
                do
                {
                    List<(int x, int y)> matches = detector.FindMatches(grid);
                    matchFound = matches.Count > 0;

                    if (matchFound)
                    {
                        Console.WriteLine("Match Found. Clearing...");
                        detector.ClearMatches(grid, matches);
                        grid.Print();

                        Console.WriteLine("Applying Gravity...");
                        grid.ApplyGravity();
                        grid.Print();

                        Console.WriteLine("Spawning New Tiles...");
                        initializer.SpawnNewTiles(grid);
                        grid.Print();
                    }

                } while (matchFound); //Yeni eklenen taşlarla birlikte yine eşleşme varsa işlem yapmaya devam eder.

                Console.WriteLine("Final Stable Board:");
                grid.Print();


                //Ekran, tablo tamamlanır tamamlanmaz kapanmaması için bekletir.
                Console.WriteLine("The process is complete. Press any key to exit...");
                Console.ReadKey();
        }
        }
    }

