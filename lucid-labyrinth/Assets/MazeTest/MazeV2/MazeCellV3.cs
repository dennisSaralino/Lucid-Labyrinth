using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class MazeCellV3 : MonoBehaviour
{
    public bool finished { get; private set; }
    int x, y;
    public List<mazeTile> finalOptionList;
    public mazeTile finalTile;


    public TileData getTileData()
    {
        return finalTile.tileData;
    }

    public void initCell(int x, int y)
    {
        finished = false;
        this.x = x;
        this.y = y;
    }


    /// <summary>
    /// Chose a tile from Final Options List
    /// </summary>
    /// <param name="prefer"></param>
    /// <param name="contains"></param>
    public void finishThisCell(string prefer = "null",bool contains = false)
    {
        finished = true;
        if (prefer == "null") finalTile = finalOptionList[Random.Range(0, finalOptionList.Count)];
        else
        {
            if (contains)
            {
                List<mazeTile> m = finalOptionList.FindAll(x =>
                {
                    bool ok = true;
                    for (int i = 0; i < prefer.Length; i++)
                    {
                        ok &= x.name.Contains(prefer[i]);
                    }
                    return ok;
                }).ToList();
                finalTile = m[Random.Range(0, m.Count)];
            }
            else finalTile = finalOptionList.Find(x => x.name == prefer);

        }

        Transform f = Instantiate(finalTile, transform).transform;
        f.localPosition = new Vector3(0, 0, 0);
        finalOptionList.Clear();
        createWaves();
    }
    /// <summary>
    /// Create waves that collapse every mazeCell surrounding the current Cell.
    /// </summary>
    void createWaves()
    {
        int width = MazeGeneratev2.i.width;
        int height = MazeGeneratev2.i.height;
        if (StaticTool.inGrid(x, y + 1, width, height)) MazeGeneratev2.i.mazeGrid[x, y + 1].collapse(finalTile.upOptions);
        if (StaticTool.inGrid(x, y - 1, width, height)) MazeGeneratev2.i.mazeGrid[x, y - 1].collapse(finalTile.downOptions);
        if (StaticTool.inGrid(x - 1, y, width, height)) MazeGeneratev2.i.mazeGrid[x - 1, y].collapse(finalTile.leftOptions);
        if (StaticTool.inGrid(x + 1, y + 1, width, height)) MazeGeneratev2.i.mazeGrid[x + 1, y].collapse(finalTile.rightOptions);
    }

    /// <summary>
    /// Collapsed by a wave made by a mazeTile source
    /// </summary>
    /// <param name="possible">tiles that can survive the wave</param>
    void collapse(List<mazeTile> possible)
    {
        if(finished)return;
        finalOptionList = finalOptionList.Intersect(possible).ToList();
        if (finalOptionList.Count == 0) Debug.Log("WRONGGGGGGG");
    }

    
}
