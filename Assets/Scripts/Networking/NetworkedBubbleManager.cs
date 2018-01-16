using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedBubbleManager : Photon.MonoBehaviour {
    BubbleManager _bubbleManager;

	// Use this for initialization
	void Start () {
        _bubbleManager = GetComponent<BubbleManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // For initial setup to be synced.
            if (BubbleManager.startingBubbleTypes[0] != -1) {
                for(int i = 0; i < 50; ++i) {
                    stream.SendNext(BubbleManager.startingBubbleTypes[i]);
                }

                //for (int i = 0; i < 13; ++i) {
                //    stream.SendNext(_bubbleManager.NextLineBubbles[i]);
                //}
            }
        } else {
            // For initial setup to be synced.
            if (BubbleManager.startingBubbleTypes[0] == -1) {
                for (int i = 0; i < 50; ++i) {
                    BubbleManager.startingBubbleTypes[i] = (int)stream.ReceiveNext();
                }

                //for (int i = 0; i < 13; ++i) {
                //    _bubbleManager.NextLineBubbles[i] = (int)stream.ReceiveNext();
                //}

                _bubbleManager.SpawnStartingBubbles();
            }
        }
    }

    [PunRPC]
    void SyncLineBubbles(int[] lineBubblesList) {
        _bubbleManager.SetNextLineBubbles(lineBubblesList);
    }

    [PunRPC]
    void AddLine(/*int[] nextLineBubbles*/) {
        _bubbleManager.AddLine();

        //for (int i = 0; i < 13; ++i) {
        //    _bubbleManager.NextLineBubbles[i] = nextLineBubbles[i];
        //}
    }

    // Checks to make sure that the board matches the master client's baord.
    [PunRPC]
    void BoardLayoutCheck(int[] boardBubbles) {

    }
}
