using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MovingObject
{
    public static Player player;
    // Stats ---------------------
    public int level = 0;
    // public int health = baseHealth;
    public int playerAttackDmg = 10;
    public int playerHP;
    public bool dead = false;
    public int exp = 0;

    public readonly int[] expToNext = {0,10,25,45,70,100,135,175,220,270,325,385,450,520,595,675,760,850,945,1045,1150,1260,1375,1495,1620,1750};
    public const int maxLevel = 26;

    public const int baseHealth = 70;
    public const int healthIncreasePerLevel = 10;

    public const int baseDamageMin = 2;
    public const int baseDamageMax = 3;
    public const int damageIncreasePerLevel = 1;

    public const int baseDodge = 5;
    public const int baseHit = 5;
    public const int dodgeHitIncreasePerLevel = 5;

    public const int baseCrit = 5;
    public const int critIncreasePerLevel = 1;

    public readonly int[] fail = {5,5,5,4,4,4,4,4,3,3,3,3,3,2,2,2,2,2,1,1,1,1,1,0,0,0};
    
    public const int baseStrength = 10;
    public const int strengthIncreasePerLevel = 1;

    public void grantXP(int gained) {
        exp += gained;
        if(exp >= expToNext[1+level]) {
            exp -= expToNext[1+level];
            level++;
        }
        IngameUI.xp.text = "Level "+(1+level)+"            "+exp+" xp";
        float v = ((float)exp / (float)expToNext[1+level]);
        // Debug.Log("Fill "+v);
        IngameUI.xpBar.fillAmount = v;
    }
    public static void updateHealth(int change) {
        player.playerHP += change;

        if(player.playerHP > (Player.baseHealth+Player.healthIncreasePerLevel*player.level)) {
            player.playerHP = (Player.baseHealth+Player.healthIncreasePerLevel*player.level);
        } else if(player.playerHP <= 0) {
            player.dead = true;
            IngameUI.health.text = "DEAD";
            IngameUI.healthBar.fillAmount = 0.0f;
        }

        if(!player.dead) {
            IngameUI.health.text = player.playerHP+" / "+(Player.baseHealth+Player.healthIncreasePerLevel*player.level);
            IngameUI.healthBar.fillAmount = (float)player.playerHP / (float)(Player.baseHealth+Player.healthIncreasePerLevel*player.level);
        }
    }

    // Non-stats ---------------------
    public Interactable focus;	// Our current focus: Item, Enemy etc.
    public GameObject levelManagerObject;
    public LevelManager levelManagerScript;
    public GameObject graphManagerObject;
    public GraphManager graphManagerScript;
    public GameObject battleManagerObject;
    public BattleManager battleManagerScript;
    public GameObject soundManagerObject;
    public SoundManager soundManagerScript;
    public Map theMap;

    private int waitMilliseconds = 500;
    private int clickedX;
    private int clickedY;
    private bool isPlayer;
    private Queue<Node> queue;
    private List<Node> bestRoute;
    public Tile theTile;
    public Tile lastClickedTile;

    protected override void Start()
    {
        base.Start();
        player = this;
    }

    public void setUpPlayerMap(GameObject mapObj)
    {
        isPlayer = true;
        playerHP = baseHealth; //for some reason we need to assign this after player creation

        levelManagerScript = levelManagerObject.GetComponent<LevelManager>();
        theMap = levelManagerScript.mapObj.GetComponent<Map>();
        graphManagerScript = graphManagerObject.GetComponent<GraphManager>();
        battleManagerObject = GameObject.Find("BattleManager");
        battleManagerScript = battleManagerObject.GetComponent<BattleManager>();
        soundManagerObject = GameObject.Find("SoundManager");
        soundManagerScript = soundManagerObject.GetComponent<SoundManager>();

        queue = new Queue<Node>();
        bestRoute = new List<Node>();
    }

    void Update()
    {
        //if left click detected
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D object_hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            //if we clicked on something valid
            if (object_hit.transform != null)
            {
                //update coords of clicked element
                clickedX = (int)object_hit.transform.position.x;
                clickedY = (int)object_hit.transform.position.y;

                lastClickedTile = theMap.getTile(clickedX, clickedY);

                // current player position
                currX = (int)transform.position.x;
                currY = (int)transform.position.y;
                faceDirection(new Vector2(clickedX, clickedY));

                // check if the user clicked on an enemy to attack and that enemy is adjacent
                if (!waitingForTurn && (theMap.getTile(clickedX, clickedY).hasEnemy) && ((Math.Abs(currX - clickedX) <= 1) && (Math.Abs(currY - clickedY) <= 1)))
                {
                    //begin the attack timer to space out attacks
                    StartTimer(waitMilliseconds);

                    //find the enemy we clicked on and attack it
                    GameObject[] enemies = levelManagerScript.enemyManagerScript.enemies;
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (enemies[i] != null)
                        {
                            if (enemies[i].transform.position.x == clickedX && enemies[i].transform.position.y == clickedY)
                            {
                                int fightResult = battleManagerScript.Fight(this, enemies[i], true, levelManagerScript.enemyManagerScript);
                                soundManagerScript.PlayerAttack();

                                //if killed the enemy
                                if (fightResult == (int)BattleManager.FightOutcomes.KILLED)
                                {
                                    Destroy(enemies[i]);
                                    lastClickedTile.hasEnemy = false;
                                    //GameObject skeletonPrefab = levelManagerScript.enemyManagerScript.skeletonPrefab;
                                    //GameObject skeletonClone = Instantiate(skeletonPrefab);
                                    //skeletonClone.transform.SetParent(this.transform);
                                    //skeletonClone.transform.position = new Vector2(clickedX, clickedY);
                                }
                            }
                        }
                    }
                }
                //if the user clicked a new floor tile
                else if (!movingLong && (theMap.getTile(clickedX, clickedY).tileType == (int)Tile.TileTypes.FLOOR) && (currX != clickedX || currY != clickedY))
                {
                    //get the fastest route to the clicked tile
                    bestRoute = graphManagerScript.BFS(queue, graphManagerScript.getNode(currX, currY), graphManagerScript.getNode(clickedX, clickedY).getName());

                    //attempt to move
                    MoveEntity(bestRoute, this.theMap, isPlayer);
                }
            }
        }
    }

    //Set our focus to a new focus
    void SetFocus(Interactable newFocus)
    {
        //If our focus has changed
        if (newFocus != focus)
        {
            //Defocus the old one
            if (focus != null)
                focus.OnDefocused();

            focus = newFocus;   // Set our new focus
                                //motor.FollowTarget(newFocus);	// Follow the new focus
        }
        newFocus.OnFocused(transform);
    }

    //Remove our current focus
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();
        focus = null;
        //motor.StopFollowingTarget();
    }
}