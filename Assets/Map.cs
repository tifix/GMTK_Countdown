using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Assets
{
    public enum Direction {South,Noth,West,East}
    internal class Map
    {
        public Map(int _width, int _height) 
        {
            width = _width; 
            height = _height; 
        }

        public int width = 200;
        public int height = 500;
        public int wallDepth = 20;
        public bool[] tiles;

        public float[] weights;

        public float GemFrequency = 0.05f;
        public float SurfaceGemFrequency = 0.1f;

        public float openingWidth = 20;

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



        public void UpdateTilemap(Tilemap surfaceTilemap, Tilemap gemsTilemap, DictEntry[] Dictionary) //, TileBase tileBase, TileBase gemTileExplosive, TileBase gemTileUpload, TileBase gemTileSurfaceReveal
        {
            TileBase tileBase = TilemapGen.EnumToData(TileType.plain, Dictionary);
            surfaceTilemap.ClearAllTiles();
            gemsTilemap.ClearAllTiles();


            for (int x = -wallDepth; x < width + wallDepth; x++)
            {
                for (int y = -wallDepth; y < height + wallDepth; y++)
                {
                    //Generate opening at the top
                    if (y > height - wallDepth && x > (width - openingWidth) / 2 && x < (width + openingWidth) / 2)
                    {
                        //tiles[x + y * width] = false;
                        continue;
                    }

                    if (OnOrBeyondMapEdge(x, y)) {
                        surfaceTilemap.SetTile(new Vector3Int(x, y), tileBase);
                    }
                    else if (GetTile(x, y))
                    {
                        surfaceTilemap.SetTile(new Vector3Int(x, y), tileBase);
                        if (UnityEngine.Random.value < GemFrequency)
                        {

                            //Randomise which of the gems to spawn
                            TileBase gemToSpawn = new TileBase[]
                            {
                                TilemapGen.EnumToData(TileType.gemExplosive, Dictionary),
                                TilemapGen.EnumToData(TileType.gemUpload, Dictionary),
                                TilemapGen.EnumToData(TileType.gemReveal, Dictionary)
                            }
                            [UnityEngine.Random.Range(0, 3)];

                            gemsTilemap.SetTile(new Vector3Int(x, y), gemToSpawn);                           
                        }
                    }
                    else
                    {
                        //if this is an edge, Spawn surface crystal
                        if (IsTouchingGround(x, y) && UnityEngine.Random.value < SurfaceGemFrequency)
                        {
                            //determine how to rotate the crystal so it connects to the ground
                            var tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                            switch (GetDirectionGroundTouched(x, y))
                            {
                                //if terrain under, no transform changes needed
                                case Direction.South:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));
                                        break;
                                    }
                                //if terrain above, flip vertically
                                case Direction.Noth:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                                        break;
                                    }
                                //if terrain to the left, rotate
                                case Direction.West:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                                        break;
                                    }
                                //if terrain to the left, rotate
                                case Direction.East:
                                    {
                                        tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                                        break;
                                    }
                            }
                            //Randomise which of the gems to spawn
                            TileBase gemToSpawn = new TileBase[] 
                            { 
                                TilemapGen.EnumToData(TileType.gemExplosiveSurface, Dictionary), 
                                TilemapGen.EnumToData(TileType.gemDashSurface, Dictionary) 
                            }
                            [UnityEngine.Random.Range(0,2)];

                            //create the transform struct using the rotations determined
                            var tileChangeData = new TileChangeData
                            {
                                position = new Vector3Int(x, y),
                                tile = gemToSpawn,
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
