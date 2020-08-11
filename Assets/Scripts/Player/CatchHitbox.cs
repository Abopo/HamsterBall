using UnityEngine;
using System.Collections.Generic;

public class CatchHitbox : MonoBehaviour {
	public GameObject playerBubble;

    List<Hamster> _caughtHamsters = new List<Hamster>();
    Hamster _closestHamster;
    float _closestDist = 1000f;
    float _tempDist = 0f;

    PlayerController _playerController;

    private void Awake() {
		_playerController = transform.parent.GetComponent<PlayerController> ();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // If we caught at least one hamster last frame
	    if(_caughtHamsters.Count > 0 && _playerController.heldBall == null) {
            // Catch the correct one

            // Reset dist values
            _closestDist = 1000f;
            _tempDist = 0f;

            // For each of the caught hamsters
            foreach (Hamster ham in _caughtHamsters) {
                if(ham.wasCaught) {
                    continue;
                }

                // Check how close the hamster is
                _tempDist = Mathf.Abs(_playerController.transform.position.x - ham.transform.position.x);
                
                // If it's the closest, record it
                if(_tempDist < _closestDist) {
                    _closestDist = _tempDist;
                    _closestHamster = ham;
                }
            }

            // Catch the closest hamster

            // If we are networked
            if (PhotonNetwork.connectedAndReady) {
                // If we are the local client and aren't already trying to catch a hamster
                if (_playerController.PhotonView.owner == PhotonNetwork.player && _playerController.GetComponent<NetworkedPlayer>().tryingToCatchHamster == null) {
                    // Catch the hamster
                    CatchHamster(_closestHamster);

                    if (PhotonNetwork.isMasterClient) {
                        // Tell other clients that a hamster was caught
                        _playerController.PhotonView.RPC("HamsterCaught", PhotonTargets.Others, _closestHamster.hamsterNum);
                    } else {
                        // Have the master client double check that it's ok
                        _playerController.PhotonView.RPC("CheckHamster", PhotonTargets.MasterClient, _closestHamster.hamsterNum);
                    }
                }
            } else {
                CatchHamster(_closestHamster);
            }

            // Clear the hamster list
            _caughtHamsters.Clear();

        } else if(_caughtHamsters.Count > 0 && _playerController.heldBall != null) {
            // Clear the hamster list
            _caughtHamsters.Clear();
        }
    }

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Hamster" && _playerController.heldBall == null) {
            Hamster hamster = other.GetComponent<Hamster>();

            if (hamster.exitedPipe && !hamster.wasCaught) {
                _caughtHamsters.Add(hamster);
            }
        }
        if(other.tag == "PowerUp"/* && other.GetComponent<PowerUp>().exitedPipe*/) {
            PowerUp pUp = other.GetComponent<PowerUp>();
            pUp.Caught(_playerController);
        }
    }

    public void CatchHamster(Hamster hamster) {
        if (PhotonNetwork.connectedAndReady) {
            InstantiateNetworkBubble(hamster);
        } else {
            GameObject bubble = Instantiate(playerBubble) as GameObject;
            _playerController.heldBall = bubble.GetComponent<Bubble>();
            _playerController.heldBall.team = _playerController.team;
            _playerController.heldBall.PlayerController = _playerController;
            _playerController.heldBall.Initialize(hamster.type);
            _playerController.heldBall.GetComponent<CircleCollider2D>().enabled = false;
            _playerController.heldBall.HideSprites();
            _playerController.heldBall.gameObject.layer = LayerMask.NameToLayer("GhostBubble");
            _playerController.heldBall.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            if (hamster.isPlasma) {
                _playerController.heldBall.SetPlasma(true);
                //GameObject spiralEffect = hamster.spiralEffectInstance;
                //spiralEffect.transform.parent = _playerController.heldBall.transform;
                //spiralEffect.transform.position = new Vector3(_playerController.heldBall.transform.position.x,
                                                              //_playerController.heldBall.transform.position.y,
                                                              //_playerController.heldBall.transform.position.z + 3);
                //spiralEffect.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        // The hamster was caught.
        hamster.Caught();

        _playerController.aimCooldownTimer = 0.0f;

        // Tell animator we've got a bubble
        _playerController.Animator.SetBool("HoldingBall", true);

        // Turn of the catch hitbox so we don't accidently catch more hamsters
        _playerController.swingObj.SetActive(false);

        // For now this is only for the AI
        _playerController.significantEvent.Invoke();

		if (hamster.type == HAMSTER_TYPES.RAINBOW){
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterCollectRainbow);
		} else if (hamster.type ==HAMSTER_TYPES.SKULL){
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterCollectSkull);
		}
		else {
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterCollectSuccessOneshot);
		}
    }

    void InstantiateNetworkBubble(Hamster hamster) {
        object[] data = new object[3];
        data[0] = _playerController.playerNum;
        data[1] = hamster.type;
        data[2] = hamster.isPlasma;
        PhotonNetwork.Instantiate("Prefabs/Networking/Bubble_PUN", _playerController.transform.position, Quaternion.identity, 0, data);
    }
}
