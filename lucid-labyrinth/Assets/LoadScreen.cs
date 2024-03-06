using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Slider loadBar;

    IEnumerator Load()
    {
        float time = 0.0f;
        while (time < 100.0f)
        {
            loadBar.value += (time / Mathf.Max(100.0f)) / 5;
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
}
