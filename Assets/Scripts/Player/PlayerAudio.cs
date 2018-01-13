using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
    // Sound effects
    AudioClip _jumpClip;
    AudioClip _bubbleClip;
    AudioClip _switchClip;
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
        _switchClip = Resources.Load<AudioClip>("Audio/SFX/Teleport");
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
    }

    public void PlayBubbleClip() {
        _audioSource.clip = _bubbleClip;
        _audioSource.Play();
    }

    public void PlaySwitchClip() {
        _audioSource.clip = _switchClip;
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
