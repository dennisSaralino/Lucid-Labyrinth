using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
public class EditorHelper : MonoBehaviour
{
    const int up = 2;
    const int down = 3;
    const int left = 0;
    const int right = 1;
    [MenuItem("EditorHelper/Tiles/generateTilePrefab")]
    public static void generatePrefab()
    {
        string scenePath = "Assets/Scenes/quocTestScene2.unity";
        var currentScene = EditorSceneManager.OpenScene(scenePath);
        mazeTile tpre = Resources.Load<mazeTile>("GameObject/SampleTile");
        Transform parent = GameObject.Find("SampleParent").transform;

        int[] rotationsOptionsOfTiles = new int[] {1,1,2,2,2,1,2,2,2 };
        int currentTileRotation = 0;
        int currentRotationIndex = 0;
        Vector2 currentPos = new Vector2(0, 0);
        Debug.Log(tpre == null);
        Debug.Log(parent == null);

        for (int i = 0; i < 15; i++)
        {
            if (currentTileRotation == rotationsOptionsOfTiles[currentRotationIndex])
            {
                currentTileRotation = 0;
                currentRotationIndex++;
                currentPos.x = 0;
                currentPos.y -= 3;
            }
            mazeTile m = Instantiate(tpre, parent);
            m.transform.localPosition = new Vector3(currentPos.x, 0, currentPos.y);


            currentPos.x += 3f;
            currentTileRotation++;
        }

        EditorSceneManager.SaveScene(currentScene);
        EditorSceneManager.CloseScene(currentScene, true);
        return;
    }

    [MenuItem("EditorHelper/Tiles/modifyTileToMatchName")]
    public static void modifyTileByName()
    {
        MazeGeneratev2 mGen = Resources.Load("GameObject/MazeGen").GetComponent<MazeGeneratev2>();
        mGen.tileRulesDatabase = new tileRulesDatabase();
        tileRulesDatabase dtb = mGen.tileRulesDatabase;

        List<Transform> parent = StaticTool.loadAllAsset<Transform>("MazeTest/MazeV2/MazeTile").ToList();
        foreach (Transform i in parent)
        {
            string iName = i.name;
            mazeTile t = i.GetComponent<mazeTile>();
            if (iName.Contains("u"))
            {
                dtb.pup.Add(t);
                i.GetChild(up).gameObject.SetActive(false);
            }
            else
            {
                dtb.wup.Add(t);
            }


            if (iName.Contains("d"))
            {
                dtb.pdown.Add(t);
                i.GetChild(down).gameObject.SetActive(false);
            }
            else
            {
                dtb.wdown.Add(t);
            }


            if (iName.Contains("l"))
            {
                dtb.pleft.Add(t);
                i.GetChild(left).gameObject.SetActive(false);
            }
            else
            {
                dtb.wleft.Add(t);
            }


            if (iName.Contains("r"))
            {
                dtb.pright.Add(t);
                i.GetChild(right).gameObject.SetActive(false);
            }
            else
            {
                dtb.wright.Add(t);
            }

            StaticTool.saveAsset(i.gameObject);


        }
        StaticTool.saveAsset(mGen.gameObject);
    }



    [MenuItem("EditorHelper/Tiles/addColliderComponent")]
    public static void addComponent()
    {
        List<GameObject> g = StaticTool.loadAllAsset<GameObject>("MazeTest/MazeV2/Tile").ToList();
        foreach (GameObject i in g)
        {
            foreach (Transform x in i.transform)
            {
                if (x.gameObject.GetComponent<BoxCollider>() == null)
                {
                    x.AddComponent<BoxCollider>();
                }
            }
            StaticTool.saveAsset(i);
        }
    }




    [MenuItem("EditorHelper/Tiles/SetTileRules")]
    public static void setTilesRules()
    {
        List<mazeTile> allT = StaticTool.loadAllAsset<mazeTile>("MazeTest/MazeV2/MazeTile").ToList();
        tileRulesDatabase database = Resources.Load<MazeGeneratev2>("GameObject/MazeGen").tileRulesDatabase;
        foreach (mazeTile i in allT)
        {
            i.init();
            i.upOptions.AddRange(i.name.Contains("u") ? database.pdown : database.wdown);
            i.downOptions.AddRange(i.name.Contains("d") ? database.pup : database.wup);
            i.leftOptions.AddRange(i.name.Contains("l") ? database.pright : database.wright);
            i.rightOptions.AddRange(i.name.Contains("r") ? database.pleft : database.wleft);
            if (i.name.Length == 1)
            {
                string target = i.name;
                List<mazeTile> checkingList = target == "l" ? i.leftOptions : target == "r" ? i.rightOptions : target == "u" ? i.upOptions : target == "d" ? i.downOptions : null;
                checkingList.RemoveAll(x => x.name.Length == 1 && x.name == StaticTool.negate(target));
            }


            StaticTool.saveAsset(i.gameObject);
        }
    }
}
#endif