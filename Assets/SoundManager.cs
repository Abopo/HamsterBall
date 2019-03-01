using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FMODUnity.RuntimeManager.PlayOneShot("event:/TestSound");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
