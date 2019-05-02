using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
    This class is the manager for a level.
    This class is tied to the LevelManager GameObject

    Functions:
        (CURRENT)
        - Reads map parameters from JSON file
        - Creates a map with parameters

        (FUTURE)
        - Creates map details
        - Removes the map (or clears it?)
        - Handles collisions
            - Player
            - Enemy
            - Item

 */

public class LevelManager : MonoBehaviour
{
    public static TextAsset LEVEL_DATA_JSON_PATH;//"./Assets/Library/Level_Data_SLIME_FACEHUGGER.json";

    public GameObject mapObj; // The GameObject of the map for this level
    public MapCreator mapCreator; // Tied to MapCreatorObj GameObject
    public GameObject enemyManagerObject;
    public EnemyManager enemyManagerScript;

    public bool playerOnEntranceTile = false;
    public bool playerOnExitTile = false;

    public int levelId;
    private Level level; // Used to store the level data from JSON

    private List<Tile> doorTiles; // Used to check which tiles to update

    void Awake() {
        LEVEL_DATA_JSON_PATH = Resources.Load<TextAsset>("Level_Data_SLIME_FACEHUGGER");
    }

    // This function creates and draws a map for a level
    public void createLevel(int levelId, GameObject playerObj)
    {
        this.levelId = levelId;
        IngameUI.level.text = "1-"+(this.levelId+1);

        // Read the details of the level from the JSON file
        parseLevelFromJson();

        // Create and draw the map
        mapObj = mapCreator.createMap(level);

        mapObj.transform.SetParent(this.transform);
        mapCreator.itemsObj.transform.SetParent(this.transform);

        // Setup map for the Player object
        playerObj.GetComponent<Player>().setUpPlayerMap(mapObj);

        // Setup map for each Enemy object
        enemyManagerScript = enemyManagerObject.GetComponent<EnemyManager>();
        for(int i = 0; i < enemyManagerScript.enemies.Length; i++)
        {
            if(enemyManagerScript.enemies[i] != null)
            {
                enemyManagerScript.enemies[i].GetComponent<Enemy>().SetupEnemyMap(mapObj, this.transform.gameObject.transform.GetChild(2).gameObject, this.transform.gameObject);
            }
        }

        // Set player position and update map
        mapCreator.setInitialPlayerPosition(playerObj);

        doorTiles = mapCreator.getDoorTiles();
    }

    // This function reads the level data from the JSON file 
    // TODO: Throw errors if not read correctly!
    public void parseLevelFromJson() 
    {

        //string levelJsonString = File.ReadAllText(@LEVEL_DATA_JSON_PATH);
        Level[] levels = JsonHelper.FromJson<Level>(LEVEL_DATA_JSON_PATH.text);
        level = levels[levelId];
        //Debug.Log(level.ToString());
    }

    public void movePlayerToExitTile(GameObject playerObj)
    {
        Map map = mapObj.GetComponent<Map>();

        for (int i = 0; i < map.MapXSize; i++)
        {
            for (int j = 0; j < map.MapYSize; j++)
            {
                if(map.getTile(i, j).hasPlayer)
                {
                    // map.getTile(i, j).hasPlayer = false;
                    // map.getTile(i-1, j).hasPlayer = true;
                    playerObj.transform.SetPositionAndRotation(map.getTile(i, j).tileObj.transform.position, Quaternion.identity);
                    return;
                }
            }
        }

    }

    public void movePlayerToEntranceTile(GameObject playerObj)
    {
        Map map = mapObj.GetComponent<Map>();

        for (int i = 0; i < map.MapXSize; i++)
        {
            for (int j = 0; j < map.MapYSize; j++)
            {
                if(map.getTile(i, j).hasPlayer)
                {
                    map.getTile(i, j).hasPlayer = false;
                    map.getTile(i+1, j).hasPlayer = true;
                    playerObj.transform.SetPositionAndRotation(map.getTile(i+1, j).tileObj.transform.position, Quaternion.identity);
                    return;
                }
            }
        }
    }

    private void Update() 
    {
        if(doorTiles != null)
        {
            foreach (Tile doorTile in doorTiles)
            {
                if (doorTile.hasPlayer || doorTile.hasEnemy)
                    mapCreator.openDoor(doorTile);
                else
                    mapCreator.closeDoor(doorTile);
            }
        }
    }
}

