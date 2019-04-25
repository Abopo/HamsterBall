using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	//Public Static Access Script to Show where Everything needs to go
	public static SoundManager mainAudio;

	public string testSound = "event:/TestSound";

	//Hamster Sounds
	public string HamsterConnect;
	public string HamsterFillBall;
	public string HamsterCollectSuccessOneshot;
	public string BallBreak;
	public string HamsterTravel;
	public string HamsterOrbCreate;
	public string NewLine;
	public string WallBounce;

	//Generic Player Sounds
	public string FootstepOneshot;
	public string SwingNetOneshot;
	public string ThrowStartOneshot;
	//public string ThrowAngleLoop;
	public string ThrowEndOneShot;
	public string PlayerAttack;
	public string PlayerJump;

	//Music
	public string MusicMain;
	public FMOD.Studio.EventInstance MusicMainEvent;

	public string HappyDaysMusic;
	public FMOD.Studio.EventInstance HappyDaysMusicEvent;

	//public FMOD.Studio.EventInstance ThrowAngleEvent;
	public FMOD.Studio.EventInstance HamsterFillBallEvent;
	//Generic Player Sounds EventInstance

	void Awake () {
        if (mainAudio != null) {
            DestroyImmediate(this);
        } else {
            mainAudio = this;

            DontDestroyOnLoad(gameObject);

            HamsterFillBallEvent = FMODUnity.RuntimeManager.CreateInstance(HamsterFillBall);
            //ThrowAngleEvent = FMODUnity.RuntimeManager.CreateInstance(ThrowAngleLoop);
        }
	}

	void Footstep (){
		FMODUnity.RuntimeManager.PlayOneShot("event:/SingleFootstepEvent");
	}

	void Jump (){
		FMODUnity.RuntimeManager.PlayOneShot("event:/PlayerJump");
	}

	//SoundManager.mainAudio.Footstep()
	//FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.testSound);
}
