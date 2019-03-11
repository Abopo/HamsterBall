using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;
    ThrowState _throwState;
    BubbleState _bubbleState;
    AttackState _attackState;

	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
        _throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        _bubbleState = (BubbleState)_playerController.GetPlayerState(PLAYER_STATE.BUBBLE);
        _attackState = (AttackState)_playerController.GetPlayerState(PLAYER_STATE.ATTACK);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ThrowBall() {
        _throwState.Throw();
    }

    public void NetSwingOn() {
        _bubbleState.Activate();
    }

    public void NetSwingOff() {
        _bubbleState.Deactivate();
    }

    public void NetSwingFinished() {
        _bubbleState.Finish();
    }

    public void ShowBubble() {
        _playerController.heldBubble.DisplaySprites();
    }

    public void AttackOn() {
        _attackState.StartAttack();
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
    	Debug.Log("Footstep");
    }
}
