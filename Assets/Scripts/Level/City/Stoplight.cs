using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoplight : MonoBehaviour {
    public Transform gateArm;
    public SpriteRenderer icon;
    public Sprite goIcon;
    public Sprite stopIcon;
    public bool isLeft;

    BoxCollider2D _collider;

    private void Awake() {
        _collider = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Go() {
        gateArm.rotation = Quaternion.Euler(0f, 0f, 60f * (isLeft ? -1 : 1));
        icon.sprite = goIcon;
        _collider.enabled = false;
    }

    public void Stop() {
        gateArm.rotation = Quaternion.identity;
        icon.sprite = stopIcon;
        _collider.enabled = true;
    }
}
