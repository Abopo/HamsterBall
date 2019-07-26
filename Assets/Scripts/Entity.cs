using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityPhysics))]
public class Entity : MonoBehaviour {
	public Vector2 velocity;
	public float gravity = 20;

    private float _waterMultiplier; // Adjusts movement when in water
    private float _waterGravMultiplier; // Adjusts gravity when in water
    protected bool grounded;
    protected bool facingRight = true;
    protected bool _springing;

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

    public float WaterMultiplier {
        get {
            return _waterMultiplier;
        }
    }

    public int curFacing; // 0 - Right, 1 - Down, 2 - Left, 3 - Up (usually only used for networking info)

    private WaterController _waterController;

    protected virtual void Awake() {

    }

    // Use this for initialization
    protected virtual void Start () {
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }

		velocity = Vector2.zero;
		_physics = GetComponent<EntityPhysics>();

        _waterMultiplier = 1f;
        _waterGravMultiplier = 1f;
        _waterController = FindObjectOfType<WaterController>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	    if(_waterController != null) {
            if(transform.position.y < _waterController.WaterHeight && _waterMultiplier == 1f) {
                _waterMultiplier = 0.55f;
                _waterGravMultiplier = 0.4f;
                velocity.y = velocity.y * 0.25f;
            } else if(transform.position.y > _waterController.WaterHeight && _waterMultiplier == 0.55f) {
                _waterMultiplier = 1f;
                _waterGravMultiplier = 1f;
            }
        }
	}

	public void ApplyGravity() {
		velocity.y -= gravity * _waterGravMultiplier * Time.deltaTime;
	}
	
	public virtual void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		if (_animator == null) {
			_animator = GetComponentInChildren<Animator> ();
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
        if (collider.gameObject.layer == 21 /*Platform*/ || collider.gameObject.layer == 18/*Fallthrough*/) {
            if(!_springing) {
                velocity.y = 0.0f;
            }
        }
    }

    public virtual void Spring(float springForce) {
        velocity.y = springForce;
        _springing = true;
        // Restrict x velocity while rising
        velocity.x = 0;
    }

    public virtual void Respawn() {

    }
}
