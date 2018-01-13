using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class SelectMapButton : MenuOption {
    public MapSelect mapSelect;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        // If we are online and not the master client, don't show the select button
        if(PhotonNetwork.connectedAndReady && !PhotonNetwork.isMasterClient) {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void Select() {
        base.Select();

        // If the map selection is rotating, don't select yet
        if(!mapSelect.IsRotating()) {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
