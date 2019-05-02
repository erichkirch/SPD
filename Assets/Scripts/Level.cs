using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class represents a level.
    This is mainly used to store the map parameters and level details
    Used in reading from and saving to a JSON file
 */
[Serializable]
public class Level
{
    /// Level Meta Parameters
    public int id;  // 0 - 24. Each 5 is a different world type
    public int world;   // 0 - 4
    public string world_type;   // Forest, Ice, Air, Fire, Radiation

    /// Map Parameters
    public int MapXSize;
    public int MapYSize;
    public int MinRoomWidth;
    public int MinRoomHeight;
    public int RoomMaxSize;
    public int RoomMinSize;
    public int Partitions;
    public int MaxItemCount;
    public int MinItemCount;

    /// Level Parameters
    public float lockedDoorChance;
    public float burnObstacleChance;
    public float levitateChance;
    public float graveRobbingChance;
    public float miniBossChance;
    public float skeletonPickingChance;
    public float treasureChance;
    public float trapChance;

    /// Creature Parameters
    public int numTotalCreatures;
    public Creature[] creatures;

    // TODO: Level details as vars (e.g. Number range of enemies, number range of chests, items in chest, etc.),

    public override string ToString()
    {
        string creatureStr = "";
        for (int i = 0; i < creatures.Length; i++)
        {
            creatureStr += creatures[i].ToString();
        }


        return "Id: " + id.ToString() + "\n" +
                "world: " + world.ToString() + "\n" +
                "world_type: " + world_type.ToString() + "\n" +
                "MapXSize: " + MapXSize.ToString() + "\n" +
                "MapYSize: " + MapYSize.ToString() + "\n" +
                "MinRoomWidth: " + MinRoomWidth.ToString() + "\n" +
                "MinRoomHeight: " + MinRoomHeight.ToString() + "\n" +
                "RoomMaxSize: " + RoomMaxSize.ToString() + "\n" +
                "RoomMinSize: " + RoomMinSize.ToString() + "\n" +
                "lockedDoorChance: " + lockedDoorChance.ToString() + "\n" +
                "burnObstacleChance: " + burnObstacleChance.ToString() + "\n" +
                "levitateChance: " + levitateChance.ToString() + "\n" +
                "graveRobbingChance: " + graveRobbingChance.ToString() + "\n" +
                "miniBossChance: " + miniBossChance.ToString() + "\n" +
                "skeletonPickingChance: " + skeletonPickingChance.ToString() + "\n" +
                "treasureChance: " + treasureChance.ToString() + "\n" +
                "trapChance: " + trapChance.ToString() + "\n" +
                "numTotalCreatures: " + numTotalCreatures.ToString() + "\n" +
                "creatures: " + creatureStr + "\n";
    }
}
