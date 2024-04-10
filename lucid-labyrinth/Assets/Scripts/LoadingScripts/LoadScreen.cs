using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Slider loadBar;
    public Canvas lucidityUI;
    public Camera mainCam;
    public BreadcrumbSpawner bspawner;
    public float time = 0.0f;

    IEnumerator Load()
    {
        while (time < 100.0f)
        {
            loadBar.value += (time / Mathf.Max(100.0f)) / 10;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        StartCoroutine(Load());

        if (loadBar.value == 100.0f)
            this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        lucidityUI.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(true);
        bspawner.gameObject.SetActive(true);
    }
}
