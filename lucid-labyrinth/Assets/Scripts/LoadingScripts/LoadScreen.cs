using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Slider loadBar;
    public Canvas lucidityUI;
    public Camera mainCam;
    public BreadcrumbSpawner bspawner;
    public MazeController maze;
    public AudioListener backupAudio;   // not important, just a workaround
    private float time = 0.0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator Load()
    {
        while (loadBar.value < 250.0f)
        {
            loadBar.value += (time / Mathf.Max(250.0f)) / 100;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        StartCoroutine(Load());

        if (loadBar.value >= 250.0f && maze.isReady)
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
