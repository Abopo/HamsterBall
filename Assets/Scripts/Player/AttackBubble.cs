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
		if (other.tag == "Hamster" && other.GetComponent<Hamster>().exitedPipe &&  _playerController.heldBubble == null &&
            !(PhotonNetwork.connectedAndReady && _playerController.GetComponent<PhotonView>().owner != PhotonNetwork.player)) {
            CatchHamster(other.GetComponent<Hamster>());
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
        if(PhotonNetwork.connectedAndReady && _playerController.GetComponent<PhotonView>().owner == PhotonNetwork.player) {
            _playerController.GetComponent<PhotonView>().RPC("CatchHamster", PhotonTargets.Others, hamster.hamsterNum);
        }

        // The hamster was caught.
        hamster.Caught();

        _playerController.aimCooldownTimer = 0.0f;

        // For now this is only for the AI
        _playerController.significantEvent.Invoke();
    }

    void InstantiateNetworkBubble(Hamster hamster) {
        object[] data = new object[2];
        data[0] = _playerController.playerNum;
        data[1] = hamster.hamsterNum;
        GameObject bubble = PhotonNetwork.Instantiate("Prefabs/Networking/Bubble_PUN", _playerController.transform.position, Quaternion.identity, 0, data);
    }
}
