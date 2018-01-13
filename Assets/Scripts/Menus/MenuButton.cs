using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
public class MenuButton : MenuOption {

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void Select() {
        base.Select();

        GetComponent<Button>().onClick.Invoke();
    }
}
