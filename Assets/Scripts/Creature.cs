using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class represents a Creature.
    Right now this is mainly used to store the creature parameters
    Used in reading from and saving to a JSON file.
 */

 // TODO: Make this class abstract? Unecessary for now

[Serializable]
public class Creature
{
    public string creatureName;
    public string difficulty;
    public float spawnChance;
    public int HP;
    public int XP;
    public int minDmg;
    public int maxDmg;
    public int armor;
    public float dodgeChance;
    public float hitChance;
    public float critChance;
    public float failChance;
    public float lootChance;


    public override string ToString()
    {
        return "\n\tcreatureName: " + creatureName.ToString() + "\n" +
                "\tdifficulty: " + difficulty.ToString() + "\n" +
                "\tspawnChance: " + spawnChance.ToString() + "\n" +
                "\tHP: " + HP.ToString() + "\n" +
                "\tXP: " + XP.ToString() + "\n" +
                "\tminDmg: " + minDmg.ToString() + "\n" +
                "\tmaxDmg: " + maxDmg.ToString() + "\n" +
                "\tarmor: " + armor.ToString() + "\n" +
                "\tdodgeChance: " + dodgeChance.ToString() + "\n" +
                "\thitChance: " + hitChance.ToString() + "\n" +
                "\tcritChance: " + critChance.ToString() + "\n" +
                "\tfailChance: " + failChance.ToString() + "\n" +
                "\tlootChance: " + lootChance.ToString() + "\n";
    }
    
}
