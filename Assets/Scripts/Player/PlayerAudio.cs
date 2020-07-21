using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
	public static PlayerAudio playerAudioScript;

    // Sound effects
	public FMOD.Studio.EventInstance PlayerJumpEvent;
	public FMOD.Studio.EventInstance PlayerLandEvent;
    public FMOD.Studio.EventInstance ShiftEvent;
	public FMOD.Studio.EventInstance ShiftMeterFilledEvent;
    public FMOD.Studio.EventInstance DamageSoundEvent;
    public FMOD.Studio.EventInstance ThrowAngleLoopEvent;


    FMOD.Studio.PLAYBACK_STATE playbackState;

    PlayerController _playerController;

    // Use this for initialization
    void Start () {
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

        ThrowAngleLoopEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.ThrowAngleLoop);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ThrowAngleLoopEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
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

    public void PlayThrowAngleClip(float angle) {
        ThrowAngleLoopEvent.getPlaybackState(out playbackState);
        if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
            Debug.Log("Start throw clip");
            ThrowAngleLoopEvent.start();
        }

        ThrowAngleLoopEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
        ThrowAngleLoopEvent.setParameterValue("LaunchAngle", angle);
    }
    public void StopThrowAngleClip() {
        ThrowAngleLoopEvent.getPlaybackState(out playbackState);
        if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING) {
            Debug.Log("Stop throw clip");
            ThrowAngleLoopEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void OnDestroy() {
        PlayerJumpEvent.release();
        PlayerLandEvent.release();
        ShiftEvent.release();
        ShiftMeterFilledEvent.release();
        DamageSoundEvent.release();
        ThrowAngleLoopEvent.release();
    }
}
