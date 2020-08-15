using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedWaterBubble : MonoBehaviour {
    WaterBubble _waterBubble;

    // Start is called before the first frame update
    void Start() {

    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        _waterBubble = GetComponent<WaterBubble>();
        PhotonView photonView = GetComponent<PhotonView>();

        _waterBubble.team = (int)photonView.instantiationData[0];
    }

    // Update is called once per frame
    void Update() {

    }

    public void InstantiateNetworkBubble(Hamster hamster) {
        object[] data = new object[5];
        data[0] = -50;
        data[1] = hamster.type;
        data[2] = hamster.isPlasma;
        data[3] = _waterBubble.team;
        data[4] = GetComponent<PhotonView>().viewID;
        GameObject newBubble = PhotonNetwork.Instantiate("Prefabs/Networking/Bubble_PUN", transform.position, Quaternion.identity, 0, data);
        _waterBubble.CaughtBubble = newBubble.GetComponent<Bubble>();
    }

    [PunRPC]
    void CatchHamster(int hamsterNum) {
        // find the hamster with the same number
        HamsterScan hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        Hamster hamster = hamsterScan.GetHamster(hamsterNum);

        if (hamster != null && !hamster.wasCaught) {
            // The hamster was caught.
            hamster.Caught();
        }

    }

    [PunRPC]
    public void DropNetworkHamster() {
        if (_waterBubble.CaughtBubble != null) {
            // Set up instantiation data
            object[] hamsterInfo = new object[5];
            hamsterInfo[0] = true; // has exited pipe
            hamsterInfo[1] = false; // inRightPipe (doesn't matter here)
            hamsterInfo[2] = _waterBubble.team; // the team

            // Set the correct type
            hamsterInfo[3] = _waterBubble.CaughtBubble.type;
            hamsterInfo[4] = _waterBubble.CaughtBubble.isPlasma;

            // Use the network instantiate method
            PhotonNetwork.Instantiate("Prefabs/Networking/Hamster_PUN", _waterBubble.CaughtBubble.transform.position, Quaternion.identity, 0, hamsterInfo);

            _waterBubble.CaughtBubble = null;
        }
    }
}
