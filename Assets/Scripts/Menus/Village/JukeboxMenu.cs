using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JukeboxMenu : Menu {
    public RectTransform content;

    public float _scrollTo;
    float _scrollSpeed = 1000f;

    float _itemYSpacing = 25f;

    protected GameObject _musicTrackObj;
    protected List<MusicTrackButton> _tracks = new List<MusicTrackButton>();

    ScrollRect _scrollRect;

    FMOD.Studio.EventInstance _playingTrack;

    PlayerController _villagePlayer;

    protected override void Awake() {
        base.Awake();

        if(_musicTrackObj == null) {
            _musicTrackObj = Resources.Load("Prefabs/Village/Music Track") as GameObject;
        }

        _scrollRect = GetComponentInChildren<ScrollRect>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        CreateMusicTracks();

        // Highlight the first item
        _tracks[0].isFirstSelection = true;

        // Properly size the content
        if (_tracks.Count > 6) {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 152 + (_tracks.Count - 6) * _itemYSpacing);
        }

    }

    void CreateMusicTracks() {
        GameObject tempOption;

        // Run through all the music tracks and if they're unlocked make a track option
        if(ES3.Load<bool>("Seren Woods 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Seren Woods 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Seren Woods 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Seren Woods 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Mount Bolor 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Mount Bolor 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Mount Bolor 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Mount Bolor 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Conch Cove 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Conch Cove 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Conch Cove 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Conch Cove 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Big City 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Big City 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Big City 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Big City 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Corporation 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Corporation 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Corporation 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Corporation 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Laboratoy 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Laboratory 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Laboratoy 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Laboratory 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Airship 1 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Airship 1");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }
        if (ES3.Load<bool>("Airship 2 Track", false)) {
            tempOption = Instantiate(_musicTrackObj, content) as GameObject;
            tempOption.GetComponent<MusicTrackButton>().SetTrack("Airship 2");
            _tracks.Add(tempOption.GetComponent<MusicTrackButton>());
        }

        // Position the items properly
        for(int i = 0; i < _tracks.Count; ++i) {
            ((RectTransform)_tracks[i].transform).anchoredPosition = new Vector3(0, 113f - (_itemYSpacing * i));
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        // Smooth scroll?
        if (content.anchoredPosition.y != _scrollTo) {
            // Scroll towards the position
            if (content.anchoredPosition.y > _scrollTo) {
                // Scroll down
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y < _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            } else if (content.anchoredPosition.y < _scrollTo) {
                // Scroll up
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y + (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y > _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            }
        }
    }

    protected override void CheckInput() {
        base.CheckInput();

        if(InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Deactivate();
        }
    }

    public override void Activate() {
        base.Activate();

        _villagePlayer = FindObjectOfType<PlayerController>();

        // Take control away form player
        _villagePlayer.hasControl = false;
    }

    public override void Deactivate() {
        base.Deactivate();

        // Give player back control
        _villagePlayer.hasControl = true;
    }

    public void ScrollUp() {
        _scrollTo -= _itemYSpacing;
        if(_scrollTo < 0) {
            _scrollTo = 0;
        }
    }
    public void ScrollDown() {
        _scrollTo += _itemYSpacing;
    }

    public void PlayTrack(FMOD.Studio.EventInstance track) {
        SoundManager.mainAudio.VillageMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _playingTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _playingTrack = track;
        _playingTrack.start();
    }
}
