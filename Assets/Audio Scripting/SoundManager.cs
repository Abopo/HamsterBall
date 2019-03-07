using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	//Public Static Access Script to Show where Everything needs to go
	public static SoundManager mainAudio;

	public string testSound = "event:/TestSound";

	//Hamster Sounds

	//Generic Player Sounds
	public string FootstepOneshot;
	public string SwingNetOneshot;
	public string HamsterCollectSuccessOneshot;
	public string ThrowStartOneshot;
	public string ThrowAngleLoop;
	public string ThrowEndOneShot;

	public FMOD.Studio.EventInstance ThrowAngleEvent;

	//Generic Player Sounds EventInstance

	void Awake () {
		if (mainAudio != null){
			Destroy(this);
		}
		mainAudio = this;

		ThrowAngleEvent = FMODUnity.RuntimeManager.CreateInstance(ThrowAngleLoop);
	}

	//FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.testSound);
}
