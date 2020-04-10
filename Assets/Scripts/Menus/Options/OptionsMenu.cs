using UnityEngine;
using System.Collections;

public class OptionsMenu : Menu {

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public void DeleteSaveData() {
        ES3.DeleteFile();
    }
}
