using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileDataEditor : MonoBehaviour
{
    public TileDataEditorData data;
}
[System.Serializable]
public class TileDataEditorData
{

    public TileData tile;
    public alTData alData;
    public TileDataEditorData(TileData t0, alTData t)
    {
        tile = t0;
        alData = t;
    }
}