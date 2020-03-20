using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
	public static PlayerAudio playerAudioScript;

    // Sound effects
    AudioClip _hitClip;

    AudioSource _audioSource;

	public FMOD.Studio.EventInstance PlayerJumpEvent;
	public FMOD.Studio.EventInstance ShiftEvent;
	public FMOD.Studio.EventInstance ShiftMeterFilledEvent;
	public FMOD.Studio.EventInstance PlayerLandEvent;

    PlayerController _playerController;

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        _playerController = GetComponent<PlayerController>();

        LoadSFX ();
		//PlayerJumpEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerJump);
		//PlayerLandEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerLand);
    }

    void LoadSFX() {
        _hitClip = Resources.Load<AudioClip>("Audio/SFX/Hit_Hurt");
    }

    void OnEnable(){
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerJumpEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(ShiftEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		//FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerLandEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    // Update is called once per frame
    void Update () {
	}

    public void PlayJumpClip() {
		PlayerJumpEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        PlayerJumpEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerJump);
		PlayerJumpEvent.start();
		PlayerJumpEvent.release();
    }

    public void PlayLandClip() {
        PlayerLandEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        PlayerLandEvent.setParameterValue("Surface", _playerController.platformIndex);
        PlayerLandEvent.start();
    }

    public void PlayShiftClip() {
		ShiftEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        ShiftEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.Shift);
		ShiftEvent.start();
		ShiftEvent.release();
    }

    public void PlayShiftReadyClip() {
		ShiftMeterFilledEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ShiftMeterFilled);
		ShiftMeterFilledEvent.start();
		ShiftMeterFilledEvent.release();
    }

    public void PlayHitClip() {
        _audioSource.clip = _hitClip;
        _audioSource.Play();
    }
}
