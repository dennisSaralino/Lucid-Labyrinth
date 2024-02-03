using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public Transform mazeCellPre;
    public Transform mazeParent;
    public int width = 10;
    public int height = 10;
    mazeCell[,] mazeGrid;

    public void Awake()
    {
        generateMaze();
    }
    public void initMazeData()
    {
        destroyChildOb(mazeParent);
        mazeGrid = new mazeCell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mazeGrid[i, j] = new mazeCell(i,j);
            }
        }
        
    }
    public static void destroyChildOb(Transform parent)
    {
        foreach (Transform i in parent)
        {
            GameObject.Destroy(i.gameObject);
        }
    }
    public void generateMaze()
    {
        initMazeData();
        visitCell(null, mazeGrid[0, 0]);
        drawCell();
    }
    public void drawCell()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mazeCell cdata = mazeGrid[i, j];
                Transform n = Instantiate(mazeCellPre, mazeParent);
                n.transform.localPosition = new Vector3(i, 0, j);
                if (cdata.left) n.GetChild(0).gameObject.SetActive(true);
                if (cdata.right) n.GetChild(1).gameObject.SetActive(true);
                if (cdata.front) n.GetChild(2).gameObject.SetActive(true);
                if (cdata.back) n.GetChild(3).gameObject.SetActive(true);
            }
        }
    }


    void visitCell(mazeCell pCell, mazeCell cCell)
    {
        cCell.visit();
        clearWalls(pCell, cCell);

        mazeCell nextCell;
        do
        {
            nextCell = getRandomNeighbourUnvisitedCell(cCell);
            if (nextCell != null)
            {
                visitCell(cCell, nextCell);
            }
        } while (nextCell != null);
    }
    private mazeCell getRandomNeighbourUnvisitedCell(mazeCell cCell)
    {
        IEnumerable<mazeCell> unvisitedNei = getNextUnvisitedCells(cCell);
        if (unvisitedNei.Count() == 0) return null;
        else return unvisitedNei.ElementAt(Random.Range(0, unvisitedNei.Count()));
    }
    private IEnumerable<mazeCell> getNextUnvisitedCells(mazeCell cCell)
    {
        if (cCell.x + 1 < width)
        {
            mazeCell possible = mazeGrid[cCell.x + 1, cCell.y];
            if(!possible.isVisit)yield return possible;
        }
        if (cCell.x - 1 >= 0)
        {
            mazeCell possible = mazeGrid[cCell.x - 1, cCell.y];
            if (!possible.isVisit) yield return possible;
        }
        if (cCell.y + 1 < height)
        {
            mazeCell possible = mazeGrid[cCell.x, cCell.y + 1];
            if (!possible.isVisit) yield return possible;
        }
        if (cCell.y - 1 >= 0)
        {
            mazeCell possible = mazeGrid[cCell.x, cCell.y - 1];
            if (!possible.isVisit) yield return possible;
        }
    }
    void clearWalls(mazeCell pCell, mazeCell cCell)
    {
        if (pCell == null) return;
        //cursor moveforward
        if (pCell.x < cCell.x)
        {
            pCell.deleteRight();
            cCell.deleteLeft();
        }
        else if (pCell.x > cCell.x)
        {
            pCell.deleteLeft();
            cCell.deleteRight();
        }
        else if (pCell.y < cCell.y)
        {
            pCell.deleteFront();
            cCell.deleteBack();
        }
        else if (pCell.y > cCell.y)
        {
            pCell.deleteBack();
            cCell.deleteFront();
        }
    }

}
public class mazeCell
{
    public int x;
    public int y;
    public bool isVisit;

    public bool left;
    public bool right;
    public bool front;
    public bool back;
    public mazeCell(int cx, int cy)
    {
        x = cx;
        y = cy;
        left = true;
        right = true;
        front = true;
        back = true;
        isVisit = false;
    }


    public void visit()
    {
        isVisit = true;
    }

    public void deleteLeft()
    {
        left= false;
    }
    public void deleteRight()
    {
        right= false;
    }
    public void deleteFront() 
    {
        front= false;
    }
    public void deleteBack() 
    {
        back = false;
    }
}