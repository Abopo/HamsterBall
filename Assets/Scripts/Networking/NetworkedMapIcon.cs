using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedMapIcon : MonoBehaviour {

    MapIcon _mapIcon;
    int mapIndex;
    int index;

	// Use this for initialization
	void Start () {
        _mapIcon = GetComponent<MapIcon>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(_mapIcon.MapIndex);
            stream.SendNext(_mapIcon.index);
        } else {
            mapIndex = (int)stream.ReceiveNext();
            index = (int)stream.ReceiveNext();
            _mapIcon.SetMap(mapIndex, index);
        }
    }
}
