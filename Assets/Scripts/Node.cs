using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Node : MonoBehaviour
public class Node
{
    public bool hasUpNode;
    public bool hasDownNode;
    public bool hasLeftNode;
    public bool hasRightNode;
    public bool wasVisited = false;

    public Node nodeParent = null;
    private Node leftNode = null;
    private Node rightNode = null;
    private Node upNode = null;
    private Node downNode = null;
    private int xPos;
    private int yPos;
    private string nodeName = "";

    public void setName(string name)
    {
        this.nodeName = name;
    }

    public string getName()
    {
        return this.nodeName;
    }

    public int getXPos()
    {
        return xPos;
    }

    public int getYPos()
    {
        return yPos;
    }

    public void setXPos(int x)
    {
        xPos = x;
    }

    public void setYPos(int y)
    {
        yPos = y;
    }

    public void setLeft(Node node)
    {
        hasLeftNode = true;
        leftNode = node;
    }

    public Node getLeft()
    {
        return leftNode;
    }

    public void setRight(Node node)
    {
        hasRightNode = true;
        rightNode = node;
    }

    public Node getRight()
    {
        return rightNode;
    }

    public void setUp(Node node)
    {
        hasUpNode = true;
        upNode = node;
    }

    public Node getUp()
    {
        return upNode;
    }

    public void setDown(Node node)
    {
        hasDownNode = true;
        downNode = node;
    }

    public Node getDown()
    {
        return downNode;
    }
}
