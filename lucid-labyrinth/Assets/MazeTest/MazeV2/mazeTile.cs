using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum direction
{
    top,
    bottom, left, right
}

public class mazeTile : MonoBehaviour
{
    public List<mazeTile> upOptions;
    public List<mazeTile> downOptions;
    public List<mazeTile> leftOptions;
    public List<mazeTile> rightOptions;




    public void init()
    {
        upOptions = new List<mazeTile>();
        downOptions = new List<mazeTile>();
        leftOptions = new List<mazeTile>();
        rightOptions = new List<mazeTile>();
    }
}

