using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrackButton : MenuButton {

    string _trackName;
    public SuperTextMesh text;

    FMOD.Studio.EventInstance _trackInstance;

    public float worldY;

    JukeboxMenu _jukebox;

    protected override void Awake() {
        base.Awake();

        _jukebox = FindObjectOfType<JukeboxMenu>();

        if (text == null) {
            text = GetComponentInChildren<SuperTextMesh>(true);
        }
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

    }

    public void SetTrack(string track) {
        _trackName = track;
        if (text != null) {
            text.text = _trackName;
        }

        if(_trackName.Contains("Seren")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
        } else if(_trackName.Contains("Mount")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MountainMusic);
        } else if (_trackName.Contains("Conch")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BeachMusic);
        } else if (_trackName.Contains("City")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CityMusic);
        } else if (_trackName.Contains("Corporation")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.CorpMusic);
        } else if (_trackName.Contains("Laboratory")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.LabMusic);
        } else if (_trackName.Contains("Airship")) {
            _trackInstance = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ForestMusic);
        }

        if (_trackName.Contains("1")) {
            _trackInstance.setParameterValue("RowDanger", 1f);
        } else if (_trackName.Contains("2")) {
            _trackInstance.setParameterValue("RowDanger", 2f);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        worldY = transform.position.y;
    }

    public override void Highlight() {
        base.Highlight();

        // If we're out of the viewport
        if(worldY < _jukebox.transform.position.y - 2.75f) {
            _jukebox.ScrollDown();
        } else if(worldY > _jukebox.transform.position.y + 1.5f) {
            _jukebox.ScrollUp();
        }
    }

    protected override void Select() {
        base.Select();

        _jukebox.PlayTrack(_trackInstance);
        _jukebox.Deactivate();
    }
}
