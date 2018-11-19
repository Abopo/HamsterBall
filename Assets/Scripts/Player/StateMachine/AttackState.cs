using UnityEngine;
using System.Collections;

public class AttackState : PlayerState {
    float sign;

    bool _isAttacking = true;
    float _attackSpeed = 8f;
    float _attackTime = 0.2f;
    float _attackTimer = 0;

    float _cooldownTime = 0.35f;
    float _cooldownTimer = 0.0f;

    // Use this for initialization
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        playerController.PlayerAudio.PlayAttackClip();

        if(playerController.inputState.left.isDown) {
            _direction = -1;
            if(playerController.Animator.GetBool("FacingRight")) {
                playerController.Flip();
            }
        } else if(playerController.inputState.right.isDown) {
            _direction = 1;
            if (!playerController.Animator.GetBool("FacingRight")) {
                playerController.Flip();
            }
        } else {
            _direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
        }

        _isAttacking = true;
        _attackTimer = 0f;
        _cooldownTimer = 0f;

        // Activate attack animation thingy
        playerController.attackObj.SetActive(true);
        playerController.velocity.x = _attackSpeed * _direction;
        playerController.velocity.y = 0f;
        playerController.attackCooldownTimer = 0;
    }

    // Update is called once per frame
    public override void Update() {
        //if (!playerController.attackObj.IsAttacking) {
        //    playerController.ChangeState(PLAYER_STATE.IDLE);
        //}

        if (_isAttacking) {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackTime) {
                _isAttacking = false;
            }
        } else {
            if (Mathf.Abs(playerController.velocity.x) > 0.5f) {
                sign = Mathf.Sign(playerController.velocity.x);
                playerController.velocity.x -= sign * 50 * playerController.Traction * Time.deltaTime;
                if (sign > 0 && playerController.velocity.x < 0 ||
                    sign < 0 && playerController.velocity.x > 0) {
                    playerController.velocity.x = 0;
                }
            } else {
                playerController.velocity.x = 0;
            }

            if (Mathf.Abs(playerController.velocity.x) < 10f) {
                // Fall
                playerController.ApplyGravity();
            }

            _cooldownTimer += Time.deltaTime;
            if(_cooldownTimer >= _cooldownTime) {
                ExitAttack();
            }
        }
    }

    public override void CheckInput(InputState inputState) {
    }

    public void StartAttack() {
        playerController.attackObj.SetActive(true);
    }

    public void EndAttack() {
        playerController.attackObj.SetActive(false);
    }

    public void ExitAttack() {
        playerController.attackObj.SetActive(false);
        playerController.attackCooldownTimer = 0;
        playerController.ChangeState(PLAYER_STATE.IDLE);
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.ATTACK;
    }

    //	use this for destruction
    public override void End() {
        
    }
}
