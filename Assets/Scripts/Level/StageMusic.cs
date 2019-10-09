using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusic : MonoBehaviour {

    FMOD.Studio.EventInstance _stageMusic;
    FMOD.Studio.EventInstance _stageAmbience;

    DividerFlash[] _dividers;
    bool _dangerTime;

    GameManager _gameManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();

    }
    // Start is called before the first frame update
    void Start() {
        _dividers = FindObjectsOfType<DividerFlash>();

        _dangerTime = false;

        _gameManager.gameOverEvent.AddListener(GameEnd);

        SetStageMusic();
    }

    void SetStageMusic() {
        switch (_gameManager.selectedBoard) {
            case BOARDS.FOREST:
                _stageMusic = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
                _stageAmbience = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestAmbience);
                _stageMusic.setParameterValue("RowDanger", 1f);
                // Stop menu music
                SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _stageMusic.start();
                _stageAmbience.start();

                break;
            case BOARDS.MOUNTAIN:
                _stageMusic = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MountainMusic);
                _stageAmbience = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SnowAmbience);
                _stageMusic.setParameterValue("RowDanger", 1f);
                // Stop menu music
                SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _stageMusic.start();
                _stageAmbience.start();

                break;
            case BOARDS.BEACH:
                _stageMusic = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BeachMusic);
                _stageAmbience = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BeachAmbience);
                _stageMusic.setParameterValue("RowDanger", 1f);
                // Stop menu music
                SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _stageMusic.start();
                _stageAmbience.start();

                break;
            case BOARDS.CITY:
                break;
            case BOARDS.CORPORATION:
                break;
            case BOARDS.LABORATORY:
                break;
            case BOARDS.AIRSHIP:
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        // If we're not in danger time
        if(!_dangerTime) {
            // Check to see if we need to enter danger time
            if(InDanger()) {
                // Go into danger time
                _dangerTime = true;
                _stageMusic.setParameterValue("RowDanger", 2f);
            }
        // If we are in danger time
        } else {
            // Check to see if we can leave it
            if(!InDanger()) {
                _dangerTime = false;
                _stageMusic.setParameterValue("RowDanger", 1f);
            }
        }
    }

    bool InDanger() {
        foreach (DividerFlash dF in _dividers) {
            if (dF.isFlashing) {
                // Still in danger time
                return true;
            }
        }

        // Not in danger time
        return false;
    }

    void GameEnd() {
        // Stop the music
        _stageMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
