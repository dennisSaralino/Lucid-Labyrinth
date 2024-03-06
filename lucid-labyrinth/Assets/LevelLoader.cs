using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadBar;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync (int sceneIndex)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex);
        while (!loadOp.isDone)
        {
            loadBar.value = loadOp.progress;
            Debug.Log(loadOp.progress);
            yield return null;
        }
    }
}
