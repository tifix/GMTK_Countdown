using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets
{
    internal class Map
    {
        public int width = 200;
        public int height = 500;
        public bool[] tiles;

        public float[] weights;

        public void Generate(float scale)
        {
            int xstart = UnityEngine.Random.Range(-9999, 9999);
            int ystart = UnityEngine.Random.Range(-9999, 9999);


            weights = new float[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    weights[x + width * y] = Mathf.PerlinNoise(x * scale + xstart, y * scale + ystart);
                }
            }
        }

        public void Build()
        {
            float cutoff = 0.5f;

            tiles = new bool[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x + y * width] = weights[x + width * y] > cutoff;
                }
            }
        }

        public bool GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return true;
            }

            return tiles[x + y * width];
        }

        public bool OnOrBeyondMapEdge(int x, int y) {
            return x < 1 || y < 1 || x >= width - 1 || y >= height - 1;
        }

        public bool IsTouchingAir(int x, int y)
        {
            return !(GetTile(x + 1, y) && GetTile(x - 1, y) && GetTile(x, y + 1) && GetTile(x, y - 1));
        }

        public bool IsTouchingGround(int x, int y)
        {
            return GetTile(x + 1, y) || GetTile(x - 1, y) || GetTile(x, y + 1) || GetTile(x, y - 1);
        }

        public void UpdateTilemap(Tilemap surfaceTilemap, Tilemap gemsTilemap, TileBase tileBase, TileBase gemTile, TileBase gemTile2, TileBase gemGemTile)
        {
            surfaceTilemap.ClearAllTiles();
            gemsTilemap.ClearAllTiles();

            int wallDepth = 20;

            for (int x = -wallDepth; x < width + wallDepth; x++)
            {
                for (int y = -wallDepth; y < height + wallDepth; y++)
                {
                    if (OnOrBeyondMapEdge(x, y)) {
                        surfaceTilemap.SetTile(new Vector3Int(x, y), tileBase);
                    }
                    else if (GetTile(x, y))
                    {
                        surfaceTilemap.SetTile(new Vector3Int(x, y), tileBase);
                        if (UnityEngine.Random.value > 0.95)
                        {
                            if (IsTouchingAir(x, y))
                            {
                                gemsTilemap.SetTile(new Vector3Int(x, y), gemTile);
                            }
                            else
                            {
                                gemsTilemap.SetTile(new Vector3Int(x, y), gemTile2);
                            }
                        }
                    }
                    else
                    {
                        if (IsTouchingGround(x, y) && UnityEngine.Random.value > 0.90)
                        {
                            gemsTilemap.SetTile(new Vector3Int(x, y), gemGemTile);
                        }
                    }
                }
            }
        }
    }
}
