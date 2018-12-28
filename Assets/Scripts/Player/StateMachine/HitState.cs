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
        if(playerController.heldBubble != null && !playerController.heldBubble.wasThrown) {
            if (PhotonNetwork.connectedAndReady) {
                if (PhotonNetwork.isMasterClient) {
                    DropNetworkHamster();
                }
            } else {
                DropHamster();
            }

            // Destroy the held bubble
            GameObject.Destroy(playerController.heldBubble.gameObject);
        }

        //hitTime = 3.0f;
        _hitTime = 0.25f;
        _hitTimer = 0f;
	}

    void DropHamster() {
        GameObject hamsterGO = GameObject.Instantiate(_hamsterObj, playerController.heldBubble.transform.position, Quaternion.identity) as GameObject;
        hamsterGO.transform.position = new Vector3(hamsterGO.transform.position.x, hamsterGO.transform.position.y, -0.5f);
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
                s.GetComponent<HamsterSpawner>().releasedHamsterCount++;
                break;
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

        // Tell the animator we don't have a bubble anymore
        playerController.Animator.SetBool("HoldingBall", false);
    }

    void DropNetworkHamster() {
        // Set up instantiation data
        object[] hamsterInfo = new object[5];
        hamsterInfo[0] = true; // has exited pipe
        hamsterInfo[1] = false; // inRightPipe (doesn't matter here)
        
        // Set the proper team
        if (playerController.shifted) {
            hamsterInfo[2] = (playerController.team == 1) ? 0 : 1;
        } else {
            hamsterInfo[2] = playerController.team;
        }

        // Set the correct type
        hamsterInfo[3] = playerController.heldBubble.type;
        if (playerController.heldBubble.isGravity) {
            hamsterInfo[4] = true; // isGravity
        } else {
            hamsterInfo[4] = false; // isGravity
        }

        // Use the network instantiate method
        PhotonNetwork.Instantiate("Prefabs/Networking/Hamster_PUN", playerController.heldBubble.transform.position, Quaternion.identity, 0, hamsterInfo);

        // Tell the animator we don't have a bubble anymore
        playerController.Animator.SetBool("HoldingBall", false);
    }

    // Update is called once per frame
    public override void Update() {
        // Fall
        playerController.ApplyGravity();

        _hitTimer += Time.deltaTime;
		if (_hitTimer >= _hitTime) {
			playerController.ChangeState(PLAYER_STATE.IDLE);
		}

        // If the player is holding a hamster, drop it
        if (playerController.heldBubble != null && !playerController.heldBubble.wasThrown) {
            if (PhotonNetwork.connectedAndReady) {
                if (PhotonNetwork.isMasterClient) {
                    DropNetworkHamster();
                }
            } else {
                DropHamster();
            }

            // Destroy the held bubble
            Object.Destroy(playerController.heldBubble.gameObject);
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
        // TODO: adjust this to feel better
        playerController.velocity = new Vector2(3f * hitDirection, 3f);
    }
}
