using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void Awake()
    {
        audioMixer = FindFirstObjectByType<AudioMixer>();
        audioMixer.SetFloat("volume", PlayerPrefs.GetFloat("volume"));
    }
    public void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Application closed.");
    }
}
