using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public GameObject FogOfWar_INVISABLE;
    public GameObject FogOfWar_VISITED;

    public int radius;
    private List<Vector2> lineOfSightEdgePoints; // list of points on the edge of the circular line of sight, based off radius

    private GameObject playerObj;
    private Tile playerTile;
    private Map map;

    private List<GameObject> fogOfWarObjs;

    // This creates the fog of war
    public void createFogOfWar(GameObject mapObj, GameObject playerObj)
    {
        this.playerObj = playerObj;
        playerTile = playerObj.GetComponent<Player>().currentTile;
        map = mapObj.GetComponent<Map>();
        fogOfWarObjs = new List<GameObject>();
        lineOfSightEdgePoints = new List<Vector2>();

        // Compute the line of sight from the set radius
        computeLineOfSight();

        // All tiles are invisable to start
        setAllInvisable();

        // First to instantiate the FogOfWar objects
        drawFogOfWar();

        // update the starting fogofwar for the player
        updateFogOfWar();
        drawFogOfWar();
    }

    // Compute the edge points (x, y) for line of sight from radius, these are used as offsets. Done only once 
    private void computeLineOfSight()
    {
        double i, angle, x1, y1;

        for (i = 0; i < 360; i += 1) {
            angle = i;
            x1 = radius * Math.Cos(angle * Math.PI / 180);
            y1 = radius * Math.Sin(angle * Math.PI / 180);

            float X = (float) Math.Round(x1);
            float Y = (float) Math.Round(y1);

            if(!lineOfSightEdgePoints.Contains(new Vector2(X, Y)))
                lineOfSightEdgePoints.Add(new Vector2(X, Y));
        }
    }

    // Sets all the tiles as invisable
    public void setAllInvisable()
    {
        for (int x = 0; x < map.MapXSize; x++)
        {
            for (int y = 0; y < map.MapYSize; y++)
            {
                map.getTile(x, y).tileFogOfWarType = (int)Tile.TileFogOfWarTypes.INVISABLE;
                map.getTile(x, y).tileFogOfWarTypeHasChanged = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player moved a tile
        if(playerObj != null)
        {
            Tile newPlayerTile = playerObj.GetComponent<Player>().currentTile;

            if (newPlayerTile.xCor != playerTile.xCor || newPlayerTile.yCor != playerTile.yCor)
            {
                playerTile = newPlayerTile;

                updateFogOfWar();

                drawFogOfWar();
            }
        }
    }

    public void updateFogOfWar()
    {
        // Set previously visable tiles to visited
        for (int x = 0; x < map.MapXSize; x++)
        {
            for (int y = 0; y < map.MapYSize; y++)
            {
                if(map.getTile(x, y).tileFogOfWarType == (int)Tile.TileFogOfWarTypes.VISABLE)
                {
                    map.getTile(x, y).tileFogOfWarType = (int)Tile.TileFogOfWarTypes.VISITED;
                    map.getTile(x, y).tileFogOfWarTypeHasChanged = true;
                }
            }
        }

        // Compute new visable tiles for everything inside the line of sight
        // Iterate over the points(x, y) from current player tile position to the edge of the pre-computed line of sight
        // If there is a wall or door, then stop and continue with the next edge point
        Vector2 playerPoint = new Vector2(playerTile.xCor, playerTile.yCor); 
        for (int i = 0; i < lineOfSightEdgePoints.Count; i++)
        {
            Vector2 edgePoint = new Vector2(playerTile.xCor + lineOfSightEdgePoints[i].x, playerTile.yCor + lineOfSightEdgePoints[i].y);
            List<Vector2> linePoints = getLinePoints(playerPoint, edgePoint);

            // iterate over the tiles along the line points
            bool stop = false;
            for (int j = 0; j < linePoints.Count && !stop; j++)
            {
                Tile t = map.getTile((int)linePoints[j].x, (int)linePoints[j].y);
                //Debug.Log(t.xCor + " -- " + t.yCor + " : " + linePoints[j].x + " -- " + linePoints[j].y);

                if(t.tileType == (int)Tile.TileTypes.WALL || (!playerTile.tileEntityName.Contains("door") && t.tileEntityName.Contains("door")))
                    stop = true;
                
                t.tileFogOfWarType = (int)Tile.TileFogOfWarTypes.VISABLE;
                t.tileFogOfWarTypeHasChanged = true;
            }

        }

    }

    // Source: https://www.redblobgames.com/grids/line-drawing.html
    // Following are used to get the points for the line of sight to be calculated
    // These first ones are just helper functions

    private float lerp(float start, float end, float t) 
    {
        return start + t * (end-start);
    }

    private Vector2 lerp_point(Vector2 p0, Vector2 p1, float t) 
    {
        return new Vector2(lerp(p0.x, p1.x, t), lerp(p0.y, p1.y, t));
    }

    private float diagonal_distance(Vector2 p0, Vector2 p1) 
    {
        var dx = p1.x - p0.x;
        var dy = p1.y - p0.y;
        return Math.Max(Math.Abs(dx), Math.Abs(dy));
    }

    private Vector2 round_point(Vector2 p) 
    {
        return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
    }

    // This is the main one to be called
    // Computes a list of points Vector2(x, y) from the player(p0) to the edge of his line of sight(p1)
    private List<Vector2> getLinePoints(Vector2 p0, Vector2 p1) 
    {
        List<Vector2> points = new List<Vector2>();
        var N = diagonal_distance(p0, p1);
        for (var step = 0; step <= N; step++) {
            float t = N == 0 ? (float)0.0 : (float)(step / N);
            points.Add(round_point(lerp_point(p0, p1, t)));
        }

        return points;
    }

    public void drawFogOfWar()
    {
        // draw the new fog of war objects
        for (int x = 0; x < map.MapXSize; x++)
        {
            for (int y = 0; y < map.MapYSize; y++)
            {
                int tileFogOfWarType = map.getTile(x, y).tileFogOfWarType;
                bool tileFogOfWarTypeHasChanged = map.getTile(x, y).tileFogOfWarTypeHasChanged;
                GameObject f = null;

                // Find and remove exisitng tile
                if(tileFogOfWarTypeHasChanged)
                {
                    GameObject fowObj = GameObject.Find("fogOfWar_" + x + "_" + y);
                    fogOfWarObjs.Remove(fowObj);
                    Destroy(fowObj);
                }

                if (tileFogOfWarType == (int)Tile.TileFogOfWarTypes.VISABLE || !tileFogOfWarTypeHasChanged)
                {
                    continue;
                }

                if (tileFogOfWarType == (int)Tile.TileFogOfWarTypes.INVISABLE)
                {
                    f = Instantiate(FogOfWar_INVISABLE);
                }
                else if (tileFogOfWarType == (int)Tile.TileFogOfWarTypes.VISITED)
                {
                    f = Instantiate(FogOfWar_VISITED);
                }

                if(f != null)
                {
                    f.transform.SetParent(this.transform);
                    f.transform.position = new Vector3(x * Tile.DRAW_SIZE, y * Tile.DRAW_SIZE, 0);
                    f.name = "fogOfWar_" + x + "_" + y;
                    fogOfWarObjs.Add(f);
                    map.getTile(x, y).tileFogOfWarTypeHasChanged = false;
                }

            }

        }
    }

}

