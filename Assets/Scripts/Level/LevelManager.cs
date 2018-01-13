using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public PauseMenu pauseMenu;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Pause")) {
            // Pause the game
            pauseMenu.Activate();
        }
    }

}
