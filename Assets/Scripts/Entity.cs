﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
public class Entity : MonoBehaviour {
	public Vector2 velocity;
	public float gravity;
	
	protected bool grounded;
    protected bool facingRight = true;

    protected EntityPhysics _physics;
	public EntityPhysics Physics {
		get { return _physics; }
	}
	protected bool _grounded = false;			// Whether or not the player is grounded.
	public bool Grounded {
		get { return _grounded; }
		set { _grounded = value; }
	}
	protected Animator _animator;
	public Animator Animator {
		get { return _animator; }
	}

    public bool FacingRight {
        get { return facingRight; }
        set { facingRight = value;}
    }


    public int curFacing; // 0 - Right, 1 - Down, 2 - Left, 3 - Up (usually only used for networking info)

    // Use this for initialization
    protected virtual void Start () {
		_animator = GetComponent<Animator> ();
		
		velocity = Vector2.zero;
		_physics = GetComponent<EntityPhysics>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ApplyGravity() {
		velocity.y -= gravity * Time.deltaTime;
	}
	
	public void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		if (_animator == null) {
			_animator = GetComponent<Animator> ();
		}
		_animator.SetBool ("FacingRight", facingRight);

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void FaceUp() {
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Sign(transform.localScale.x) * 90f);
        curFacing = 3;
    }
    public void FaceDown() {
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Sign(transform.localScale.x) * -90f);
        curFacing = 1;
    }
    public void FaceLeft() {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        if (facingRight) {
            Flip();
        }
        curFacing = 2;
    }
    public void FaceRight() {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        if (!facingRight) {
            Flip();
        }
        curFacing = 0;
    }


    public virtual void CollisionResponseX(Collider2D collider) {

	}
	public virtual void CollisionResponseY(Collider2D collider) {
		
	}

    public virtual void Spring(float springForce) {

    }
}
