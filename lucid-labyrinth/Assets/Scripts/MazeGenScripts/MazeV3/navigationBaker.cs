using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using System.Linq;
public class navigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;
    public static navigationBaker baker;
    public void Awake()
    {
        if (baker == null) baker = this;
        else Destroy(this.gameObject);
    }
    public void bakeMap()
    {
        surface.BuildNavMesh();
    }
    public void bakeMap(List<NavMeshSurface> l)
    {
        StartCoroutine(bakeMapSlowly(l));
    }
    static IEnumerator bakeMapSlowly(List<NavMeshSurface> l)
    {
        int j = 10;
        foreach (NavMeshSurface i in l)
        {
            if (j == 0)
            {
                yield return null;
                j = 10;
            }
            i.BuildNavMesh();
            j--;   
        }
    }
}
