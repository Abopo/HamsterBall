using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RemapButton : MonoBehaviour {
    public int actionID;
    public GameObject _inputAnyKeyObj;
    public SuperTextMesh _buttonText;

    InputRemappingMenu _inputRemapping;

    void Awake() {
        _inputRemapping = FindObjectOfType<InputRemappingMenu>();
    }
    // Start is called before the first frame update
    void Start() {
        // Display the button corresponding to our action
        UpdateButtonText();

        _inputRemapping.InputMapper.InputMappedEvent += OnButtonRemapped;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartRemap() {
        // Move input obj to us
        _inputAnyKeyObj.transform.position = transform.position;
        _inputAnyKeyObj.SetActive(true);
    }

    void OnButtonRemapped(InputMapper.InputMappedEventData mapData) {
        // If our button was the one changed
        if (mapData.actionElementMap.actionId == actionID) {
            // Update the button text
            _buttonText.text = mapData.actionElementMap.elementIdentifierName;

            // Turn off input obj
            _inputAnyKeyObj.SetActive(false);
        }
    }

    public void UpdateButtonText() {
        Player player = ReInput.players.GetPlayer(0);
        ActionElementMap aeMap = player.controllers.maps.GetFirstButtonMapWithAction(actionID, true);
        _buttonText.text = aeMap.elementIdentifierName;
    }
}
