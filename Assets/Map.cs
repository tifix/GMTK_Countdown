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

        public void UpdateTilemap(Tilemap tilemap, TileBase tileBase, TileBase gemTile, TileBase gemTile2, TileBase gemGemTile)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (OnOrBeyondMapEdge(x, y)) {
                        tilemap.SetTile(new Vector3Int(x, y), tileBase);

                    }
                    else if (GetTile(x, y))
                    {
                        if (UnityEngine.Random.value > 0.95)
                        {
                            if (IsTouchingAir(x, y))
                            {
                                tilemap.SetTile(new Vector3Int(x, y), gemTile);
                            }
                            else
                            {
                                tilemap.SetTile(new Vector3Int(x, y), gemTile2);
                            }
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(x, y), tileBase);
                        }
                    }
                    else
                    {
                        if (IsTouchingGround(x, y) && UnityEngine.Random.value > 0.90)
                        {
                            tilemap.SetTile(new Vector3Int(x, y), gemGemTile);
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(x, y), null);
                        }
                    }
                }
            }
        }
    }
}
