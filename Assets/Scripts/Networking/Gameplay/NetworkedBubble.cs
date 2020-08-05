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
        
        // If this was a bubble caught by the player
        if (photonView.instantiationData[0] != null) {
            PlayerController playerController = playerSpawner.GetPlayer((int)photonView.instantiationData[0]);
            playerController.heldBall = _bubble;
            playerController.heldBall.team = playerController.team;
            playerController.heldBall.PlayerController = playerController;
            playerController.heldBall.Initialize((HAMSTER_TYPES)photonView.instantiationData[1]);
            playerController.heldBall.GetComponent<CircleCollider2D>().enabled = false;
            playerController.heldBall.HideSprites();

            // if it's a gravity hamster
            if ((bool)photonView.instantiationData[2]) {
                playerController.heldBall.isPlasma = true;
            }
        // Otherwise it's parent should be a water bubble
        } else {
            // Find the parent water bubble
            PhotonView parentBubble = PhotonView.Find((int)photonView.instantiationData[4]);
            parentBubble.GetComponent<WaterBubble>().CaughtBubble = _bubble;
            transform.parent = parentBubble.transform;

            transform.localPosition = new Vector3(0f, 0f, 0f);
            _bubble.team = (int)photonView.instantiationData[3];
            _bubble.PlayerController = null;
            _bubble.Initialize((HAMSTER_TYPES)photonView.instantiationData[1]);
            _bubble.GetComponent<CircleCollider2D>().enabled = false;

            _bubble.isPlasma = (bool)photonView.instantiationData[2];
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
    void AddToBoard(int team, int node) {
        // Find the proper bubble manager
        _bubble.team = team;
        BubbleManager[] bubManagers = FindObjectsOfType<BubbleManager>();
        foreach(BubbleManager bMan in bubManagers) {
            if(bMan.team == team) {
                _bubble.HomeBubbleManager = bMan;
                break;
            }
        }

        _bubble.HomeBubbleManager.AddBubble(_bubble, node);
        _bubble.CollisionWithBoard(_bubble.HomeBubbleManager);
    }
}
