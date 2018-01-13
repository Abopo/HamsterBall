using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class SetupReadyButton : MenuOption {
    public GameObject infoBox;

    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void Highlight() {
        base.Highlight();

        if(infoBox != null) {
            infoBox.SetActive(false);
        }
    }

    protected override void Select() {
        base.Select();

        GetComponent<Button>().onClick.Invoke();
    }
}
