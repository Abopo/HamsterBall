using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
	public static PlayerAudio playerAudioScript;

    // Sound effects
    AudioClip _shiftReadyClip;
    AudioClip _hitClip;

    AudioSource _audioSource;

	public FMOD.Studio.EventInstance PlayerJumpEvent;
	public FMOD.Studio.EventInstance ShiftEvent;
	public FMOD.Studio.EventInstance PlayerLandEvent;


    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        LoadSFX ();
		//PlayerJumpEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerJump);
		//PlayerLandEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerLand);
    }

    void LoadSFX() {
        _shiftReadyClip = Resources.Load<AudioClip>("Audio/SFX/ShiftReady");
        _hitClip = Resources.Load<AudioClip>("Audio/SFX/Hit_Hurt");
    }

    void OnEnable(){
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerJumpEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(ShiftEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerLandEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    // Update is called once per frame
    void Update () {
		PlayerJumpEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		ShiftEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		PlayerLandEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
	}

    public void PlayJumpClip() {
		PlayerJumpEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerJump);
		PlayerJumpEvent.start();
		PlayerJumpEvent.release();
    }


    public void PlayShiftClip() {
		ShiftEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.Shift);
		ShiftEvent.start();
		ShiftEvent.release();
    }

    public void PlayShiftReadyClip() {
        _audioSource.clip = _shiftReadyClip;
        _audioSource.Play();
    }

    public void PlayHitClip() {
        _audioSource.clip = _hitClip;
        _audioSource.Play();
    }
}
