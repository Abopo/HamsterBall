using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingIcon : MonoBehaviour {

    bool _connected;
    float _connectTime = 1.0f;
    float _connectTimer = 0f;

    SuperTextMesh _connectText;

    private void Awake() {
        _connectText = GetComponentInChildren<SuperTextMesh>();
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if(_connected) {
            _connectTimer += Time.deltaTime;
            if(_connectTimer >= _connectTime) {
                gameObject.SetActive(false);
            }
        }
    }

    public void OnReceivedRoomListUpdate() {
        Debug.Log("Room list update");

        _connected = true;
        _connectText.text = "Connected!";
    }
}
