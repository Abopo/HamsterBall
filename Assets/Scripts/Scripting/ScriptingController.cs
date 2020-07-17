using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptingController : MonoBehaviour {

    public delegate void Del();

    protected bool began;
    protected Del nextAction;
    protected float nextTimer = 0f;
    protected float nextTime;

    protected virtual void Awake() {
        Setup();
    }
    // Start is called before the first frame update
    protected virtual void Start() {
        Begin();
    }

    public virtual void Setup() {

    }

    // Update is called once per frame
    protected virtual void Update() {
        if (began && nextAction != null) {
            nextTimer += Time.deltaTime;
            if (nextTimer >= nextTime) {
                Debug.Log(nextAction.Method.ToString());
                nextAction();
                nextTimer = 0f;
            }
        }
    }

    public virtual void Begin() {
        began = true;
    }
}