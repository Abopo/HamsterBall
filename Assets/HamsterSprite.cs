using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterSprite : MonoBehaviour {
    Rigidbody2D _rigidbody;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void Pop() {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f);
        _rigidbody.GetComponent<Animator>().SetInteger("State", 1);

        _rigidbody.isKinematic = false;
        _rigidbody.gravityScale = 2;

        float rX = Random.Range(1f, 2f);
        float rY = Random.Range(-0.5f, 2f);

        // TODO: set direction based on which side of the match the bubble/sprite is on
        Bubble bubble = transform.parent.GetComponent<Bubble>();
        float averageX = 0f;
        foreach(Bubble b in bubble.matches) {
            if (b != null) {
                averageX += b.transform.position.x;
            }
        }
        averageX = averageX / bubble.matches.Count;

        float dir = 1f;
        if (transform.position.x < averageX) {
            dir = -1f;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        _rigidbody.velocity = new Vector2(rX * dir, 6f + rY);
    }
}
