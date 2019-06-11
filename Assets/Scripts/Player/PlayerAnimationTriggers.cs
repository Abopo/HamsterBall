using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;
    ThrowState _throwState;
    CatchState _bubbleState;
    AttackState _attackState;

    PlayerEffects _playerEffects;

	FMODUnity.StudioEventEmitter footstepEmitter;
	//FMODUnity.StudioEventEmitter netSwingEmitter;

	//public FMOD.Studio.EventInstance PlayerFootstepEvent
	public FMOD.Studio.EventInstance SwingNetEvent;

	public FMOD.Studio.EventInstance GrassFootstepEvent;
	public FMOD.Studio.EventInstance WoodFootstepEvent;

	public FMOD.Studio.ParameterInstance ParameterInstance;
	public int test;

	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
        _throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        _bubbleState = (CatchState)_playerController.GetPlayerState(PLAYER_STATE.CATCH);
        _attackState = (AttackState)_playerController.GetPlayerState(PLAYER_STATE.ATTACK);

        _playerEffects = transform.parent.GetComponentInChildren<PlayerEffects>();



		SwingNetEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SwingNetOneshot);
		GrassFootstepEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.GrassPlayerFootstep);
		WoodFootstepEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.WoodPlayerFootstep);
		//FMOD.Studio.EventInstance.getParameterValue("Surface", out _playerController.platformIndex, out test);
    }

	public void OnEnable(){
		var target1 = this.gameObject;
		footstepEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter>();
		//netSwingEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter.>();
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(SwingNetEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(GrassFootstepEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(WoodFootstepEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
	}

    // Update is called once per frame
    void Update () {
		SwingNetEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		GrassFootstepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
		WoodFootstepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, GetComponent<Rigidbody>()));
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
		//PlayerFootstepEvent.getParameter("Surface", PlayerFootstepEvent);

		if(_playerController.platformIndex == 0){
			GrassFootstepEvent.start();
			Debug.Log("Grass");
		} else if (_playerController.platformIndex == 1){
			WoodFootstepEvent.start();
			Debug.Log("Wood");
		}
		Debug.Log(_playerController.platformIndex);
        _playerEffects.PlayFootstep();
    }
}
