using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is the main manager for the entire game.
    This class is tied to the GameManager GameObject

    Functions:
        (CURRENT)
        - Instantiates the level
        - Calls the function to create and draw the map

        (FUTURE)
        - Handles main menu
        - Handles general game settings
        - Handles player instantiation
        - Handles player 
        - Main game loop (?)

 */
// TODO: Create a Game class that does the actual "playing" of the game,
//         This class would create the Game and level objects,
//         Seems unecessary for now, could be useful for having multiple games in the future
public class GameManager : MonoBehaviour
{
    public GameObject levelManagerPrefab;
    public GameObject levelManagerObj;
    private List<GameObject> levelManagerObjs;

    private LevelManager levelManager;
    private GraphManager graphManager;
    private FogOfWar fogOfWar;

    public GameObject loadingCanvas;
    public Camera loadingCamera;

    // TODO: Integrate player object properly
    public GameObject playerObj;

    public int currentLevel = -1;
    public bool loading = false;

    public int maxNumLevels;

    private void Awake()
    {
        levelManagerObjs = new List<GameObject>();
    }

    // Main game loop for now
    public void Update()
    {
        // First level
        if(currentLevel == -1)
        {
            StartCoroutine(loadingScreen());
            startNextLevel();
        }

        // Check to see if the player is on the current levels exit
        if (!loading && levelManager.playerOnExitTile && currentLevel < maxNumLevels)
        {
            levelManager.playerOnExitTile = false;
            StartCoroutine(loadingScreen());
            startNextLevel();
            
        }
        else if (!loading && levelManager.playerOnExitTile && currentLevel == maxNumLevels)
        {
            Debug.Log("THE END");
            levelManager.playerOnExitTile = false;
        }

        // Check to see if the player is on the current levels entrance
        if (!loading && levelManager.playerOnEntranceTile)
        {
            levelManager.playerOnEntranceTile = false;
            if (currentLevel != 0)
            {
                StartCoroutine(loadingScreen());
                loadPrevLevel();
                //Debug.Log("THERE'S NO GOING BACK");
            }
            else
            {
                Debug.Log("ONE DOES NOT SIMPLY LEAVE");
            }

        }

        // TODO: Move the player if needed
        // TODO: Do all level checks
        // TODO: If level is complete clear the current screen of all non-essential GameObjects (e.g. previous level), then draw next level
    }

    IEnumerator loadingScreen()
    {
        loading = true;
        loadingCamera.gameObject.SetActive(true);
        loadingCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        loadingCanvas.SetActive(false);
        loadingCamera.gameObject.SetActive(false);
        loading = false;
    }

    public void startNextLevel()
    {
        //Get rid of all skeletons
        clearSkeletons();

        currentLevel++;

        // Disable the last level's game object
        if (currentLevel != 0)
        {
            levelManagerObjs[currentLevel - 1].SetActive(false);
        }

        if(checkLevelManagerExistance() == -1)
        {
            levelManagerObj = Instantiate(levelManagerPrefab);
            levelManagerObj.transform.name = "levelManagerObj_" + currentLevel;
            levelManagerObjs.Add(levelManagerObj);

            playerObj.GetComponent<Player>().levelManagerObject = levelManagerObj;
            playerObj.GetComponent<Player>().graphManagerObject = levelManagerObj.transform.GetChild(2).gameObject;

            levelManager = levelManagerObj.GetComponent<LevelManager>();
            levelManager.createLevel(currentLevel, playerObj);
            levelManager.enemyManagerObject = levelManagerObj.transform.GetChild(3).gameObject;

            graphManager = levelManagerObj.transform.GetChild(2).gameObject.GetComponent<GraphManager>();
            graphManager.setUp(levelManager);

            fogOfWar = levelManagerObj.transform.GetChild(1).gameObject.GetComponent<FogOfWar>();
            fogOfWar.createFogOfWar(levelManager.mapObj, playerObj);
        }
        else
        {
            levelManagerObj = levelManagerObjs[currentLevel];
            levelManagerObj.SetActive(true);

            playerObj.GetComponent<Player>().levelManagerObject = levelManagerObj;
            playerObj.GetComponent<Player>().graphManagerObject = levelManagerObj.transform.GetChild(2).gameObject;

            levelManager = levelManagerObj.GetComponent<LevelManager>();
            levelManager.movePlayerToEntranceTile(playerObj);

            graphManager = levelManagerObj.transform.GetChild(2).gameObject.GetComponent<GraphManager>();
            fogOfWar = levelManagerObj.transform.GetChild(1).gameObject.GetComponent<FogOfWar>();

            playerObj.GetComponent<Player>().levelManagerScript = levelManagerObj.GetComponent<LevelManager>();
            playerObj.GetComponent<Player>().theMap = levelManager.mapObj.GetComponent<Map>();
            playerObj.GetComponent<Player>().graphManagerScript = playerObj.GetComponent<Player>().graphManagerObject.GetComponent<GraphManager>();
        }

    }

    public void loadPrevLevel()
    {
        clearSkeletons();

        currentLevel--;
        
        levelManagerObj.SetActive(false);

        levelManagerObj = levelManagerObjs[currentLevel];
        levelManagerObj.SetActive(true);

        playerObj.GetComponent<Player>().levelManagerObject = levelManagerObj;
        playerObj.GetComponent<Player>().graphManagerObject = levelManagerObj.transform.GetChild(2).gameObject;

        levelManager = levelManagerObj.GetComponent<LevelManager>();
        levelManager.movePlayerToExitTile(playerObj);

        graphManager = levelManagerObj.transform.GetChild(2).gameObject.GetComponent<GraphManager>();
        fogOfWar = levelManagerObj.transform.GetChild(1).gameObject.GetComponent<FogOfWar>();

        playerObj.GetComponent<Player>().levelManagerScript = levelManagerObj.GetComponent<LevelManager>();
        playerObj.GetComponent<Player>().theMap = levelManager.mapObj.GetComponent<Map>();
        playerObj.GetComponent<Player>().graphManagerScript = playerObj.GetComponent<Player>().graphManagerObject.GetComponent<GraphManager>();

    }

    public int checkLevelManagerExistance()
    {
        for (int i = 0; i < levelManagerObjs.Count; i++)
        {
            if(levelManagerObjs[i].GetComponent<LevelManager>().levelId == currentLevel)
                return i;
        }

        return -1;
    }

    private void clearSkeletons()
    {
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        if (skeletons.Length > 0)
        {
            for (int i = 0; i < skeletons.Length; i++)
            {
                Destroy(skeletons[i]);
            }
        }
    }

}
