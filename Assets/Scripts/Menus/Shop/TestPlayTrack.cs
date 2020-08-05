using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayTrack : MonoBehaviour {

    FMOD.Studio.EventInstance _musicTrack;

    MusicPage _musicPage;
    ShopItem _curItem;

    bool _isPlaying;

    FMOD.Studio.EventInstance[] _allTracks = new FMOD.Studio.EventInstance[7];

    private void Awake() {
        _musicPage = FindObjectOfType<MusicPage>();
    }
    // Start is called before the first frame update
    void Start() {
        LoadTracks();
    }

    void LoadTracks() {
        _allTracks[0] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
        _allTracks[1] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MountainMusic);
        _allTracks[2] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BeachMusic);
        _allTracks[3] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CityMusic);
        _allTracks[4] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CorpMusic);
        _allTracks[5] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.LabMusic);
        _allTracks[6] = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
    }

    // Update is called once per frame
    void Update() {
        if(InputState.GetButtonOnAnyControllerPressed("Extra")) {
            if (_isPlaying && _curItem == _musicPage.CurItem) {
                StopTrack();
            } else {
                StopTrack();
                PlayTrack();
            }
        }
    }

    public void PlayTrack() {
        Debug.Log("Play test track");
        // Pause the menu music
        SoundManager.mainAudio.MenuGeneralEvent.setPaused(true);

        _curItem = _musicPage.CurItem;

        if (_curItem.ItemInfo.itemName.Contains("Seren")) {
            _musicTrack = _allTracks[0];
        } else if (_curItem.ItemInfo.itemName.Contains("Mount")) {
            _musicTrack = _allTracks[1];
        } else if (_curItem.ItemInfo.itemName.Contains("Conch")) {
            _musicTrack = _allTracks[2];
        } else if (_curItem.ItemInfo.itemName.Contains("City")) {
            _musicTrack = _allTracks[3];
        } else if (_curItem.ItemInfo.itemName.Contains("Corporation")) {
            _musicTrack = _allTracks[4];
        } else if (_curItem.ItemInfo.itemName.Contains("Laboratory")) {
            _musicTrack = _allTracks[5];
        } else if (_curItem.ItemInfo.itemName.Contains("Airship")) {
            _musicTrack = _allTracks[6];
        }

        if (_curItem.ItemInfo.itemName.Contains("1")) {
            _musicTrack.setParameterValue("RowDanger", 1f);
        } else if (_curItem.ItemInfo.itemName.Contains("2")) {
            _musicTrack.setParameterValue("RowDanger", 2f);
        }

        _musicTrack.start();

        _isPlaying = true;
    }

    public void StopTrack() {
        Debug.Log("Stop test track");
        _musicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _isPlaying = false;

        // Resume the menu music
        SoundManager.mainAudio.MenuGeneralEvent.setPaused(false);
    }
}
