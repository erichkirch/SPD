using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    private Map theMap;
    private List<Node> nodes;
    private List<Node> bestRoute;
    private Node nextNode;
    private Node parent;

    public void setUp(LevelManager levelManager)
    {
        theMap = levelManager.mapObj.GetComponent<Map>();
        nodes = new List<Node>();
        bestRoute = new List<Node>();
        createNodes();
        createEdges();
    }

    public Node getNode(int x, int y)
    {
        return nodes.Find(z => z.getName().Equals("Node_" + x.ToString() + "_" + y.ToString()));
    }

    private void createNodes()
    {
        //create a node object for each floor tile and give it a name for ID
        for(int x = 0; x < (theMap.getXSize()); x++)
        {
            for(int y = 0; y < (theMap.getYSize()); y++)
            {
                if(theMap.getTile(x, y).getTileType() == (int)Tile.TileTypes.FLOOR)
                {
                    Node newNode = new Node();
                    newNode.setXPos(x);
                    newNode.setYPos(y);
                    newNode.setName("Node_" + x + "_" + y);
                    nodes.Add(newNode);
                }
            }
        }
    }

    private void createEdges()
    {
        //create edges between nodes based on named ID
        foreach(Node temp in nodes)
        {
            if (nodes.Exists(z => z.getName().Equals("Node_" + (temp.getXPos() - 1).ToString() + "_" + temp.getYPos().ToString())))
            {
                temp.setLeft(nodes.Find(z => z.getName().Equals("Node_" + (temp.getXPos() - 1).ToString() + "_" + temp.getYPos().ToString())));
            }
            if (nodes.Exists(z => z.getName().Equals("Node_" + (temp.getXPos() + 1).ToString() + "_" + temp.getYPos().ToString())))
            {
                temp.setRight(nodes.Find(z => z.getName().Equals("Node_" + (temp.getXPos() + 1).ToString() + "_" + temp.getYPos().ToString())));
            }
            if (nodes.Exists(z => z.getName().Equals("Node_" + temp.getXPos().ToString() + "_" + (temp.getYPos() + 1).ToString())))
            {
                temp.setUp(nodes.Find(z => z.getName().Equals("Node_" + temp.getXPos().ToString() + "_" + (temp.getYPos() + 1).ToString())));
            }
            if (nodes.Exists(z => z.getName().Equals("Node_" + temp.getXPos().ToString() + "_" + (temp.getYPos() - 1).ToString())))
            {
                temp.setDown(nodes.Find(z => z.getName().Equals("Node_" + temp.getXPos().ToString() + "_" + (temp.getYPos() - 1).ToString())));
            }
        }
    }

    public List<Node> BFS(Queue<Node> queue, Node current, string targetName)
    {
        //initiate breadth-first search for target node
        bestRoute = new List<Node>();
        queue.Enqueue(current);

        while (queue.Count > 0)
        {
            nextNode = queue.Dequeue();

            if (!nextNode.wasVisited)
            {
                queue = visitNode(queue, nextNode, targetName);
            }
            if(nextNode.getName() == targetName)
            {
                //if we've found the target node
                while(nextNode.nodeParent != null)
                {
                    bestRoute.Add(nextNode);
                    nextNode = nextNode.nodeParent;
                }
                bestRoute.Reverse();
            }
        }
        resetNodes();
        return bestRoute;
    }

    private Queue<Node> visitNode(Queue<Node> queue, Node current, string targetName)
    {
        //add child nodes to the queue and mark this node as visited
        current.wasVisited = true;
        if (current.hasDownNode && !current.getDown().wasVisited)
        {
            queue.Enqueue(current.getDown());
            current.getDown().nodeParent = current;
        }
        if (current.hasUpNode && !current.getUp().wasVisited)
        {
            queue.Enqueue(current.getUp());
            current.getUp().nodeParent = current;
        }
        if (current.hasLeftNode && !current.getLeft().wasVisited)
        {
            queue.Enqueue(current.getLeft());
            current.getLeft().nodeParent = current;
        }
        if (current.hasRightNode && !current.getRight().wasVisited)
        {
            queue.Enqueue(current.getRight());
            current.getRight().nodeParent = current;
        }
        return (queue);
    }

    private void resetNodes()
    {
        foreach(Node temp in nodes)
        {
            temp.nodeParent = null;
            temp.wasVisited = false;
        }
    }
}
