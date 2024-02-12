using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class navigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;
    public void bakeMap()
    {
        surface.BuildNavMesh();
    }
}
