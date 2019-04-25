using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    bool firstPass = true;
    int sceneIndex = 0;

    static bool alreadyPlaying = false;

    void Awake() {
        SceneManager.sceneLoaded += PlayMusic;
    }

    // Use this for initialization
    void Start () {
        LoadBGM();

        if (sceneIndex < 15) {
            SoundManager.mainAudio.HappyDaysMusicEvent.start();
            alreadyPlaying = true;
        }
    }

    void LoadBGM() {
        Debug.Log("Load Menu Music");
		SoundManager.mainAudio.HappyDaysMusicEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.HappyDaysMusic);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void PlayMusic(Scene scene, LoadSceneMode mode) {
        if(firstPass) {
            sceneIndex = scene.buildIndex;
            firstPass = false;
        } else {
            sceneIndex = scene.buildIndex;
            if (sceneIndex < 15 && !alreadyPlaying) {
                // We're in a menu so play menu music
                Debug.Log("Play Menu Music");
                SoundManager.mainAudio.HappyDaysMusicEvent.start();
                alreadyPlaying = true;
            } else if (sceneIndex > 12) {
                // We're in a level so play level music
                Debug.Log("Stop menu music");
                SoundManager.mainAudio.HappyDaysMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                alreadyPlaying = false;
            }
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= PlayMusic;
    }
}
