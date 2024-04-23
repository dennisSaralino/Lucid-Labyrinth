using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEngine;
using System;

public static class StaticTool
{

    
#if UNITY_EDITOR
    public static void saveAsset(UnityEngine.Object j)
    {
        EditorUtility.SetDirty(j);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
    //does not include Assets/
    public static T[] loadAllAsset<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

        foreach (string fileName in fileEntries)
        {
            string temp = fileName.Replace("\\", "/");
            int index = temp.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += temp.Substring(index);

            UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }

        T[] result = new T[al.Count];

        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

#endif


    public static void printReport(string content, string name)
    {
        string report = "=========REPORT: " + name  + "=========\n";
        report += content;
        report += "=========END=========";
        Debug.Log(report);
    }
    public static bool checkInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    public static void destroyChildOb(Transform parent)
    {
        foreach (Transform i in parent)
        {
            GameObject.Destroy(i.gameObject);
        }
    }

    public static void UnsubscribeAll(ref Action eventToRemove)
    {
        if (eventToRemove != null)
        {
            foreach (Delegate d in eventToRemove.GetInvocationList())
            {
                eventToRemove -= (Action)d;
            }
        }
    }

    public static void foreachChild(Transform parent, System.Action<Transform> toDo, Predicate<Transform> predicate = null)
    {
        if (predicate != null? predicate(parent):true) toDo(parent);
        foreach (Transform i in parent)
        {
            foreachChild(i, toDo, predicate);
        }
    }
    public static bool inGrid(int x, int y, int width, int height)
    {
        bool checkedgeX = (x < width) && (x >= 0);
        bool checkedgeY = (y < height) && (y >= 0);
        return checkedgeX && checkedgeY;
    }
    public static string negate(string i)
    {
        return i == "l" ? "r" : i == "r" ? "l" : i == "u" ? "d" : i == "d" ? "u" : "";
    }
}