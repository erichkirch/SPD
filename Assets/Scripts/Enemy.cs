using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MovingObject
{
    //Currently this don't get assigned - will be assigned from Creature.cs
    public string creatureName;
    public string difficulty;
    public float spawnChance;
    public int enemyHP = 50;
    public int XP;
    public int minDmg;
    public int maxDmg;
    public int armor;
    public float dodgeChance;
    public float hitChance;
    public float critChance;
    public float failChance;
    public float lootChance;
    private int waitMilliseconds = 800;

    public bool isPlayer;
    private bool seenPlayer;
    public GameObject levelManagerObject;
    public int enemyAttackDmg = 10; //TEMPORARTY, JUST FOR TESTING
    private LevelManager levelManagerScript;
    private Queue<Node> queue;
    private List<Node> bestRoute;
    private Map theMap;
    private GameObject soundManagerObject;
    private SoundManager soundManagerScript;
    private GameObject battleManagerObject;
    private BattleManager battleManagerScript;
    GameObject thePlayer;
    GameObject graphManagerObj;

    private GraphManager graphManagerScript;

    protected override void Start()
    {
        base.Start();
    }

    public void SetupEnemyMap(GameObject mapObj, GameObject graphManagerObj, GameObject levelManagerObject)
    {
        isPlayer = false;
        thePlayer = GameObject.Find("Player");
        this.graphManagerObj = graphManagerObj;
        graphManagerScript = graphManagerObj.GetComponent<GraphManager>();
        this.levelManagerObject = levelManagerObject;
        levelManagerScript = levelManagerObject.GetComponent<LevelManager>();
        theMap = levelManagerScript.mapObj.GetComponent<Map>();
        theMap.getTile(currX, currY).hasEnemy = true;
        soundManagerObject = GameObject.Find("SoundManager");
        soundManagerScript = soundManagerObject.GetComponent<SoundManager>();
        battleManagerObject = GameObject.Find("BattleManager");
        battleManagerScript = battleManagerObject.GetComponent<BattleManager>();
        queue = new Queue<Node>();
        bestRoute = new List<Node>();
    }

    void Update()
    {
        currX = (int)transform.position.x;
        currY = (int)transform.position.y;
        if(thePlayer != null)
        {
            if ((Math.Abs(currY - thePlayer.GetComponent<Player>().currY) <= 1) && (Math.Abs(currX - thePlayer.GetComponent<Player>().currX) <= 1))
            {
                faceDirection(new Vector2(thePlayer.transform.position.x, thePlayer.transform.position.y));
                if (!waitingForTurn)
                {
                    StartTimer(waitMilliseconds);
                    int fightResult = battleManagerScript.Fight(thePlayer.GetComponent<Player>(), this.gameObject, false, levelManagerScript.enemyManagerScript);
                    soundManagerScript.EnemyAttack();
                    if (fightResult == (int)BattleManager.FightOutcomes.KILLED)
                    {
                        //THIS IS GAME OVER
                    }
                }
            }
            else if (!movingLong)
            {
                bestRoute = graphManagerScript.BFS(queue, graphManagerScript.getNode((int)this.transform.position.x, (int)this.transform.position.y), graphManagerScript.getNode((int)thePlayer.transform.position.x, (int)thePlayer.transform.position.y).getName());
                if (bestRoute.Count > 0)
                {
                    if (theMap.getTile(this.currX, this.currY).tileFogOfWarType == (int)Tile.TileFogOfWarTypes.VISABLE)
                    {
                        seenPlayer = true;
                    }
                    if (seenPlayer)
                    {
                        MoveEntity(bestRoute, theMap, isPlayer);
                    }
                }
            }
        }
        
    }
}
