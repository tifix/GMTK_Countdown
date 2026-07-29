using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using UnityEngine.Tilemaps;
using Assets;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;

public enum TileType { plain, gemExplosive, gemUpload, gemReveal, gemDash, gemExplosiveSurface, gemDashSurface, gemRevealSurface }
//way to organise tiles, fetch them. Like a dictionary but not necessarily limited to key-value pairs
[System.Serializable]
public struct DictEntry 
{ 
    public TileType name; 
    public TileBase TileData; 
    public GameObject SpawnOnDestroyed; 
}


public class TilemapGen : MonoBehaviour
{

    public DictEntry[] TileDictionary = new DictEntry[5];


    public GameObject debugSprite;
    public TileBase tileBase;
    public TileBase gemTileExplosive;
    public TileBase gemTileUpload;
    public TileBase gemTileSurfaceReveal;

    public Tilemap surfaceTilemap;
    public Tilemap gemsTilemap;
    public Tilemap backgroundTilemap;

    private Map map;

    public int width = 200;
    public int height = 500;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //move player
        FindFirstObjectByType<PlayerController>().transform.position = new Vector3(width/2, height + 10);
        BUTT_regenerate();
        //backgroundTilemap.BoxFill(Vector3Int.zero, background, 0, 0, map.width, map.height);
    }

    public TileBase EnumToData(TileType Type) 
    {
        foreach (var Entry in TileDictionary) 
        {
            if(Entry.name == Type) 
            {
                return Entry.TileData;
            }
        }
        return null;
    }
    public static TileBase EnumToData(TileType Type, DictEntry[] Dictionary)
    {
        foreach (var Entry in Dictionary)
        {
            if (Entry.name == Type)
            {
                return Entry.TileData;
            }
        }
        return null;
    }

    public void BUTT_regenerate()
    {
        map = new(width, height);
        map.Generate(0.11f);
        map.Build();
        map.UpdateTilemap(surfaceTilemap, gemsTilemap, TileDictionary); //,tileBase, gemTileExplosive, gemTileUpload, gemTileSurfaceReveal

        //EnumToData(TileType.gemExplosive);

        //Texture2D texture = new(map.width, map.height);
        //for (int x = 0; x < map.width; x++)
        //{
        //    for (int y = 0; y < map.height; y++)
        //    {
        //        texture.SetPixel(x, y, new Color(map.weights[x + map.width * y], 0f, 0f));
        //    }
        //}

        //texture.Apply();
        //debugSprite.GetComponent<RawImage>().texture = texture;

        //Gizmos.DrawGUITexture(new Rect(10, 10, 20, 20), texture);
        //Gizmos.DrawLine(new Vector3(0, 0), new Vector3(100, 100));
    }

    //Destroys tiles around a location in a radius specified
    public void BreakTiles(TilemapCollider2D TilemapColliderAffected, Vector3 explosionLocation, float explosionRadius)
    {
        //which layer of tilemap did we hit?
        Tilemap TileMap = TilemapColliderAffected.GetComponent<Tilemap>();
        GridLayout gridLayout = GetComponent<GridLayout>();

        //create a bounds struct to check within - TODO: update to circle not square checker
        Vector3 BoundsMiddle = explosionLocation - Vector3Int.one * Mathf.FloorToInt(explosionRadius / 2);
        var cellBounds = new BoundsInt(
        gridLayout.WorldToCell(BoundsMiddle), Vector3Int.one * Mathf.FloorToInt(explosionRadius));

        //check all tiles within the bounds
        foreach (var cell in cellBounds.allPositionsWithin)
        {
            BreakTileAtPosition(cell, TileMap);         
        }
    }
    public void BreakTileAtPosition(Vector3Int position, Tilemap TileMap)
    {
        GridLayout gridLayout = GetComponent<GridLayout>();

        if (TileMap.HasTile(position))
        {
            //get tile data, check if it's special, if it is, spawn a gem from prefab
            TileBase tileDestroyed = TileMap.GetTile(position);
            Debug.Log("cell of type " + tileDestroyed.name + " exploded!");
            foreach (var TilePrefab in TileDictionary)
            {
                if (tileDestroyed == TilePrefab.TileData && TilePrefab.SpawnOnDestroyed != null)
                {
                    Instantiate(TilePrefab.SpawnOnDestroyed, gridLayout.CellToWorld(position), Quaternion.identity);
                }
            }

            TileMap.SetTile(position, null);
        }
    }
}
