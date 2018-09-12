using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterIcon : Photon.MonoBehaviour {

    CharacterIcon _characterIcon;

	// Use this for initialization
	void Start () {
        _characterIcon = GetComponent<CharacterIcon>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Input
            stream.Serialize(ref _characterIcon.isLocked);
        } else {
            // Input
            bool locked = false;
            stream.Serialize(ref locked);

            if(locked && !_characterIcon.isLocked) {
                _characterIcon.Lock();
            } else if(!locked && _characterIcon.isLocked) {
                _characterIcon.Unlock();
            }
        }
    }
}
