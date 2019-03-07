using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	//Public Static Access Script to Show where Everything needs to go
	public static SoundManager mainAudio;

	public string testSound = "event:/TestSound";

	//Hamster Sounds

	//Generic Player Sounds
	public string footstepOneshot;
	public string swingNetOneshot;
	public string hamsterCollectSuccessOneshot;
	public string throwStartOneshot;
	public string throwAngleLoopEvent;


	void Awake () {
		if (mainAudio != null){
			Destroy(this);
		}
		mainAudio = this;
	}


}
