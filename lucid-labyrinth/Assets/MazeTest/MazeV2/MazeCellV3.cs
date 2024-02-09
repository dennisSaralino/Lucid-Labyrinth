using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class MazeCellV3 : MonoBehaviour
{
    public bool finished { get; private set; }
    int x, y;
    public List<mazeTile> tileOptions;
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

    public void finishThisCell(string prefer = "null",bool contains = false)
    {
        finished = true;
        if (prefer == "null") finalTile = tileOptions[Random.Range(0, tileOptions.Count)];
        else
        {
            if (contains)
            {
                List<mazeTile> m = tileOptions.FindAll(x =>
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
            else finalTile = tileOptions.Find(x => x.name == prefer);

        }

        Transform f = Instantiate(finalTile, transform).transform;
        f.localPosition = new Vector3(0, 0, 0);

        tileOptions.Clear();
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
    /// <param name="possible">tiles that can survive the source</param>
    void collapse(List<mazeTile> possible)
    {
        if(finished)return;
        tileOptions = tileOptions.Intersect(possible).ToList();
        if (tileOptions.Count == 0) Debug.Log("WRONGGGGGGG");
    }

    
}
