using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using UnityEngine.Tilemaps;
using Assets;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class TilemapGen : MonoBehaviour
{
    //way to organise tiles, fetch them. Like a dictionary but not necessarily limited to key-value pairs
    [System.Serializable]
    public struct DictEntry { public TileType name; public TileBase TileData; public GameObject SpawnOnDestroyed; }
    public enum TileType{ plain, gemExplosive, gemUpload, gemReveal, gemDash}
    public DictEntry[] TileDictionary = new DictEntry[5];


    public GameObject debugSprite;
    public TileBase tileBase;
    public TileBase gemTile;
    public TileBase gemTile2;
    public TileBase gemGemTile;

    private Tilemap tilemap;

    private Map map;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        BUTT_regenerate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BUTT_regenerate() 
    {
        map = new();
        map.Generate(0.11f);
        map.Build();
        map.UpdateTilemap(tilemap, tileBase, gemTile, gemTile2, gemGemTile);

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
}
