using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedBubbleManager : Photon.MonoBehaviour {
    BubbleManager _bubbleManager;

    float _boardCheckTime = 5.0f;
    float _boardCheckTimer = 0f;

	// Use this for initialization
	void Start () {
        _bubbleManager = GetComponent<BubbleManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient) {
            _boardCheckTimer += Time.deltaTime;
            if (_boardCheckTimer >= _boardCheckTime) {
                //SendBoardLayoutCheck();
            }
        }
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

    void SendBoardLayoutCheck() {
        int[] bubblesPerNode = new int[_bubbleManager.nodeList.Count];
        int i = 0;
        foreach (Node n in _bubbleManager.nodeList) {
            if (n.bubble != null) {
                bubblesPerNode[i] = (int)n.bubble.type;
            } else {
                bubblesPerNode[i] = -1;
            }
            ++i;
        }

        photonView.RPC("BoardLayoutCheck", PhotonTargets.Others, bubblesPerNode);
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

    // Checks to make sure that the board matches the master client's board.
    [PunRPC]
    void BoardLayoutCheck(int[] boardBubbles) {
        int i = 0;
        foreach (Node n in _bubbleManager.nodeList) {
            // If there shouldn't be a bubble where there currently is one
            if(boardBubbles[i] == -1 && n.bubble != null) {
                // Destroy that bubble
                n.bubble.Pop();
            }
            // If there should be a bubble here but we don't have one
            if (boardBubbles[i] != -1 && n.bubble == null) {
                // Make a new bubble
                GameObject bub = Instantiate(_bubbleManager.bubbleObj, n.nPosition, Quaternion.identity) as GameObject;
                Bubble bubble = bub.GetComponent<Bubble>();
                _bubbleManager.AddBubble(bubble, n.number);
            }
            // if the bubble here isn't the right type
            if (boardBubbles[i] != (int)n.bubble.type) {
                // Switch to the correct type
                n.bubble.SetType(boardBubbles[i]);
            }

            ++i;
        }
    }
}
