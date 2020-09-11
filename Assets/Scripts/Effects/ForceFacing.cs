using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFacing : MonoBehaviour {

    public bool faceRight;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(faceRight && transform.lossyScale.x < 0) {
            Transform parent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.parent = parent;
        } else if(!faceRight && transform.localScale.x > 0) {
            Transform parent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.parent = parent;
        }
    }
}
