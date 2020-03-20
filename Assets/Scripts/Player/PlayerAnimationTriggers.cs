using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;
    ThrowState _throwState;
    CatchState _catchState;
    AttackState _attackState;

    PlayerEffects _playerEffects;

	FMODUnity.StudioEventEmitter footstepEmitter;
	//FMODUnity.StudioEventEmitter netSwingEmitter;

	//public FMOD.Studio.EventInstance PlayerFootstepEvent
	public FMOD.Studio.EventInstance SwingNetEvent;

	public FMOD.Studio.ParameterInstance ParameterInstance;
	public int test;

	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
        _throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        _catchState = (CatchState)_playerController.GetPlayerState(PLAYER_STATE.CATCH);
        _attackState = (AttackState)_playerController.GetPlayerState(PLAYER_STATE.ATTACK);

        _playerEffects = transform.parent.GetComponentInChildren<PlayerEffects>();
               		
        //FMOD.Studio.EventInstance.getParameterValue("Surface", out _playerController.platformIndex, out test);

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SwingNetEvent, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
    }

    public void OnEnable(){
		var target1 = this.gameObject;
		footstepEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter>();
		//netSwingEmitter = target1.GetComponent<FMODUnity.StudioEventEmitter.>();
	}

    // Update is called once per frame
    void Update () {
	}

    public void ThrowBall() {
        _throwState.Throw();
		//SoundManager.mainAudio.ThrowAngleEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.ThrowEndOneShot);
    }

    public void NetSwingOn() {
        _catchState.Activate();

        // Play swing sound
        SwingNetEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
        SwingNetEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SwingNetOneshot);
		SwingNetEvent.start();
		SwingNetEvent.release();
		//netSwingEmitter.Play();
    }

    public void NetSwingOff() {
        _catchState.Deactivate();
    }

    public void NetSwingFinished() {
        _catchState.Finish();
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

        /*
		if(_playerController.platformIndex == 0){
			GrassFootstepEvent.start();
			Debug.Log("Grass");
		} else if (_playerController.platformIndex == 1){
			WoodFootstepEvent.start();
			Debug.Log("Wood");
		}
		Debug.Log(_playerController.platformIndex);
        */
        _playerEffects.PlayFootstep();
    }

    public void WinLoopStart() {
        _playerController.Animator.SetBool("WinLoop", true);
    }
}
