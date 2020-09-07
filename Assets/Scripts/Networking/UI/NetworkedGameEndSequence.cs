using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedGameEndSequence : MonoBehaviour {
    
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
        } else {
        }
    }
}
