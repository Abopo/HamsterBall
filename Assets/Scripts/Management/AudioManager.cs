using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    List<AudioClip> _backgroundMusic = new List<AudioClip>();
    AudioSource _audioSource;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();

        LoadBGM();

        SceneManager.sceneLoaded += PlayMusic;
    }

    // Use this for initialization
    void Start () {
	}
	
    void LoadBGM() {
        AudioClip clip1, clip2;
        //clip1 = Resources.Load<AudioClip>("Audio/BGM/Puzzle Bobble - Theme Remix by SuperNormanBross");
        //clip2 = Resources.Load<AudioClip>("Audio/BGM/bubble-bobble-06-ingame-music-hurry-up-");
        clip1 = Resources.Load<AudioClip>("Audio/BGM/Happy Days");
        clip2 = Resources.Load<AudioClip>("Audio/BGM/Silly 01");
        _backgroundMusic.Add(clip1);
        _backgroundMusic.Add(clip2);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void PlayMusic(Scene scene, LoadSceneMode mode) {
        if(scene.buildIndex < 13 && _audioSource != null && _audioSource.clip != _backgroundMusic[0]) {
            // We're in a menu so play menu music
            _audioSource.clip = _backgroundMusic[0];
            _audioSource.volume = 1f;
            _audioSource.Play();
        } else if (scene.buildIndex > 12 && _audioSource != null) {
            // We're in a level so play level music
            _audioSource.clip = _backgroundMusic[1];
            _audioSource.volume = 0.25f;
            _audioSource.Play();
        }
    }
}
