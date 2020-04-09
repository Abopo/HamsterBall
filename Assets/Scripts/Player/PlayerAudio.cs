using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
	public static PlayerAudio playerAudioScript;

    // Sound effects
    AudioClip _hitClip;

    AudioSource _audioSource;

	public FMOD.Studio.EventInstance PlayerJumpEvent;
	public FMOD.Studio.EventInstance PlayerLandEvent;
    public FMOD.Studio.EventInstance ShiftEvent;
	public FMOD.Studio.EventInstance ShiftMeterFilledEvent;
    public FMOD.Studio.EventInstance DamageSoundEvent;

    PlayerController _playerController;

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        _playerController = GetComponent<PlayerController>();

        PlayerJumpEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerJump);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerJumpEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        ShiftEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.Shift);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ShiftEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        PlayerLandEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerLand);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerLandEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        ShiftMeterFilledEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ShiftMeterFilled);

        DamageSoundEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerAttackConnect);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(DamageSoundEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        LoadSFX();
    }

    void LoadSFX() {
        _hitClip = Resources.Load<AudioClip>("Audio/SFX/Hit_Hurt");
    }

    void OnEnable(){
    }
    // Update is called once per frame
    void Update () {
	}

    public void PlayJumpClip() {
        PlayerJumpEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        PlayerJumpEvent.start();
    }

    public void PlayLandClip() {
        PlayerLandEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        PlayerLandEvent.setParameterValue("Surface", _playerController.platformIndex);
        PlayerLandEvent.start();
    }

    public void PlayShiftClip() {
		ShiftEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		ShiftEvent.start();
    }

    public void PlayShiftReadyClip() {
		ShiftMeterFilledEvent.start();
    }

    public void PlayHitClip() {
        DamageSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        DamageSoundEvent.start();
    }

    private void OnDestroy() {
        PlayerJumpEvent.release();
        PlayerLandEvent.release();
        ShiftEvent.release();
        ShiftMeterFilledEvent.release();
        DamageSoundEvent.release();
    }
}
