using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;

public abstract class MovingObject : MonoBehaviour
{
    Coroutine moveLongRoutine = null;
    Coroutine moveOneRoutine = null;

    public int currX;
    public int currY;
    public int enemyID = -1;
    public Tile currentTile;                            // The Tile that this object is currently on
    private CollisionManager collisionManager;          // Detects collisions with other entities when moving
    public float moveTime = 0.1f;                       // Time it takes object to move (in seconds)
    private float inverseMoveTime;                      // Makes movement calculations more efficient
    public Animator animator;                          // Controls animation
    public string previousAnimation = null;
    private Rigidbody2D rb2D;                           // Store component reference to the rigidbody2D component of unit we're moving
    private bool moveBlocked;
    protected bool movingOne = false;                   // Are we currently moving from one tile to an adjacent tile
    public bool movingLong = false;                  // Are we currently moving from one position on the map to another
    protected bool waitingForTurn = false;
    public Timer timer;

    protected virtual void Start()
    {
        collisionManager = (CollisionManager)this.GetComponent("CollisionManager");
        animator = GetComponent<Animator>();
        rb2D = this.GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; //used to multiply by reciprocal instead of divide (more computationally efficient)
        animator.speed = 0;
        currX = (int)transform.position.x;
        currY = (int)transform.position.y;
    }

    protected void MoveEntity(List<Node> path, Map theMap, bool isPlayer)
    {
        path = checkPath(path); //checks path for errors (fixes a weird bug)

        //If currently moving, assign cancel movement routines and assign new movement
        //if (movingLong)
        //{
        //    stopRoutines(theMap, isPlayer); 
        //}

        //begin move routine
        if (!movingLong)
        {
            moveLongRoutine = StartCoroutine(move(path, theMap, isPlayer));
        }
    }

    IEnumerator move(List<Node> path, Map theMap, bool isPlayer)
    {
        movingLong = true;
        moveBlocked = false;

        //index of next tile to move toward in path sequence
        int pathIndex = 0;

        // lastPosition is the final target node in path sequence
        Node finalPosition;
        finalPosition = path[path.Count - 1];

        while (((currX != finalPosition.getXPos()) || (currY != finalPosition.getYPos())) && !moveBlocked)
        {
            if (!movingOne)
            {
                //confirm no other unit is blocking the move
                Tile nextTile = theMap.getTile(path[pathIndex].getXPos(), path[pathIndex].getYPos());
                moveBlocked = collisionManager.CheckForBlocked(nextTile, theMap);
                if (!moveBlocked)
                {
                    moveOneRoutine = StartCoroutine(MoveOneTile(transform.position, new Vector2(path[pathIndex].getXPos(), path[pathIndex].getYPos()), 1f, theMap, isPlayer));
                    pathIndex++;
                }
            }
            yield return null;
        }

        //move complete, reset position, flags and stop animation
        transform.position = new Vector2((float)Math.Round(transform.position.x), (float)Math.Round(transform.position.y));
        movingLong = false;
    }

    IEnumerator MoveOneTile(Vector2 start, Vector2 end, float speed, Map theMap, bool isPlayer)
    {
        animator.speed = 1;

        //Lock player movement until this move is finished
        movingOne = true;

        //update entity contained status for 2 tiles in question
        updateCurrentTile(theMap, isPlayer, false, (int)start.x, (int)start.y);
        updateCurrentTile(theMap, isPlayer, true, (int)end.x, (int)end.y);

        Tile endTile = theMap.getTile((int)end.x, (int)end.y);

        //play animation
        faceDirection(end);

        //Check for items etc.
        if (isPlayer)
        {
            collisionManager.handleCollision(endTile, theMap);
        }

        //Perform the move
        while (start != end)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime); //move in a straight line toward end position
            rb2D.MovePosition(newPosition);
            start = transform.position;
            yield return null;
        }

        //unlock player movement since the move is finished
        movingOne = false;

        animator.speed = 0;
    }

    //Face entity in the right direction
    protected void faceDirection(Vector2 end)
    {
        if (transform.position.x > end.x) //attempting to move left
        {
            setAnimation("left", previousAnimation);
        }
        else if (transform.position.x < end.x) //attempting to move right
        {
            setAnimation("right", previousAnimation);
        }
        else if (transform.position.y > end.y) //attempting to move down
        {
            setAnimation("down", previousAnimation);
        }
        else if (transform.position.y < end.y) //attempting to move up
        {
            setAnimation("up", previousAnimation);
        }
    }

    //Handles animation
    public void setAnimation(string current, string previous)
    {
        string entityName;
        if (current != previous)
        {
            entityName = this.name.Replace("(Clone)", "");
            entityName = this.name.Replace(" ", "");
            
            animator.SetBool(entityName + "LeftBool", false);
            animator.SetBool(entityName + "RightBool", false);
            animator.SetBool(entityName + "UpBool", false);
            animator.SetBool(entityName + "DownBool", false);
            if (current == "left")
            {
                animator.SetBool(entityName + "LeftBool", true);
            }
            else if (current == "right")
            {
                animator.SetBool(entityName + "RightBool", true);
            }
            else if (current == "down")
            {
                animator.SetBool(entityName + "DownBool", true);
            }
            else if (current == "up")
            {
                animator.SetBool(entityName + "UpBool", true);
            }
        }
        previous = current;
        previousAnimation = current;
    }

    // updates the map object's tile that this object is currently on
    public void updateCurrentTile(Map theMap, bool isPlayer, bool hasEntity, int x, int y)
    {
        if (hasEntity)
        {
            currX = x;
            currY = y;
        }

        currentTile = theMap.getTile(x, y);
        if (isPlayer)
        {
            currentTile.hasPlayer = hasEntity;
        }
        else
        {
            currentTile.hasEnemy = hasEntity;
        }
    }

    //call this to halt current player movement and reset movement flags
    /*public void stopRoutines(Map theMap, bool isPlayer)
    {
        //assign new player position based on current location
        transform.position = new Vector2((float)Math.Round(transform.position.x), (float)Math.Round(transform.position.y));
        //updateCurrentTile(theMap, isPlayer, true);
        movingLong = false;
        animator.speed = 0;

        //halt currently executing movement routines
        StopCoroutine(moveLongRoutine);
        if (moveOneRoutine != null)
        {
            StopCoroutine(moveOneRoutine);
        }

        //reset remaining movement flags
        moveLongRoutine = null;
        moveOneRoutine = null;
        movingOne = false;
    }
    */

    // Confirm we have a valid path for movement
    private List<Node> checkPath(List<Node> path)
    {
        if (path.Count() != path.Distinct().Count())
        {
            path = path.GetRange(0, path.Count() / 2);
        }
        return path;
    }

    // Timer routines below implemented for real-time attack system
    public void StartTimer(int timeMilliseconds)
    {
        timer = new Timer(state => ResetTurn(), null, 0, timeMilliseconds);
    }

    private void ResetTurn()
    {
        if(!waitingForTurn)
        {
            waitingForTurn = true;
        }
        else
        {
            waitingForTurn = false;
            StopTimer();
        }
    }

    private void StopTimer()
    {
        timer.Dispose();
    }

    private void OnDestroy()
    {
        if(timer != null)
        {
            timer.Dispose();
        }
    }
}
