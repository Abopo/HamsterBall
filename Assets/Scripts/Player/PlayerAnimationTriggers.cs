using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;
    ThrowState _throwState;
    CatchState _bubbleState;
    AttackState _attackState;

	FMODUnity.StudioEventEmitter footstepEmitter;
	//FMODUnity.StudioEventEmitter netSwingEmitter;

	//public FMOD.Studio.EventInstance PlayerFootstepEvent
	public FMOD.Studio.EventInstance SwingNetEvent;
	public FMOD.Studio.EventInstance PlayerFootstepEvent;

	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
        _throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        _bubbleState = (CatchState)_playerController.GetPlayerState(PLAYER_STATE.CATCH);
        _attackState = (AttackState)_playerController.GetPlayerState(PLAYER_STATE.ATTACK);

		SwingNetEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SwingNetOneshot);
		PlayerFootstepEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PlayerFootstep);
    }

	public void OnEnable(){
		var target1 = this.gameObject;
		footstepEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter>();
		//netSwingEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter.>();
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(SwingNetEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerFootstepEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
	}

    // Update is called once per frame
    void Update () {
		SwingNetEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		PlayerFootstepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
	}

    public void ThrowBall() {
        _throwState.Throw();
		//SoundManager.mainAudio.ThrowAngleEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.ThrowEndOneShot);
    }

    public void NetSwingOn() {
        _bubbleState.Activate();
		SwingNetEvent.start();
		//netSwingEmitter.Play();
    }

    public void NetSwingOff() {
        _bubbleState.Deactivate();
    }

    public void NetSwingFinished() {
        _bubbleState.Finish();
    }

    public void ShowBubble() {
        _playerController.heldBall.DisplaySprites();
    }

    public void AttackOn() {
        _attackState.StartAttack();
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.PlayerAttack);
    }

    public void AttackOff() {
        _attackState.EndAttack();
    }

    public void AttackFinished() {
        _attackState.ExitAttack();
    }

    public void LongIdleEnd() {
        _playerController.Animator.SetBool("LongIdle", false);
        _playerController.ChangeState(PLAYER_STATE.IDLE);
    }
    public void Footstep() {
		PlayerFootstepEvent.start();
    }
}
