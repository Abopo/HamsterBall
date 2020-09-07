using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MenuAudio : MonoBehaviour {

    bool firstPass = true;
    int sceneIndex = 0;

    static int musicPlaying = -1;

    void Awake() {
        SceneManager.activeSceneChanged += OnSceneLoaded;
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

    void OnSceneLoaded(Scene currentScene, Scene newScene) {
        sceneIndex = newScene.buildIndex;

        StartCoroutine(PlayMusicLater());
    }

    IEnumerator PlayMusicLater() {
        yield return null;

        PlayMusic();
    }

    void PlayMusic() {
        //if(firstPass) {
        //    sceneIndex = scene.buildIndex;
        //    firstPass = false;
        //} else {
            if(sceneIndex == 1) {
                // For some reason this is the only way to stop the menu music from playing
                SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
                SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

                // For some reason we have to create a new instance every time here
                SoundManager.mainAudio.VillageMusicEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.VillageMusic);
                SoundManager.mainAudio.VillageMusicEvent.start();
                SoundManager.mainAudio.ForestAmbienceEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestAmbience);
                SoundManager.mainAudio.ForestAmbienceEvent.start();

                musicPlaying = 0;
            } else if (sceneIndex < 15 && musicPlaying != 1) {
                // We're in a menu so play menu music
                Debug.Log("Play Menu Music");

                // For some reason this is the only way to stop the menu music from playing
                SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
                SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

                // For some reason we have to create a new instance every time here
		        SoundManager.mainAudio.MenuGeneralEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MenuGeneral);
                SoundManager.mainAudio.MenuGeneralEvent.start();
                SoundManager.mainAudio.Door();
                musicPlaying = 1;
            } else if (sceneIndex > 12) {
                // We're in a level so play level music
                Debug.Log("Stop menu music");
                SoundManager.mainAudio.MenuGeneralEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                musicPlaying = -1;
            }
        //}
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }
}
