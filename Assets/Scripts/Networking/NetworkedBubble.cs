using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedBubble : Photon.MonoBehaviour {
    Bubble _bubble;
    GameObject _spiralEffectObj;

    // Use this for initialization
    void Start () {
	}

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        _bubble = GetComponent<Bubble>();
        PhotonView photonView = GetComponent<PhotonView>();
        NetworkedPlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<NetworkedPlayerSpawner>();
        PlayerController playerController = playerSpawner.GetPlayer((int)photonView.instantiationData[0]);

        playerController.heldBubble = _bubble;
        playerController.heldBubble.team = playerController.team;
        playerController.heldBubble.PlayerController = playerController;
        playerController.heldBubble.Initialize((HAMSTER_TYPES)photonView.instantiationData[1]);
        playerController.heldBubble.GetComponent<CircleCollider2D>().enabled = false;

        // if it's a gravity hamster
        if ((bool)photonView.instantiationData[2]) {
            playerController.heldBubble.isGravity = true;
            _spiralEffectObj = Resources.Load("Prefabs/Effects/SpiralEffect") as GameObject;
            GameObject spiralEffect = Instantiate(_spiralEffectObj, playerController.heldBubble.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            spiralEffect.transform.parent = transform;
            spiralEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);
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
