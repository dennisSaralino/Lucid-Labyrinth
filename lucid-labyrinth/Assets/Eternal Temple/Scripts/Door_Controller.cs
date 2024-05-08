﻿using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace commanastationwww.eternaltemple
{
public class Door_Controller : MonoBehaviour {
		
	public bool stayOpen = true; //If set to true, door will open once and stay like this until Close() is called. If set to false, door will close after player leaves the trigger area
	public bool locked = false; //If set to true, door will not open when player enters trigger area
	public float openingSpeed = 5.0f;
	
	private Transform[] allTransform; //Array for Transform components of this object and it's children	
	private Transform[] childrenTransform; //for children's Transform components only

	private AudioSource audioSource;
	public AudioClip	audioClip;
	public EnvironmentController env;
	
// Use this for initialization
	void Start ()
	{		
		//Getting transform components of this object and all it's children
		env = GameObject.FindGameObjectWithTag("EnvironmentController").GetComponent<EnvironmentController>();
		allTransform = GetComponentsInChildren<Transform>();
		stayOpen = false;
		//Create new Array for children's Transforms only
		childrenTransform = new Transform[allTransform.Length - 1];
		
		//Copy all components of allTransform except 0 in childrenTransform array, ie remove Transform component of this object and keep it's children's only.
			for (int i=1; i < allTransform.Length; i++)
				{
					childrenTransform[i-1]=allTransform[i];		
				}
		
		audioSource = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (stayOpen == false)
		{
			if (other.gameObject.CompareTag("Key"))
			{
				//if (audioClip != null)
				//	audioSource.PlayOneShot(audioClip);
				stayOpen = true;
				Destroy(other.gameObject);
				Open();
				env.DespawnMonster();
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (stayOpen == false)
			{
				Close();
			}
			
	}

	//Couroutine to move door down
	IEnumerator openInterpolation()
	{	
		

		while (childrenTransform[0].localPosition.y > (-2.6f))
			{				
				foreach (Transform childTransform in childrenTransform)
					{
                    if (audioClip != null)
                        audioSource.PlayOneShot(audioClip);
                    childTransform.Translate(Vector3.down * openingSpeed * Time.deltaTime);
						yield return null;						
					}
			}			
	}
	//Couroutine to move door up
	IEnumerator closeInterpolation()
	{
		while (childrenTransform[0].localPosition.y < 0f)
			{
				foreach (Transform childTransform in childrenTransform)
					{
						childTransform.Translate(Vector3.up * openingSpeed * Time.deltaTime);
						yield return null;
					}
			}
	}

	public void Open()
	{
		if (locked == false)
		{
			StopAllCoroutines();
			StartCoroutine(openInterpolation());
		}
	}
	
	public void Close()
	{
		StopAllCoroutines();
		StartCoroutine(closeInterpolation());
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
			if (collision.gameObject.tag == "Player")
			{
			}
    }



    }
}
