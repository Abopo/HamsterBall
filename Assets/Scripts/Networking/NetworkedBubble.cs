using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedBubble : Photon.MonoBehaviour {
    Bubble _bubble;

	// Use this for initialization
	void Start () {
	}

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        _bubble = GetComponent<Bubble>();
        PhotonView photonView = GetComponent<PhotonView>();
        NetworkedPlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<NetworkedPlayerSpawner>();
        PlayerController playerController = playerSpawner.GetPlayer((int)photonView.instantiationData[0]);
        HamsterScan hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        Hamster hamster = hamsterScan.GetHamster((int)photonView.instantiationData[1]);

        playerController.heldBubble = _bubble;
        playerController.heldBubble.team = playerController.team;
        playerController.heldBubble.PlayerController = playerController;
        playerController.heldBubble.Initialize(hamster.type);
        playerController.heldBubble.GetComponent<CircleCollider2D>().enabled = false;

        if (hamster.isGravity) {
            playerController.heldBubble.isGravity = true;
            GameObject spiralEffect = hamster.spiralEffectInstance;
            spiralEffect.transform.parent = playerController.heldBubble.transform;
            spiralEffect.transform.position = new Vector3(playerController.heldBubble.transform.position.x,
                                                          playerController.heldBubble.transform.position.y,
                                                          playerController.heldBubble.transform.position.z + 3);
            spiralEffect.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
        } else {

        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    [PunRPC]
    void AddToBoard(int node) {
        _bubble.HomeBubbleManager.AddBubble(_bubble, node);
        _bubble.CollisionWithBoard();
    }
}
