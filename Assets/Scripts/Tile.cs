using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class represents a single tile.
    A tile contains the background as well as any GameObjects (Entities) on top

 */
public class Tile
{
    public enum TileTypes { FLOOR, WALL };
    public enum TileEntityTypes { NONE, ITEM, ENTRANCE, EXIT }; 
    public enum TileFogOfWarTypes { INVISABLE, VISITED, VISABLE };

    public static float DRAW_SIZE = 1f;

    public GameObject tileObj; // The GameObject that this tile is linked to
    public bool hasPlayer = false;
    public bool hasEnemy = false;
    //public bool isWall; // If tile is a wall tile
    public string name = "";
    public Item itemSpawn;

    public int xCor;
    public int yCor;
    public int tileType;

    public int tileFogOfWarType;
    public bool tileFogOfWarTypeHasChanged;

    // TODO: Add entities once implemented
    // For now, only the prefab for the one object
    // Can be:
    //      "entrance"
    //      "exit"
    //      "path_door_closed"
    public int tileEntityType = 0;
    public string tileEntityName = "";
    public GameObject tileEntityObj;
    
    public Tile(int xcor, int ycor, int tiletype)
    {
        this.xCor = xcor;
        this.yCor = ycor;
        this.tileType = tiletype;
        this.hasPlayer = false;
        this.tileFogOfWarTypeHasChanged = false;
    }

    public string getName()
    {
        return name;
    }
    // TODO: Complete or modify this
    // Function to check what entites the tile contains
    // public bool containsEntity(Entity entity)
    // {
    //     bool isContained = false;

    //     return isContained;
    // }

    // TODO: Complete this?
    // Function to add player to this tile
    // public void addPlayerToTile(Player player)
    // {
    //     tileEntities.add(player);
    //     this.hasPlayer = true;
    // }

    // TODO: Complete this
    // Function to add an entity to the tile
    // Use addPlayerToTile to add a player?
    // public void addEntityToTile(Entity entity)
    // {
    //     tileEntities.add(entity);
    // }

    public int getTileType()
    {
        return this.tileType;
    }

    public void SetTileEntity(int type, GameObject newEntity)
    {
        this.tileEntityType = type;
        this.tileEntityObj = newEntity;
    }

    public int GetTileEntityType()
    {
        return this.tileEntityType;
    }
}
