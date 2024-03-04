using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMaze : MonoBehaviour
{
    public static DataToMaze i;
    public Vector3 startPos;
    public void Awake()
    {
        if (i == null) i = this;
        else Destroy(this.gameObject);
        startPos = new Vector3(-1000, -1000, -1000);
        transform.position = startPos;
    }


    public void dataToMaze(TileData[,] data)
    {
        StartCoroutine(dataToMazeI(data));   
    }
    IEnumerator dataToMazeI(TileData[,] data)
    {
        Transform prefab = Resources.Load<Transform>("GameObject/3dTile");
        Material solutionmaterial = Resources.Load<Material>("Material/SolutionPath");
        MeshRenderer m = prefab.GetChild(0).GetComponent<MeshRenderer>();
        Vector3 tileSize = m.bounds.size;
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                Transform p = Instantiate(prefab, transform);
                TileData currentData = data[j, i];
                p.localPosition = new Vector3(tileSize.x * j, 0, tileSize.z * i);
                if (data[j, i].isSolutionPath)
                    p.GetChild(0).GetComponent<MeshRenderer>().material = solutionmaterial;
                currentData.loadInto(p);

            }
            yield return null;
        }

    }
}
