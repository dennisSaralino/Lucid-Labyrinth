using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Slider loadBar;
    public GameObject lucidityUI;
    public Camera mainCam;
    public BreadcrumbSpawner bspawner;
    public MazeController maze;
    public AudioListener backupAudio;   // not important, just a workaround
    private float time = 0.0f;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        while (loadBar.value < 250.0f)
        {
            if (maze.isReady) break;
            loadBar.value += time;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (!maze.isReady)
            yield return null;
        loadBar.value = 250;
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (lucidityUI != null)
            lucidityUI.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(true);
        bspawner.gameObject.SetActive(true);
        backupAudio.gameObject.SetActive(false);
    }


}
