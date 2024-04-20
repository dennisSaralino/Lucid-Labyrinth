
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
public enum sideType
{
    wall, 
    path,
    door,
    upStair,
    downStair,
}
[Serializable]
public class TileData
{
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
        layer = 0;
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
        #region WALLS
        p.transform.position = new Vector3(p.transform.position.x, layer * 3.9f, p.transform.position.z);
        GameObject rightside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[right.ToString()], p);
        GameObject upside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[up.ToString()], p);
        GameObject downside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[down.ToString()], p);
        GameObject leftside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[left.ToString()], p);
        downside.transform.Rotate(Vector3.up, 90f);
        leftside.transform.Rotate(Vector3.up, 180f);
        upside.transform.Rotate(Vector3.up, 270f);
        GameObject floor = null;
        #endregion


        bool isThereDOOR = (right == sideType.door || left == sideType.door || up == sideType.door || down == sideType.door);
        bool isThereStair = (right == sideType.upStair || right == sideType.downStair || left == sideType.upStair || left == sideType.downStair || up == sideType.upStair || up == sideType.downStair || down == sideType.upStair || down == sideType.downStair);
        if (!isThereStair)
        {
            floor = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict["floor"], p);
        }
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


        #region TRAP
        #endregion



        return p.GetComponentInChildren<NavMeshSurface>();
    }
}
