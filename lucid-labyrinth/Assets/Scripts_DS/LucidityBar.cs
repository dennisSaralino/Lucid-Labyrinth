using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LucidityBar : MonoBehaviour
{

    public Slider slider;
    
    public void SetStartingLucidity(int health)
    {
        slider.maxValue = health;
        slider.value = health / 2;
    }

    private void Awake()
    {
        SetStartingLucidity(100);
        //StartCoroutine(TimeDecrease());
    }

    /*
    public IEnumerator TimeDecrease()
    {
        while (slider.value > 0)
        {
            yield return new WaitForSeconds(1f);
            slider.value -= 1f;
        }
    }
    */
    private void FixedUpdate()
    {
        slider.value -= Time.deltaTime * 2;
        if (slider.value <= 0)
            Die();
    }
 
    private void Die()
    {
        Debug.Log("You died");
    }
}


