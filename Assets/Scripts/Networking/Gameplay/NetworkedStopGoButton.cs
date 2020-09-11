using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedStopGoButton : Photon.MonoBehaviour {
    StopGoButton _stopGoButton;

    private void Awake() {
        _stopGoButton = GetComponent<StopGoButton>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }


    // Update is called once per frame
   void Update() {
        
    }

    [PunRPC]
    void NetworkPress() {
        Debug.Log("Checking button press...");

        // If we should be pressed but aren't
        if(!_stopGoButton.pressed) {
            Debug.Log("Button not pressed, pressing...");

            // Press our button
            _stopGoButton.Press();
        }
    }
}
