using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinArea : MonoBehaviour
{
    public TMP_Text winText;
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.input.Disable();
            winText.gameObject.SetActive(true);
        }
    }
}
