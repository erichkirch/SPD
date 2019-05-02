using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
    This class creates a map.
    This class is tied to the MapCreatorObj GameObject
 */

public class MapCreator : MonoBehaviour
{
    // Prefabs to be used
    // TODO: Convert some to lists to add variations
    [Header("Prefabs")]
    public GameObject[] floors;
    public GameObject[] walls;
    public GameObject entrance;
    public GameObject exit;
    public GameObject path_door_closed;
    public GameObject path_door_open;

    // The map that will be created
    public GameObject mapObj;

    public GameObject enemyManagerObj;
    public EnemyManager enemyManager;

    public GameObject[] itemSet;
    public GameObject itemsObj;

    public Map map; // Used for reference to the mapObj's map class

    // The level to create the map for, contains the details
    private Level level;

    // Vars used between functions
    private Room entranceRoom;
    private Room exitRoom;
    private Tile playerTile;

    public GameObject createMap(Level level)
    {
        // To use the parameters 
        this.level = level;

        // Create the map GameObject
        mapObj = new GameObject("MapObj_"+this.level.id);
        mapObj.AddComponent<Map>();
        map = mapObj.GetComponent<Map>();

        map.initMap(level.MapXSize, level.MapYSize);

        createRoomsAndPaths();

        createWalls();

        createEnterAndExit();

        createPathDoors();

        drawMap();

        createItems();

        createEnemies();

        return this.mapObj;
    }

    private void createRoomsAndPaths()
    {
        List<Rect> all_partitions = new List<Rect>();
        Rect total_world = new Rect(0, 0, level.MapXSize, level.MapYSize);

        all_partitions.Add(total_world);

        for (int i = 0; i < level.Partitions; i++)
        {
            // Get a random partition and partition it onwards
            int random_index = Random.Range(0, all_partitions.Count);
            var random_part = all_partitions[random_index];

            // Check to see the partition is not too small
            if (random_part.width / 2 < level.MinRoomWidth || random_part.height / 2 < level.MinRoomHeight)
                continue;

            // Remove it as it is going to be broken into parts
            all_partitions.RemoveAt(random_index);

            // Break into parts and add parts
            if (random_part.width < random_part.height)
            {
                //Debug.Log("Horizontal Partition.");
                int start = (int)random_part.y;
                int end = (int)random_part.y + (int)random_part.height;

                int mid_point = (int)((end - start) * Random.Range(0.33f, 0.66f));
                int left_over = (int)random_part.height - mid_point;

                Rect a = new Rect(random_part.x, random_part.y, random_part.width, mid_point);
                Rect b = new Rect(random_part.x, random_part.y + mid_point, random_part.width, left_over);

                all_partitions.Add(a);
                all_partitions.Add(b);
            }
            else
            {
                //Debug.Log("Vertical Partition.");
                int start = (int)random_part.x;
                int end = (int)random_part.x + (int)random_part.width;

                int mid_point = (int)(random_part.width * Random.Range(0.33f, 0.66f));
                int left_over = (int)random_part.width - mid_point;

                Rect a = new Rect(random_part.x, random_part.y, mid_point, random_part.height);
                Rect b = new Rect(random_part.x + mid_point, random_part.y, left_over, random_part.height);

                all_partitions.Add(a);
                all_partitions.Add(b);
            }
        }

        var rooms = new List<Room>();

        // For every partition, create a room
        foreach (var part in all_partitions)
        {
            //Debug.Log(part);
            int x_padding = (int)(part.width * Random.Range(0f, 0.5f)); // up to 50% of part width
            int y_padding = (int)(part.height * Random.Range(0f, 0.5f));

            int room_width = (int)(part.width * Random.Range(0f, 0.9f));
            int room_height = (int)(part.height * Random.Range(0f, 0.9f));

            room_width = Mathf.Min(level.RoomMaxSize, room_width);
            room_height = Mathf.Min(level.RoomMaxSize, room_height);

            room_width = Mathf.Max(level.RoomMinSize, room_width);
            room_height = Mathf.Max(level.RoomMinSize, room_height);

            // Bound checking so that it does not draw outside
            if ((int)(part.x + x_padding) + room_width >= level.MapXSize)
                room_width -= ((int)(part.x + x_padding) + room_width) - level.MapXSize;

            if ((int)(part.y + y_padding) + room_height >= level.MapYSize)
                room_height -= ((int)(part.y + y_padding) + room_height) - level.MapYSize;

            // Try to make a room
            Room r = new Room((int)(part.x + x_padding), (int)(part.y + y_padding), room_width, room_height);
            //Debug.Log(r.rect.x + "-" + r.rect.y + ":" + r.rect.width + "-" + r.rect.height);
            rooms.Add(r);
        }

        

        // Create pathways between rooms. Start with 2 random rooms and keep connecting.
        int prev_room_index = Random.Range(0, rooms.Count - 1);
        Room previous_room = rooms[prev_room_index];
        entranceRoom = previous_room;   // This first random room is our entrance room

        // Adds all the rooms to the map
        foreach (Room r in rooms)
        {
            if(r != entranceRoom)
                addRoomToMap(r);
            else if (r == entranceRoom)
                addRoomToMap(r, "entrance");
            else
                Debug.LogWarning("ROOM NOT ADDED");
        }

        rooms.RemoveAt(prev_room_index);

        Room current_room = null;
        while (rooms.Count > 0)
        {
            int current_room_index = Random.Range(0, rooms.Count - 1);
            current_room = rooms[current_room_index];
            rooms.RemoveAt(current_room_index);

            connectRooms(previous_room, current_room);

            previous_room = current_room;
        }

        // This last random room is our exit room
        exitRoom = current_room;
    }

    public void addRoomToMap(Room r, string text = null)
    {
        var part = r.rect;

        for (int row = (int)part.y; row < (int)(part.y + part.height); row++)
        {
            for (int col = (int)part.x; col < (int)(part.x + part.width); col++)
            {
                if (!(row == part.y || row == part.y + part.height - 1) &&
                    !(col == part.x || col == part.x + part.width - 1))
                {
                    Tile t = new Tile(col, row, (int)Tile.TileTypes.FLOOR);
                    t.name = "room_floor_" + col + "_" + row;
                    if(text != null)
                    {
                        t.name += "_" + text;
                    }
                    map.setTile(t);
                }
            }
        }
    }

    public void connectRooms(Room r1, Room r2)
    {
        // r1 is always to the left of r2.
        if (r1.room_center.x > r2.room_center.x)
        {
            Room temp = r1;
            r1 = r2;
            r2 = temp;
        }

        //Debug.Log("Room 1: " + r1.rect + ", Room Center: " + r1.room_center);
        //Debug.Log("Room 2: " + r2.rect + ", Room Center: " + r2.room_center);

        Vector2 r1_c = r1.room_center;
        Vector2 r2_c = r2.room_center;

        int starting_x = (int)r1.room_center.x;
        int end_x = (int)r2.room_center.x;

        // Draw the x connection (always starting with room 1, as it's to the left
        for (int j = starting_x; j <= end_x; j++)
        {
            Tile t = new Tile(j, (int)r1.room_center.y, (int)Tile.TileTypes.FLOOR);
            t.name = "path_h_floor_" + j + "_" + (int)r1.room_center.y;
            map.setTile(t);
        }

        int starting_y = (int)r1.room_center.y;
        int end_y = (int)r2.room_center.y;

        if (starting_y > end_y)
        {
            starting_y = (int)r2.room_center.y;
            end_y = (int)r1.room_center.y;
        }

        for (int j = starting_y; j < end_y; j++)
        {
            Tile t = new Tile((int)r2.room_center.x, j, (int)Tile.TileTypes.FLOOR);
            t.name = "path_v_floor_" + (int)r2.room_center.x + "_" + j;
            map.setTile(t);
        }

    }

    private void createWalls()
    {
        // Place a wall for every space that is not already occupied in map
        for (int row = 0; row <= level.MapYSize; row++)
        {
            for (int col = 0; col <= level.MapXSize; col++)
            {
                Tile t = new Tile(col, row, (int)Tile.TileTypes.WALL);
                t.name = "wall_" + col + "_" + row;
                map.setTile(t);
            }
        }
    }

    // TODO: Make them random spots instead of the middle
    // Draw the entrance and exit at a random spot in the selected rooms
    private void createEnterAndExit()
    {
        // Entrance
        int xCor = (int)entranceRoom.room_center.x;
        int yCor = (int)entranceRoom.room_center.y;

        // If the tile immediately to the right is a wall or path, then move the entrance one to the left
        Tile t_e = map.getTile(xCor+1, yCor);
        if(t_e.tileType == (int)Tile.TileTypes.WALL || t_e.name.Contains("path_"))
            xCor--;

        Tile t_entrance = map.getTile(xCor, yCor);
        t_entrance.tileEntityName = "entrance";
        t_entrance.tileEntityType = (int)Tile.TileEntityTypes.ENTRANCE;

        // Set the tile for the player
        playerTile = map.getTile(xCor+1, yCor);
        playerTile.hasPlayer = true;
        playerTile.tileEntityName = "player";

        // Exit
        Tile t_exit = map.getTile((int)exitRoom.room_center.x, (int)exitRoom.room_center.y);
        t_exit.tileEntityName = "exit";
        t_exit.tileEntityType = (int)Tile.TileEntityTypes.EXIT;

    }

    private void createPathDoors() 
    {
        for (int row = 0; row < level.MapYSize; row++)
        {
            for (int col = 0; col < level.MapXSize; col++)
            {
                // Check if the tile is a path tile
                Tile t = map.getTile(col, row);
                if(!t.name.Contains("path_"))
                    continue;

                // Get the 4 neighbouring tiles
                Tile t_n = map.getTile(col, row+1);
                Tile t_s = map.getTile(col, row-1);
                Tile t_e = map.getTile(col+1, row);
                Tile t_w = map.getTile(col-1, row);

                // Check for matching patterns for the valid spots
                if((t_n.name.Contains("room_floor") && t_w.name.Contains("wall") && t_e.name.Contains("wall")) ||
                    (t_s.name.Contains("room_floor") && t_w.name.Contains("wall") && t_e.name.Contains("wall")) ||
                    (t_n.name.Contains("wall") && t_w.name.Contains("room_floor") && t_s.name.Contains("wall")) ||
                    (t_n.name.Contains("wall") && t_e.name.Contains("room_floor") && t_s.name.Contains("wall")))
                {
                    t.tileEntityName = "path_door_closed_" + col + "_" + row;
                }
            }
        }
    }

    // TODO: Complete this
    // This function is the main method to use for drawing the base tiles and entities
    private void drawMap()
    {
        // Add the GameObjects used for heirarchy organizational purposes
        map.wallTilesObj = new GameObject("Wall Tiles");
        map.wallTilesObj.transform.SetParent(mapObj.transform);

        map.floorTilesObj = new GameObject("Floor Tiles");
        map.floorTilesObj.transform.SetParent(mapObj.transform);

        map.doorTilesObj = new GameObject("Doors");
        map.doorTilesObj.transform.SetParent(mapObj.transform);

        // Draw the map. Iterate over each tile
        for (int row = 0; row < level.MapYSize; row++)
        {
            for (int col = 0; col < level.MapXSize; col++)
            {
                Tile tile = map.getTile(col, row);

                if (tile == null)
                    continue;

                drawMapTile(row, col, tile);

                drawMapEntites(row, col, tile);

            }
        }
    }

    // Draws the base tile
    private void drawMapTile(int row, int col, Tile tile)
    {
        GameObject tileObj;
        GameObject tileObjParent;

        if (tile.tileType == (int)Tile.TileTypes.FLOOR)
        {
            int randFloor = Random.Range(0, floors.Length);
            tileObj = Instantiate(floors[randFloor]);
            tileObjParent = map.floorTilesObj;
        }
        else if (tile.tileType == (int)Tile.TileTypes.WALL)
        {
            int randWall = Random.Range(0, walls.Length);
            tileObj = Instantiate(walls[randWall]);
            tileObjParent = map.wallTilesObj;
        }
        else
        {
            Debug.LogError("Could not draw tile!! : " + row + " - " + col);
            return;
        }

        tileObj.transform.position = new Vector3(col * Tile.DRAW_SIZE, row * Tile.DRAW_SIZE, 0);
        tileObj.transform.SetParent(tileObjParent.transform);
        tileObj.name = tile.name;

        tile.tileObj = tileObj;
    }

    // TODO: Move the setparent out?
    // Draws the entites on the tile
    private void drawMapEntites(int row, int col, Tile tile)
    {
        GameObject entityObj;

        if(tile.tileEntityName == "entrance")
        {
            entityObj = Instantiate(entrance);
            entityObj.transform.SetParent(mapObj.transform);
        }
        else if (tile.tileEntityName == "exit")
        {
            entityObj = Instantiate(exit);
            entityObj.transform.SetParent(mapObj.transform);
        }
        else if (tile.tileEntityName.Contains("path_door_closed_"))
        {
            entityObj = Instantiate(path_door_closed);
            entityObj.transform.SetParent(map.doorTilesObj.transform);
        }
        else if (tile.tileEntityName.Contains("path_door_open_"))
        {
            entityObj = Instantiate(path_door_open);
            entityObj.transform.SetParent(map.doorTilesObj.transform);
        }
        else
        {
            return;
        }

        entityObj.transform.position = new Vector3(col * Tile.DRAW_SIZE, row * Tile.DRAW_SIZE, 0);
        entityObj.name = tile.tileEntityName;

        tile.tileEntityObj = entityObj;

    }

    // Center given camera on map
    public void centerMapCamera(Camera mapCamera)
    {
        float newX = (level.MapXSize / 2) * Tile.DRAW_SIZE;
        float newY = (level.MapYSize / 2) * Tile.DRAW_SIZE;
        mapCamera.transform.SetPositionAndRotation(new Vector3(newX, newY, -10f), Quaternion.identity);
    }

    // This sets the initial player position, based on the entrance location
    public void setInitialPlayerPosition(GameObject playerObj)
    {
        playerObj.transform.SetPositionAndRotation(playerTile.tileObj.transform.position, Quaternion.identity);
        playerObj.GetComponent<Player>().updateCurrentTile(mapObj.GetComponent<Map>(), true, true, playerTile.xCor, playerTile.yCor);
    }

    private void createItems()
    {
        // int testItemLocation = 21;

        // testItem = GameObject.Find("Test item");
        // testItem.transform.position = (new Vector2(testItemLocation, testItemLocation));

        // map.getTile(testItemLocation, testItemLocation).SetTileEntity((int)Tile.TileEntityTypes.ITEM, testItem);

        itemsObj = new GameObject("Items");

        GameObject myItem;
        int itemCount = 0;
        while(true) {
            for (int row = 0; row < level.MapYSize; row++) {
                for (int col = 0; col < level.MapXSize; col++) {
                    Tile t = map.getTile(col, row);
                    if(t.name.Contains("room_floor") && 
                        !t.getName().Contains("entrance") && 
                        !t.tileEntityName.Contains("exit") && 
                        t.tileEntityType != (int)Tile.TileEntityTypes.ITEM) 
                    {
                        if(Random.Range(0, 500) == 5) {
                            itemCount++;
                            int randItem = Random.Range(0, itemSet.Length);
                            myItem = Instantiate(itemSet[randItem]);
                            myItem.transform.SetParent(itemsObj.transform);
                            myItem.name = itemSet[randItem].name;
                            myItem.transform.position = (new Vector2(t.xCor, t.yCor));
                            t.itemSpawn = myItem.GetComponent<ItemPickup>().item;
                            t.SetTileEntity((int)Tile.TileEntityTypes.ITEM, myItem);
                        }
                    }
                    if(itemCount >= level.MaxItemCount)
                        break;
                }
            }
            if(itemCount >= level.MinItemCount)
                break;
        }
    }

    private void createEnemies()
    {
        enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
        enemyManager.createEnemies(level, map);
    }

    public List<Tile> getDoorTiles()
    {
        List<Tile> doorTiles = new List<Tile>();

        for (int row = 0; row < level.MapYSize; row++)
        {
            for (int col = 0; col < level.MapXSize; col++)
            {
                if (map.getTile(col, row).tileEntityName.Contains("path_door"))
                    doorTiles.Add(map.getTile(col, row));
            }
        }

        return doorTiles;
    }

    public void openDoor(Tile doorTile)
    {
        if(!doorTile.tileEntityName.Contains("path_door_open_"))
        {
             Destroy(doorTile.tileEntityObj);
            doorTile.tileEntityName = "path_door_open_" + doorTile.xCor + "_" + doorTile.yCor;
            drawMapEntites(doorTile.yCor, doorTile.xCor, doorTile);
        }
    }

    public void closeDoor(Tile doorTile)
    {
        if(!doorTile.tileEntityName.Contains("path_door_closed_"))
        {
             Destroy(doorTile.tileEntityObj);
            doorTile.tileEntityName = "path_door_closed_" + doorTile.xCor + "_" + doorTile.yCor;
            drawMapEntites(doorTile.yCor, doorTile.xCor, doorTile);
        }
    }

    // TODO: Complete this
    // public void saveMapToFile(string path)
    // {
    //     // // Serialize placedTileName and placedTilePositions to JSON format
    //     // string placedTilesString = "";
    //     // for (int i = 0; i < placedTileNames.Count; i++)
    //     // {
    //     //     placedTilesString += placedTileNames[i] + " :: " + "x:" + placedTilePositions[i].x + " y:" + placedTilePositions[i].y + " z:" + placedTilePositions[i].z + "\n";
    //     // }

    //     // File.WriteAllText(path, placedTilesString);

    // }

    // TODO: Complete this
    // public void loadMapFromFile(string path)
    // {
    //     // De-Serialize placedTileName and placedTilePositions from JSON format and instantiate
    // }
}