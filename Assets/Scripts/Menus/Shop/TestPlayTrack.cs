using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayTrack : MonoBehaviour {

    FMOD.Studio.EventInstance _musicTrack;

    MusicPage _musicPage;
    ShopItem _curItem;

    bool _isPlaying;

    private void Awake() {
        _musicPage = FindObjectOfType<MusicPage>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(InputState.GetButtonOnAnyControllerPressed("Shift")) {
            if (_isPlaying && _curItem == _musicPage.CurItem) {
                StopTrack();
            } else {
                StopTrack();
                PlayTrack();
            }
        }
    }

    public void PlayTrack() {
        _curItem = _musicPage.CurItem;

        if (_curItem.ItemInfo.itemName.Contains("Seren")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("Mount")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MountainMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("Conch")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BeachMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("City")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CityMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("Corporation")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CorpMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("Laboratory")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.LabMusic);
        } else if (_curItem.ItemInfo.itemName.Contains("Airship")) {
            _musicTrack = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
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
        _musicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _isPlaying = false;
    }
}
