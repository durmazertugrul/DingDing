using System;
using System.Collections.Generic;


namespace DingDing.Core
{
    internal class Grid
    {
        //Oyun alanı boyut ve hücre değişkenleri
        public int Width { get; } 
        public int Height { get; } 
        public TileType[,] Cells { get; }

        public Grid(int width, int height) 
        {
            Width = width;
            Height = height;
            Cells = new TileType[Width, Height];    
        }

        
        public void ApplyGravity() // Fills the gaps left by matched tiles.
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = Height - 1; y >= 0; y--)
                {
                    if (Cells[x, y] == TileType.Empty)
                    {
                        for (int above = y - 1; above >= 0; above--)
                        {
                            if (Cells[x, above] != TileType.Empty)
                            {
                                Cells[x, y] = Cells[x, above];
                                Cells[x, above] = TileType.Empty;
                                break;
                            }
                        }
                    }
                }
            }
        }


    }
}
