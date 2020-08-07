using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveUp : ScriptingController {

    PlayerController _player;
    InputState _playerInput = new InputState();

    // Start is called before the first frame update
    protected override void Start() {
        _player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        // Apply the input state
        _player.TakeInput(_playerInput);
    }

    public override void Begin() {
        base.Begin();
        FaceLeft();
    }

    void FaceLeft() {
        _playerInput.left.isDown = true;
        nextTime = 0.1f;
        nextAction = Wait1;
    }

    void Wait1() {
        ResetInput();
        nextTime = 2.0f;
        nextAction = MoveLeft;
    }

    void MoveLeft() {
        _playerInput.left.isDown = true;
        nextTime = 0.25f;
        nextAction = StartJump1;
    }

    void StartJump1() {
        _playerInput.left.isDown = true;
        _playerInput.jump.isDown = true;
        _playerInput.jump.isJustPressed = true;
        nextTime = 0.1f;
        nextAction = HoldJump1;
    }

    void HoldJump1() {
        _playerInput.jump.isJustPressed = false;
        nextTime = 0.75f;
        nextAction = MoveRight;
    }

    void MoveRight() {
        ResetInput();
        _playerInput.right.isDown = true;
        nextTime = 0.2f;
        nextAction = StartJump2;
    }

    void StartJump2() {
        _playerInput.jump.isDown = true;
        _playerInput.jump.isJustPressed = true;
        nextTime = 0.1f;
        nextAction = HoldJump2;
    }

    void HoldJump2() {
        _playerInput.jump.isJustPressed = false;
        nextTime = 0.55f;
        nextAction = Wait2;
    }

    void Wait2() {
        ResetInput();
        nextTime = 1.75f;
        nextAction = CatchHamster;
    }

    void CatchHamster() {
        _player.ChangeState(PLAYER_STATE.CATCH);
        nextTime = 0.1f;
        nextAction = Wait3;
    }

    void Wait3() {
        ResetInput();
        nextTime = 1.5f;
        nextAction = StartAim;
    }

    void StartAim() {
        _player.ChangeState(PLAYER_STATE.THROW);

        // Tell main script to begin camera change
        FindObjectOfType<CameraExpand>().Begin();

        // Wait for further instructions
        nextAction = null;
    }

    public void Throw1() {
        _playerInput.swing.isJustPressed = true;
        nextTime = 0.1f;
        nextAction = Wait4;
    }

    void Wait4() {
        ResetInput();
        FindObjectOfType<TrailerOpeningScript>().SpawnHamster2();
        nextTime = 2f;
        nextAction = MoveLeft2;
    }

    void MoveLeft2() {
        _playerInput.left.isDown = true;
        nextTime = 0.08f;
        nextAction = StartJump3;
    }

    void StartJump3() {
        _playerInput.jump.isDown = true;
        _playerInput.jump.isJustPressed = true;
        nextTime = 0.1f;
        nextAction = HoldJump3;
    }

    void HoldJump3() {
        _playerInput.jump.isJustPressed = false;
        nextTime = 0.55f;
        nextAction = Wait5;
    }

    void Wait5() {
        ResetInput();
        nextTime = 0.65f;
        nextAction = CatchHamster2;
    }

    void CatchHamster2() {
        _player.ChangeState(PLAYER_STATE.CATCH);
        nextTime = 0.1f;
        nextAction = Wait6;
    }

    void Wait6() {
        ResetInput();
        nextTime = 1.5f;
        nextAction = FaceRight;
    }

    void FaceRight() {
        _playerInput.right.isDown = true;
        nextTime = 0.1f;
        nextAction = StartAim2;
    }

    void StartAim2() {
        ResetInput();
        _player.ChangeState(PLAYER_STATE.THROW);
        nextTime = 0.1f;
        nextAction = AdjustAim;
    }

    void AdjustAim() {
        _playerInput.right.isDown = true;
        nextTime = 0.25f;
        nextAction = Throw2;
    }

    void Throw2() {
        _playerInput.swing.isJustPressed = true;
        nextTime = 0.1f;
        nextAction = Wait7;
    }

    void Wait7() {
        ResetInput();
        nextTime = 0.75f;
        nextAction = End;
    }

    void End() {
        // Tell main script to begin camera change
        FindObjectOfType<CameraExpand>().Expand2();

        began = false;
    }

    void ResetInput() {
        _playerInput.ResetAllInput();
    }
}
