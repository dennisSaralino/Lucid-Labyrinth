
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool isSolutionPath;
    public Vector2 pdir1;
    public Vector2 pdir2;
    public TileData()
    {
        up = false;
        down = false;
        left = false;
        right = false;
    }
    public TileData(TileData t)
    {
        this.up = t.up;
        this.down = t.down;
        this.left = t.left;
        this.right = t.right;
        this.isSolutionPath = t.isSolutionPath;
    }
    public void loadInto(Transform p)
    {
        #region WALLS
        if (up)
        {
            p.GetChild(1).gameObject.SetActive(false);
        }

        if (down)
        {
            p.GetChild(2).gameObject.SetActive(false);
        }
        if (left)
        {
            p.GetChild(3).gameObject.SetActive(false);
        }
        if (right)
        {
            p.GetChild(4).gameObject.SetActive(false);
        }
        #endregion
        #region TRAP
        #endregion
    }
}
