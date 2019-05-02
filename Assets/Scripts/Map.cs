using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class represents the map.
 */
public class Map : MonoBehaviour
{
    public Tile[,] map;

    // For heirarchy organizational purposes
    // Created by MapCreator, used here for future references
    public GameObject wallTilesObj;
    public GameObject floorTilesObj;
    public GameObject doorTilesObj;

    public int MapXSize;
    public int MapYSize;

    public void initMap(int mapXSize, int mapYSize) 
    {
        this.MapXSize = mapXSize;
        this.MapYSize = mapYSize;
        map = new Tile[MapXSize, MapYSize];
    }

    public Tile getTile(int xCor, int yCor)
    {
        if(xCor < MapXSize && yCor < MapYSize)
        {
            return map[xCor, yCor];
        }
        else
        {
            return null;
        }
    }

    public bool setTile(Tile tile)
    {
        if(tile.xCor < MapXSize && tile.yCor < MapYSize && !tileIsSet(tile.xCor, tile.yCor))
        {
            map[tile.xCor, tile.yCor] = tile;
            return true;
        }
        else
        {
            return false;
        }
    }

    // TODO: Implement once entities exist
    // public bool setTileEntites()
    // {
    //     return false;
    // }

    public bool tileIsSet(int row, int col)
    {
        return map[row, col] != null;
    }

    public int getXSize()
    {
        return this.MapXSize;
    }

    public int getYSize()
    {
        return this.MapYSize;
    }
}
