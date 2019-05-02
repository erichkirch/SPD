using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject skeletonPrefab;
    public GameObject slimePrefab;
    public GameObject faceHuggerPrefab;
    private GameObject whichPrefab = null;

    GameObject creatureClone;
    public GameObject[] enemies;

    public void createEnemies(Level level, Map map)
    {
     
        enemies = new GameObject[level.numTotalCreatures];

        int randX=0;
        int randY=0;

        // Create creature objects
        for (int i = 0; i < level.numTotalCreatures; i++)
        {
            bool goodSpawnLocation = false;

            // Spawn at random floor tile location
            while (!goodSpawnLocation)
            {
                randX = Random.Range(0, level.MapXSize);
                randY = Random.Range(0, level.MapYSize);
                Tile t = map.getTile(randX, randY);

                if (t.getTileType() == (int)Tile.TileTypes.FLOOR && !t.getName().Contains("entrance") && !t.tileEntityName.Contains("door"))
                {
                    goodSpawnLocation = true;
                }
            }

            //Spawn locations decided, now decide on a random creature to spawn there
            int whichCreature = Random.Range(0, level.creatures.Length);

            string creatureName = level.creatures[whichCreature].creatureName;

            if (creatureName == "Slime")
            {
                whichPrefab = slimePrefab;
            }
            else if(creatureName == "Face Hugger")
            {
                whichPrefab = faceHuggerPrefab;
            }

            // TEMPORARY UNITL ADDING MORE ENEMIES
            if(whichPrefab == null)
                continue;

            creatureClone = Instantiate(whichPrefab);
            creatureClone.transform.SetParent(this.transform);
            creatureClone.name = creatureName;

            creatureClone.transform.position = new Vector2(randX, randY);
            Enemy enemyScript = creatureClone.GetComponent<Enemy>();
            enemyScript.enemyID = i;
            enemyScript.currX = randX;
            enemyScript.currY = randY;

            //Debug.Log("Creating creature " + creatureName + " ID : " + enemyScript.enemyID + " at " + randX + "," + randY);

            //Keep an record of all the enemies in a level
            enemies[i] = creatureClone;
        }
    }
}
