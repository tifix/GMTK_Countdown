using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Assets
{
    public enum Direction {South,Noth,West,East}
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
        //Returns the direction towards which terrain is present if touching. If there's terrain above this tile, returns north.
        public Direction GetDirectionGroundTouched(int x, int y)
        {
            if (GetTile(x + 1, y))
            {
                return Direction.East;
            }
            if (GetTile(x - 1, y))
            {
                return Direction.West;
            }
            if (GetTile(x, y + 1))
            {
                return Direction.Noth;
            }
            return Direction.South;
        }

        public void UpdateTilemap(Tilemap surfaceTilemap, Tilemap gemsTilemap, TileBase tileBase, TileBase gemTileExplosive, TileBase gemTileUpload, TileBase gemTileSurfaceReveal)
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
                            //spawn explosive gems at the surface
                            if (IsTouchingAir(x, y))
                            {
                                gemsTilemap.SetTile(new Vector3Int(x, y), gemTileExplosive);
                            }
                            else
                            {
                                gemsTilemap.SetTile(new Vector3Int(x, y), gemTileUpload);
                            }
                        }
                    }
                    else
                    {
                        //if this is an edge, Spawn surface crystal
                        if (IsTouchingGround(x, y) && UnityEngine.Random.value > 0.90)
                        {
                            //determine how to rotate the crystal so it connects to the ground
                            var tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                            switch (GetDirectionGroundTouched(x, y))
                            {
                                //if terrain under, no transform changes needed
                                case Direction.South:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));
                                        Debug.Log("south");
                                        break;
                                    }
                                //if terrain above, flip vertically
                                case Direction.Noth:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                                        Debug.Log("north");
                                        break;
                                    }
                                //if terrain to the left, rotate
                                case Direction.West:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                                        Debug.Log("west");
                                        break;
                                    }
                                //if terrain to the left, rotate
                                case Direction.East:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                                        Debug.Log("east");
                                        break;
                                    }
                            }

                            //create the transform struct using the rotations determined
                            var tileChangeData = new TileChangeData
                            {
                                position = new Vector3Int(x, y),
                                tile = gemTileSurfaceReveal,
                                color = Color.white,
                                transform = tileTransform
                            };
                            //finally place the tile
                            gemsTilemap.SetTile(tileChangeData, true);
                        }
                    }      
                }
            }
        }
    }       
}
