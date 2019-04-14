using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;
    ThrowState _throwState;
    BubbleState _bubbleState;
    AttackState _attackState;

	public FMOD.Studio.EventInstance PlayerFootstepEvent;
	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
        _throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        _bubbleState = (BubbleState)_playerController.GetPlayerState(PLAYER_STATE.BUBBLE);
        _attackState = (AttackState)_playerController.GetPlayerState(PLAYER_STATE.ATTACK);

		PlayerFootstepEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.FootstepOneshot);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayerFootstepEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ThrowBall() {
        _throwState.Throw();
		SoundManager.mainAudio.ThrowAngleEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.ThrowEndOneShot);
    }

    public void NetSwingOn() {
        _bubbleState.Activate();
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SwingNetOneshot);
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
