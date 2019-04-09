using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
    // Sound effects
    AudioClip _jumpClip;
    AudioClip _bubbleClip;
    AudioClip _shiftClip;
    AudioClip _shiftReadyClip;
    AudioClip _throwClip;
    AudioClip _attackClip;
    AudioClip _hitClip;

    AudioSource _audioSource;

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        LoadSFX ();
    }

    void LoadSFX() {
        _jumpClip = Resources.Load<AudioClip>("Audio/SFX/Jump");
        _bubbleClip = Resources.Load<AudioClip>("Audio/SFX/Bubble");
        _shiftClip = Resources.Load<AudioClip>("Audio/SFX/Shift");
        _shiftReadyClip = Resources.Load<AudioClip>("Audio/SFX/ShiftReady");
        _throwClip = Resources.Load<AudioClip>("Audio/SFX/Throw");
        _attackClip = Resources.Load<AudioClip>("Audio/SFX/Punch");
        _hitClip = Resources.Load<AudioClip>("Audio/SFX/Hit_Hurt");
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void PlayJumpClip() {
        _audioSource.clip = _jumpClip;
        _audioSource.Play();
		FMODUnity.RuntimeManager.PlayOneShot("event:/PlayerJump");
    }

    public void PlayBubbleClip() {
        //_audioSource.clip = _bubbleClip;
        //_audioSource.Play();
    }

    public void PlayShiftClip() {
        _audioSource.clip = _shiftClip;
        _audioSource.Play();
    }

    public void PlayShiftReadyClip() {
        _audioSource.clip = _shiftReadyClip;
        _audioSource.Play();
    }

    public void PlayThrowClip() {
        _audioSource.clip = _throwClip;
        _audioSource.Play();
    }

    public void PlayAttackClip() {
        _audioSource.clip = _attackClip;
        _audioSource.Play();
    }

    public void PlayHitClip() {
        _audioSource.clip = _hitClip;
        _audioSource.Play();
    }
}
