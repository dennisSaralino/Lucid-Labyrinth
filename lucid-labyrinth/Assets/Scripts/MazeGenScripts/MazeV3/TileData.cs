
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TrapMazeType
{
    firetrap,
    watertrap,
    spikestrap,
    logtrap,
    arrowtrap
}


public enum TileType
{
    Side,
    Cell
}
public enum CellType
{
    startTile,
    endTile,
    buildingTile
}
public enum SideType
{
    wall, 
    path,
    door,
    upStair,
    downStair,
}
public enum DecorationType
{
    none,
    wall,
    floor
}
[Serializable]
public class TileData
{
    public Vector3 position;
    public Vector2Int fullPos;
    public TileType tileT;
    public CellType cellT;
    public SideType up;
    public SideType down;
    public SideType left;
    public SideType right;
    public bool isSolutionPath;
    public bool isBranching;
    public bool isInBranch;
    public bool isDeadEnd;
    public float layer = 0;
    public bool visited;
    public bool isStartTile;
    public bool isEndTile;
    public bool haveDecoration;
    public DecorationType decoT;
    public bool haveTrap;
    public TrapMazeType trapT;


    #region SET UPON FINISHED SETUP
    public bool isStair;
    public bool isDoor;
    public bool[] wallSides;
    public SideType[] sideSides;
    public int traprotateable;
    public void setBaseOnSides()
    {
        wallSides = new bool[] { right == SideType.wall, down == SideType.wall, left == SideType.wall, up == SideType.wall };
        sideSides = new SideType[] { right, down, left, up };
        isDoor = (right == SideType.door || left == SideType.door || up == SideType.door || down == SideType.door);
        isStair = (right == SideType.upStair || right == SideType.downStair || left == SideType.upStair || left == SideType.downStair || up == SideType.upStair || up == SideType.downStair || down == SideType.upStair || down == SideType.downStair);
        if (isDoor)
        {
            tileT = TileType.Cell;
            cellT = CellType.buildingTile;
        }
    }
    #endregion
    public TileData()
    {
        up = SideType.wall;
        down = SideType.wall;
        left = SideType.wall;
        right = SideType.wall;
        decoT = DecorationType.none;
    }
    public TileData(alTData d)
    {
        if (d == null) return;
        up = d.u ? SideType.path: SideType.wall;
        down = d.d ? SideType.path : SideType.wall;
        left = d.l ? SideType.path : SideType.wall;
        right = d.r ? SideType.path : SideType.wall;
        isSolutionPath = d.isSolution;
        isBranching = d.isBranching;
        isInBranch = d.isInBranch;
        isDeadEnd = d.isDeadEnd;
        isStartTile = d.isStartT;
        isEndTile = d.isEndT;
        layer = 0;

        if (isStartTile || isEndTile)
        {
            tileT = TileType.Cell;
            cellT = isStartTile ? CellType.startTile : CellType.endTile;
        }
    }
    public bool setDecorationTrue()
    {
        List<DecorationType> possible = new List<DecorationType>();
        possible.Add(DecorationType.wall);
        if (!isStair) possible.Add(DecorationType.floor);

        if (possible.Count > 0)
        {
            decoT = possible[Random.Range(0, possible.Count)];
            haveDecoration = true;
            return true;
        }
        return false;
    }
    public bool setTrap()
    {
        traprotateable = -1;
        List<TrapMazeType> possible = new List<TrapMazeType>();
        if (!isStair && !isDoor && tileT == TileType.Side && decoT != DecorationType.floor)
        {
            possible.Add(TrapMazeType.watertrap);
            possible.Add(TrapMazeType.firetrap);
            possible.Add(TrapMazeType.spikestrap);

            if (wallSides[0] && wallSides[2])
            {
                traprotateable = 0;
            }
            else if (wallSides[1] && wallSides[3])
            {
                traprotateable = 1;
            }
            if(traprotateable != -1)
            {
                possible.Add(TrapMazeType.arrowtrap);
                possible.Add(TrapMazeType.logtrap);
            }
        }
        



        if (possible.Count != 0)
        {
            trapT = possible[Random.Range(0, possible.Count)];
            haveTrap = true;
            return true;
        }
        haveTrap = false;
        return false;
    }
    public TileData(TileData t)
    {
        this.up = t.up;
        this.down = t.down;
        this.left = t.left;
        this.right = t.right;
        this.isSolutionPath = t.isSolutionPath;
    }
    public void setSide(Vector2Int side, SideType value)
    {
        if (side.x == 1) right = value;
        else if (side.x == -1) left = value;
        else if (side.x == 1) up = value;
        else if(side.y == -1) down = value;
    }
    public ref SideType getSide(Vector2Int side)
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

        Vector3 centered = new Vector3(p.transform.position.x, layer * 3.9f, p.transform.position.z);
        this.position = centered;
        p.transform.position = centered;
        GameObject floor = null;
        GameObject rightside = null;
        GameObject upside = null;
        GameObject downside = null;
        GameObject leftside = null;
        if (tileT == TileType.Cell)
        {

            #region CellType
            Transform cell = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[cellT.ToString()], p).transform;
            cell.transform.position = centered;
            cell.GetChild(0).gameObject.SetActive(wallSides[0]);
            cell.GetChild(1).gameObject.SetActive(wallSides[1]);
            cell.GetChild(2).gameObject.SetActive(wallSides[2]);
            cell.GetChild(3).gameObject.SetActive(wallSides[3]);
            if (cellT == CellType.buildingTile)
            {
                if (isDoor)
                {
                    Transform door = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[SideType.door.ToString()], p).transform;
                    door.transform.position = centered;
                    for (int i = 0; i < 4; i++)
                    {
                        if (sideSides[i] == SideType.door)
                        {
                            door.transform.Rotate(Vector3.up, 90f * i);
                            break;
                        }
                    }
                }
            }

            #endregion
        }
        else
        {
            #region SideType
            #region WALLS
            if (haveTrap && trapT == TrapMazeType.logtrap)
            {
                if (traprotateable == 0 && (right != SideType.wall || left != SideType.wall))
                {
                    Debug.Log("WRONGGGGGG");
                }
                if (traprotateable == 1 && (up != SideType.wall || down != SideType.wall))
                {
                    Debug.Log("WRONGGGGGG");
                }

            }
            rightside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[right.ToString()], p);
            rightside.name = "RIGHT";
            upside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[up.ToString()], p);
            upside.name = "UP";
            downside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[down.ToString()], p);
            downside.name = "DOWN";
            leftside = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[left.ToString()], p);
            leftside.name = "LEFT";
            downside.transform.Rotate(Vector3.up, 90f);
            leftside.transform.Rotate(Vector3.up, 180f);
            upside.transform.Rotate(Vector3.up, 270f);

            #endregion
            if (!isStair)
            {
                floor = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict["floor"], p);
                floor.transform.position = centered;
            }
            if (DataToMaze.i.materialDebug)
            {
                #region TESTING MATERIAL
                if (isDeadEnd)
                {
                    Material solutionmaterial = Resources.Load<Material>("Material/isDeadEnd");
                    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                }
                else if (isStair)
                {
                    Material solutionmaterial = Resources.Load<Material>("Material/isStair");
                    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                }
                else if (isDoor)
                {
                    Material solutionmaterial = Resources.Load<Material>("Material/isDoor");
                    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                }
                //else if (isBranching)
                //{
                //    Material solutionmaterial = Resources.Load<Material>("Material/BranchingPath");
                //    floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                //}
                else if (isInBranch)
                {
                    Material solutionmaterial = Resources.Load<Material>("Material/inBranch");
                    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                }
                else if (isSolutionPath)
                {
                    Material solutionmaterial = Resources.Load<Material>("Material/SolutionPath");
                    if (floor != null) floor.transform.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                }
                #endregion
            }
            #endregion
        }



        #region Decoration
        if (haveDecoration)
        {
            Transform deco = null;
            if (decoT == DecorationType.wall)
            {
                deco = UnityEngine.Object.Instantiate(DataToMaze.i.wallDecoration[Random.Range(0, DataToMaze.i.wallDecoration.Count)], p).transform;
               
                for (int i = 0; i < 4; i++)
                {
                    float degree = 90 * i;
                    if (wallSides[i] == true)
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
        if (haveTrap)
        {
            Transform cell = UnityEngine.Object.Instantiate(DataToMaze.i.tileDict[trapT.ToString()], p).transform;
            cell.transform.position = centered;
            if (trapT == TrapMazeType.firetrap || trapT == TrapMazeType.watertrap || trapT == TrapMazeType.spikestrap)
            {
                GameObject.DestroyImmediate(floor.gameObject);
                floor = cell.gameObject;
            }
            else if (trapT == TrapMazeType.logtrap || trapT == TrapMazeType.arrowtrap)
            {
                if (traprotateable == 0)
                {
                    GameObject.Destroy(leftside);
                    GameObject.Destroy(rightside);
                }
                else if (traprotateable == 1)
                {
                    GameObject.Destroy(upside);
                    GameObject.Destroy(downside);
                    cell.transform.Rotate(Vector3.up, 90);
                }
            }
        }
        #endregion


        return p.GetComponentInChildren<NavMeshSurface>();
    }
}



