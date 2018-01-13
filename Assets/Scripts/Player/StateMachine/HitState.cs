using UnityEngine;
using System.Collections;

public class HitState : PlayerState {
    //SpriteRenderer bubbleSprite;
    GameObject _hamsterObj;

	float _hitTime;
	float _hitTimer;

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        //playerController.Shift ();
        //bubbleSprite = playerController.transform.GetChild (2).GetComponent<SpriteRenderer> ();
        //bubbleSprite.enabled = true;

        _hamsterObj = Resources.Load("Prefabs/Entities/Hamster") as GameObject;

        // TODO: Play SFX
        playerController.PlayerAudio.PlayHitClip();

        // If the player is holding a hamster, drop it
        if(playerController.heldBubble != null) {
            DropHamster();
        }

        //hitTime = 3.0f;
        _hitTime = 0.1f;
        _hitTimer = 0;
	}

    void DropHamster() {
        GameObject hamsterGO = GameObject.Instantiate(_hamsterObj, playerController.heldBubble.transform.position, Quaternion.identity) as GameObject;
        Hamster hamster = hamsterGO.GetComponent<Hamster>();
        // Set the correct team and parent spawner
        if (playerController.shifted) {
            hamster.Initialize((playerController.team == 1) ? 0 : 1);
        } else {
            hamster.Initialize(playerController.team);
        }

        hamster.exitedPipe = true;

        // Set a parent spawner
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Hamster Spawner");
        foreach (GameObject s in spawners) {
            if (s.GetComponent<HamsterSpawner>().team == hamster.team) {
                hamster.ParentSpawner = s.GetComponent<HamsterSpawner>();
                s.GetComponent<HamsterSpawner>().hamsterCount++;
            }
        }

        // Set the correct type
        if (playerController.heldBubble.isGravity) {
            hamster.SetType(11, playerController.heldBubble.type);
        } else {
            hamster.SetType((int)playerController.heldBubble.type);
        }

        // Randomly choose direction
        if(Random.Range(0, 11) > 5) {
            hamster.Flip();
        }

        // Destroy the held bubble
        GameObject.Destroy(playerController.heldBubble.gameObject);
    }

    // Update is called once per frame
    public override void Update() {
		_hitTimer += Time.deltaTime;
		if (_hitTimer >= _hitTime) {
			playerController.ChangeState(PLAYER_STATE.IDLE);
		}
	}

	public override void CheckInput(InputState inputState) {

	}

	// returns the PLAYER_STATE that represents this state
	public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.HIT;
	}

	//	use this for destruction
	public override void End(){
		_hitTimer = 0;
        playerController.StartInvulnTime();
		//bubbleSprite.enabled = false;
	}

    public void Knockback(int hitDirection) {
        // Knock the player back
        playerController.velocity = new Vector2(10f * hitDirection, 1f);
    }
}
