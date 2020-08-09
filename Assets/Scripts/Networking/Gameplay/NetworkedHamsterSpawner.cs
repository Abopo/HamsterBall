using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkedHamsterSpawner : Photon.MonoBehaviour {
    HamsterSpawner _hamsterSpawner;
    PhotonView _photonView;

    private void Awake() {
        _hamsterSpawner = GetComponent<HamsterSpawner>();
        _photonView = GetComponent<PhotonView>();
        Debug.Log("Assigned photon view?" + (_photonView != null));
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    public void ReleaseHamster() {
        _photonView.RPC("ReleaseNetworkHamster", PhotonTargets.All);
    }

    [PunRPC]
    void ReleaseNetworkHamster() {
        _hamsterSpawner.ReleaseNextHamster();
    }
}
