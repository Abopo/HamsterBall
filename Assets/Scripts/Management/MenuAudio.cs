using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuAudio : MonoBehaviour {

    bool firstPass = true;
    int sceneIndex = 0;

    static int musicPlaying = -1;

    void Awake() {
        SceneManager.sceneLoaded += PlayMusic;
    }

    // Use this for initialization
    void Start () {
        LoadBGM();

        if (sceneIndex < 2) {
			SoundManager.mainAudio.VillageMusicEvent.start();
        }
    }

    void LoadBGM() {
        Debug.Log("Load Menu Music");
		SoundManager.mainAudio.VillageMusicEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.VillageMusic);
		SoundManager.mainAudio.MenuGeneralEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MenuGeneral);
    }

    // Update is called once per frame
    void Update () {
	    if(Input.GetKeyDown(KeyCode.M)) {
            Debug.Log("Force stop music");
            SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
            SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    // For some reason this code is extremely inconsistent. Some scenes will stop and play music properly,
    // while others will not stop music, not play music, or some combination of both.
    // I tried my best to force the proper functionality

    void PlayMusic(Scene scene, LoadSceneMode mode) {
        if(firstPass) {
            sceneIndex = scene.buildIndex;
            firstPass = false;
        } else {
            sceneIndex = scene.buildIndex;
            if(sceneIndex == 1 && musicPlaying != 0) {
                // For some reason this is the only way to stop the menu music from playing
                SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
                SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

                // This did not work
                //SoundManager.mainAudio.MenuGeneralEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                // For some reason we have to create a new instance every time here
                SoundManager.mainAudio.VillageMusicEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.VillageMusic);
                SoundManager.mainAudio.VillageMusicEvent.start();

                musicPlaying = 0;
            } else if (sceneIndex < 15 && musicPlaying != 1) {
                // We're in a menu so play menu music
                Debug.Log("Play Menu Music");

                // For some reason this is the only way to stop the menu music from playing
                SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
                SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

                //SoundManager.mainAudio.HappyDaysMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                // For some reason we have to create a new instance every time here
		        SoundManager.mainAudio.MenuGeneralEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MenuGeneral);
                SoundManager.mainAudio.MenuGeneralEvent.start();

                musicPlaying = 1;
            } else if (sceneIndex > 12) {
                // We're in a level so play level music
                Debug.Log("Stop menu music");
                SoundManager.mainAudio.MenuGeneralEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                musicPlaying = -1;
            }
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= PlayMusic;
    }
}
