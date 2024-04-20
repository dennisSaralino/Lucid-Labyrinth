
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public enum tileType
{
    Side,
    Cell
}
public enum CellType
{
    startTile,
    endTile
}
public enum sideType
{
    wall, 
    path,
    door,
    upStair,
    downStair,
}
public enum decorationType
{
    wall,
    floor
}
[Serializable]
public class TileData
{
    public tileType tileT;
    public CellType cellT;
    public sideType up;
    public sideType down;
    public sideType left;
    public sideType right;
    public bool isSolutionPath;
    public bool isBranching;
    public bool isInBranch;
    public bool isDeadEnd;
    public float layer = 0;
    public bool visited;
    public bool isStartTile;
    public bool isEndTile;
    public bool haveDecoration;
    public decorationType decoT;
    public TileData()
    {
        up = sideType.wall;
        down = sideType.wall;
        left = sideType.wall;
        right = sideType.wall;
    }
    public TileData(alTData d)
    {
        if (d == null) return;
        up = d.u ? sideType.path: sideType.wall;
        down = d.d ? sideType.path : sideType.wall;
        left = d.l ? sideType.path : sideType.wall;
        right = d.r ? sideType.path : sideType.wall;
        isSolutionPath = d.isSolution;
        isBranching = d.isBranching;
        isInBranch = d.isInBranch;
        isDeadEnd = d.isDeadEnd;
        isStartTile = d.isStartT;
        isEndTile = d.isEndT;
        layer = 0;

        if (isStartTile || isEndTile)
        {
            tileT = tileType.Cell;
            cellT = isStartTile ? CellType.startTile : CellType.endTile;
        }
    }
    public void setDecorationTrue()
    {
        haveDecoration = true;
        decoT = (decorationType)Random.Range(0, 2);
    }
    public TileData(TileData t)
    {
        this.up = t.up;
        this.down = t.down;
        this.left = t.left;
        this.right = t.right;
        this.isSolutionPath = t.isSolutionPath;
    }
    public void setSide(Vector2Int side, sideType value)
    {
        if (side.x == 1) right = value;
        else if (side.x == -1) left = value;
        else if (side.x == 1) up = value;
        else if(side.y == -1) down = value;
    }
    public ref sideType getSide(Vector2Int side)
    {
        if (side.x == -1) return ref left;
        else if (side.x == 1) return ref right;
        else if (side.y == 1) return ref up;
        else if (side.y == -1) return ref down;
        else
        {
            Debug.Log("WTF are you doing here");
            return ref left;
        }
    }
    public NavMeshSurface loadInto(Transform p)
    {
        bool[] sideWall = new bool[] { right == sideType.wall, down == sideType.wall, left == sideType.wall, up == sideType.wall };


        Vector3 centered = new Vector3(p.transform.position.x, layer * 3.9f, p.transform.position.z);
        p.transform.position = centered;
        bool isThereDOOR = (right == sideType.door || left == sideType.door || up == sideType.door || down == sideType.door);
        bool isThereStair = (right == sideType.upStair || right == sideType.downStair || left == sideType.upStair || left == sideType.downStair || up == sideType.upStair || up == sideType.downStair || down == sideType.upStair || down == sideType.downStair);
        if (tileT == tileType.Cell)
        {

            #region CellType
            Transform cell = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[cellT.ToString()], p).transform;
            cell.GetChild(0).gameObject.SetActive(sideWall[0]);
            cell.GetChild(1).gameObject.SetActive(sideWall[1]);
            cell.GetChild(2).gameObject.SetActive(sideWall[2]);
            cell.GetChild(3).gameObject.SetActive(sideWall[3]);
            #endregion
        }
        else
        {
            #region SideType
            #region WALLS
           
            GameObject rightside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[right.ToString()], p);
            GameObject upside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[up.ToString()], p);
            GameObject downside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[down.ToString()], p);
            GameObject leftside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[left.ToString()], p);
            downside.transform.Rotate(Vector3.up, 90f);
            leftside.transform.Rotate(Vector3.up, 180f);
            upside.transform.Rotate(Vector3.up, 270f);
            GameObject floor = null;
            #endregion
            if (!isThereStair)
            {
                floor = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict["floor"], p);
            }
            #region TESTING MATERIAL
            //if (isDeadEnd)
            //{
            //    Material solutionmaterial = Resources.Load<Material>("Material/isDeadEnd");
            //    if(floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            //}
            //else if (isThereStair)
            //{
            //    Material solutionmaterial = Resources.Load<Material>("Material/isStair");
            //    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            //}
            //else if (isThereDOOR)
            //{
            //    Material solutionmaterial = Resources.Load<Material>("Material/isDoor");
            //    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            //}
            ////else if (isBranching)
            ////{
            ////    Material solutionmaterial = Resources.Load<Material>("Material/BranchingPath");
            ////    floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            ////}
            //else if (isInBranch)
            //{
            //    Material solutionmaterial = Resources.Load<Material>("Material/inBranch");
            //    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            //}
            //else if (isSolutionPath)
            //{
            //    Material solutionmaterial = Resources.Load<Material>("Material/SolutionPath");
            //    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
            //}
            #endregion
            #endregion
        }



        #region Decoration
        if (haveDecoration)
        {
            Transform deco = null;
            if (decoT == decorationType.wall)
            {
                deco = UnityEngine.Object.Instantiate(DataToMaze.i.wallDecoration[Random.Range(0, DataToMaze.i.wallDecoration.Count)], p).transform;
               
                for (int i = 0; i < 4; i++)
                {
                    float degree = 90 * i;
                    if (sideWall[i] == true)
                    {
                        deco.transform.Rotate(Vector3.up, degree);
                        break;
                    }
                }
            }
            else
            {
                deco = UnityEngine.Object.Instantiate(DataToMaze.i.floorDecoration[Random.Range(0, DataToMaze.i.floorDecoration.Count)], p).transform;
                float degree = 90 * Random.Range(0, 4);
                deco.transform.Rotate(Vector3.up, degree);
            }
            deco.transform.position = centered;
        }
        #endregion

        #region TRAP
        #endregion



        return p.GetComponentInChildren<NavMeshSurface>();
    }
}
