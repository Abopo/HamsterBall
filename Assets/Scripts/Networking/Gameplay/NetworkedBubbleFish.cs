using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBubbleFish : MonoBehaviour{

    BubbleFish _bubbleFish;

    // Start is called before the first frame update
    void Start() {
        
    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        _bubbleFish = GetComponent<BubbleFish>();
        PhotonView photonView = transform.parent.GetComponent<PhotonView>();

        _bubbleFish.team = (int)photonView.instantiationData[0];
    }

    // Update is called once per frame
    void Update() {
        
    }
}
