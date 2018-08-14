using UnityEngine;
using System.Collections;

public class AttackBubble : MonoBehaviour {
	public GameObject playerBubble;

    PlayerController _playerController;

    private void Awake() {
		_playerController = transform.parent.GetComponent<PlayerController> ();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Hamster" && _playerController.heldBubble == null) {
            // && other.GetComponent<Hamster>().exitedPipe && 
            Hamster hamster = other.GetComponent<Hamster>();

            if (hamster.exitedPipe && !hamster.wasCaught) {
                // If we are networked
                if (PhotonNetwork.connectedAndReady) {
                    // If we are the local client and aren't already trying to catch a hamster
                    if (_playerController.GetComponent<PhotonView>().owner == PhotonNetwork.player && _playerController.GetComponent<NetworkedPlayer>().tryingToCatchHamster == null) {
                        // Catch the hamster
                        CatchHamster(hamster);
                        if (PhotonNetwork.isMasterClient) {
                            // Tell other clients that a hamster was caught
                            _playerController.GetComponent<PhotonView>().RPC("HamsterCaught", PhotonTargets.All, hamster.hamsterNum);
                        } else {
                            // Have the master client double check that it's ok
                            _playerController.GetComponent<PhotonView>().RPC("CheckHamster", PhotonTargets.MasterClient, hamster.hamsterNum);
                        }
                    }
                } else {
                    CatchHamster(hamster);
                }
            }
        }
        if(other.tag == "PowerUp"/* && other.GetComponent<PowerUp>().exitedPipe*/) {
            PowerUp pUp = other.GetComponent<PowerUp>();
            other.GetComponent<PowerUp>().Caught(_playerController);
        }
    }

    public void CatchHamster(Hamster hamster) {
        if (PhotonNetwork.connectedAndReady) {
            InstantiateNetworkBubble(hamster);
        } else {
            GameObject bubble = Instantiate(playerBubble) as GameObject;
            _playerController.heldBubble = bubble.GetComponent<Bubble>();
            _playerController.heldBubble.team = _playerController.team;
            _playerController.heldBubble.PlayerController = _playerController;
            _playerController.heldBubble.Initialize(hamster.type);
            _playerController.heldBubble.GetComponent<CircleCollider2D>().enabled = false;

            if (hamster.isGravity) {
                _playerController.heldBubble.isGravity = true;
                GameObject spiralEffect = hamster.spiralEffectInstance;
                spiralEffect.transform.parent = _playerController.heldBubble.transform;
                spiralEffect.transform.position = new Vector3(_playerController.heldBubble.transform.position.x,
                                                              _playerController.heldBubble.transform.position.y,
                                                              _playerController.heldBubble.transform.position.z + 3);
                spiralEffect.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        // If we are networked, send RPC
        //if(PhotonNetwork.connectedAndReady && _playerController.GetComponent<PhotonView>().owner == PhotonNetwork.player) {
        //    _playerController.GetComponent<PhotonView>().RPC("CatchHamster", PhotonTargets.Others, hamster.hamsterNum);
        //}

        // The hamster was caught.
        hamster.Caught();

        _playerController.aimCooldownTimer = 0.0f;

        // Tell animator we've got a bubble
        _playerController.Animator.SetBool("HoldingBall", true);

        // For now this is only for the AI
        _playerController.significantEvent.Invoke();
    }

    void InstantiateNetworkBubble(Hamster hamster) {
        object[] data = new object[3];
        data[0] = _playerController.playerNum;
        data[1] = hamster.type;
        data[2] = hamster.isGravity;
        PhotonNetwork.Instantiate("Prefabs/Networking/Bubble_PUN", _playerController.transform.position, Quaternion.identity, 0, data);
    }
}
