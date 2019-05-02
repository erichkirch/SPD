using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public GameObject playerObj;
    public Player player;

    void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<Player>();
    }

    public bool CheckForBlocked(Tile tile, Map theMap)
    {
        bool blocked = false;

        if(tile.hasEnemy || tile.hasPlayer)
        {
            blocked = true;
        }

        return blocked;
    }

    public void handleCollision(Tile tile, Map theMap)
    {
        // If tile contains an item
        if(tile.GetTileEntityType() == (int)Tile.TileEntityTypes.ITEM)
        {
            //Debug.Log("Walked Over Item: " + tile.tileEntityObj.name);
            if (tile == player.lastClickedTile)
            {    // if we clicked an item tile and pass over it
               // Debug.Log("Picking up " + tile.tileEntityObj.name);
                IngameUI.logPrint("Picked up " + tile.tileEntityObj.name);
                bool wasPickedUp = Inventory.instance.Add(tile.itemSpawn);    // Add to inventory

                // If successfully picked up
                if (wasPickedUp)
                {
                    tile.itemSpawn = null;
                    Destroy(tile.tileEntityObj);    // Destroy item from scene
                    tile.SetTileEntity((int)Tile.TileEntityTypes.NONE, null);
                }
            }
        }
        else if (tile.GetTileEntityType() == (int)Tile.TileEntityTypes.ENTRANCE && tile == player.lastClickedTile)
        {
            // Go to previous level
            StartCoroutine(waitForPlayerOnEntranceTile());
        }
        else if (tile.GetTileEntityType() == (int)Tile.TileEntityTypes.EXIT && tile == player.lastClickedTile)
        {
            // Go to next level
            StartCoroutine(waitForPlayerOnExitTile());
        }
    }

    // Wait for the player to finish walking to the exit tile
    IEnumerator waitForPlayerOnExitTile()
    {
        yield return new WaitForSeconds(0.5f);
        player.levelManagerScript.playerOnExitTile = true;
    }

    // Wait for the player to finish walking to the entrance tile
    IEnumerator waitForPlayerOnEntranceTile()
    {
        yield return new WaitForSeconds(0.5f);
        player.levelManagerScript.playerOnEntranceTile = true;
    }
}
