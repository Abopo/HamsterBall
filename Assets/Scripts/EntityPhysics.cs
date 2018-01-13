using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class EntityPhysics : MonoBehaviour {
    public LayerMask collisionMask1;
    public LayerMask collisionMask2;
    public float wallCheckDist;

    private Entity entity;
    private Vector2 _pos;
    private CircleCollider2D _collider;
    private float _radius;
    private float _scaledRadius;
    private Vector2 _center;

    private float _skin = 0.005f;

    private Ray2D _ray;
    private RaycastHit2D _hit;

    private bool isTouchingCeiling;
    private int ceilingHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingFloor;
    private int floorHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallLeft;
    private int leftHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallRight;
    private int rightHitCount; // this is a counter for how much of the top of the player is colliding

    public bool IsTouchingCeiling {
        get { return isTouchingCeiling; }
    }
    public bool IsTouchingFloor {
        get { return isTouchingFloor; }
    }
    public bool IsTouchingWallLeft {
        get { return isTouchingWallLeft; }
    }
    public bool IsTouchingWallRight {
        get { return isTouchingWallRight; }
    }

    void Start() {
        entity = GetComponent<Entity>();
        _collider = GetComponent<CircleCollider2D>();
        _radius = _collider.radius;
        _scaledRadius = Mathf.Abs(_radius * transform.localScale.x);
        _center = _collider.offset;
        wallCheckDist = 0.1f;
    }

    void Update() {
        CheckBelow();
        WallCheck();
    }

    public void MoveX(float deltaX) {
        if (deltaX == 0) {
            return;
        }

        for (int i = 0; i < 3; ++i) {
            float dir = Mathf.Sign(deltaX);
            float x = _pos.x + _center.x + _scaledRadius * dir;
            float y = (_pos.y + _center.y - _scaledRadius / 1.5f) + _scaledRadius / 1.5f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * dir);
            Debug.DrawRay(_ray.origin, _ray.direction * Mathf.Abs(deltaX));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(deltaX), collisionMask1);
            if (_hit) {
                float dst = Vector2.Distance(_ray.origin, _hit.point);

                if (dst > _skin) {
                    deltaX = dst * dir + _skin;
                } else {
                    deltaX = 0;
                }

                entity.CollisionResponseX(_hit.collider);
                break;
            }
        }

        transform.Translate(deltaX, 0.0f, 0.0f);
    }

    public void MoveY(float inDeltaY) {
        if (inDeltaY == 0) {
            return;
        }

        ceilingHitCount = 0;
        floorHitCount = 0;
        float deltaY = inDeltaY;
        _pos = transform.position;

        for (int i = 0; i < 3; ++i) {
            float dir = Mathf.Sign(inDeltaY);
            float x = (_pos.x + _center.x - _scaledRadius / 2) + _scaledRadius / 2 * i;
            float y = _pos.y + _center.y + _scaledRadius * dir;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * dir);
            Debug.DrawRay(_ray.origin, _ray.direction * Mathf.Abs(inDeltaY));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(inDeltaY), collisionMask1);
            if (_hit) {
                if (dir == 1) {
                    ceilingHitCount++;
                } else if (dir == -1) {
                    floorHitCount++;
                }
                if (ceilingHitCount > 1 || floorHitCount > 1) {
                    continue;
                }

                float dst = Vector2.Distance(_ray.origin, _hit.point);

                if (dst > _skin) {
                    deltaY = dst * dir + _skin;
                } else {
                    deltaY = 0;
                }

                entity.CollisionResponseY(_hit.collider);
                //break;
            }
        }

        transform.Translate(0.0f, deltaY, 0.0f);
    }

    // Checks if there is anything below the player
    public void CheckBelow() {
        _pos = transform.position;
        isTouchingFloor = false;
        floorHitCount = 0;

        for (int i = 0; i < 3; ++i) {
            float x = (_pos.x + _center.x - _scaledRadius / 2) + _scaledRadius / 2 * i;
            float y = _pos.y + _center.y + _scaledRadius * -1;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * -1);
            Debug.DrawRay(_ray.origin, _ray.direction * 0.05f);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, 0.05f, collisionMask2);
            if (_hit) {
                isTouchingFloor = true;
                entity.Grounded = true;
                floorHitCount++;
            }
        }
    }

    public void WallCheck() {
        _pos = transform.position;

        isTouchingWallLeft = false;
        isTouchingWallRight = false;
        leftHitCount = 0;
        rightHitCount = 0;

        // Check right first
        for (int i = 0; i < 3; ++i) {
            float x = _pos.x + _center.x + _scaledRadius;
            float y = (_pos.y + _center.y - _scaledRadius / 2) + _scaledRadius / 2 * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right);
            Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMask2);
            if (_hit) {
                rightHitCount++;
                isTouchingWallRight = true;
            }
        }

        // Check left second
        for (int i = 0; i < 3; ++i) {
            float x = _pos.x + _center.x + _scaledRadius * -1;
            float y = (_pos.y + _center.y - _scaledRadius / 2) + _scaledRadius / 2 * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * -1);
            Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMask2);
            if (_hit) {
                isTouchingWallLeft = true;
                leftHitCount++;
            }
        }
    }

    public bool IsTouchingAnything() {
        if (isTouchingFloor || isTouchingCeiling || isTouchingWallLeft || isTouchingWallRight) {
            return true;
        }

        return false;
    }

    void OnTriggerStay2D(Collider2D collider) {
        // If we are in a wall (layer 9)
        if (collider.gameObject.layer == 9) {
            // Figure out from which direction we are colliding
            int tempLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            int dir = -1;
            int hitCount = -1;
            // Right side check
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(_pos.x + _center.x + _scaledRadius, _pos.y), new Vector2(1, 0), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x + _center.x + _scaledRadius, _pos.y), new Vector2(1, 0));
            if(hit && hit.collider == collider) {
                dir = 0;
                hitCount = rightHitCount;
            }
            // left side check
            hit = Physics2D.Raycast(new Vector2(_pos.x + _center.x - _scaledRadius, _pos.y), new Vector2(-1, 0), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x + _center.x - _scaledRadius, _pos.y), new Vector2(-1, 0));
            if (hit && hit.collider == collider && hitCount < leftHitCount) {
                dir = 1;
                hitCount = leftHitCount;
            }
            // Bottom check
            hit = Physics2D.Raycast(new Vector2(_pos.x, _pos.y + _center.y - _scaledRadius), new Vector2(0, -1), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x, _pos.y + _center.y - _scaledRadius), new Vector2(0, -1));
            if (hit && hit.collider == collider && hitCount < floorHitCount) {
                dir = 2;
                hitCount = floorHitCount;
            }
            // Top check
            hit = Physics2D.Raycast(new Vector2(_pos.x, _pos.y + _center.y + _scaledRadius), new Vector2(0, 1), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x, _pos.y + _center.y + _scaledRadius), new Vector2(0, 1));
            if (hit && hit.collider == collider & hitCount < ceilingHitCount) {
                dir = 3;
                hitCount = ceilingHitCount;
            }

            gameObject.layer = tempLayer;

            /*
            int[] counts = { leftHitCount, rightHitCount, floorHitCount, ceilingHitCount };
            int highestCount = 0;
            int index = -1;
            for (int i = 0; i < 4; ++i) {
                if(counts[i] > highestCount) {
                    highestCount = counts[i];
                    index = i;
                }
            }
            */

            // Move out of it
            switch (dir) {
                case 0:
                    float curXPos = transform.position.x + _scaledRadius;
                    float wantXPos = collider.transform.position.x - (collider.GetComponent<BoxCollider2D>().size.x * collider.transform.localScale.x) / 2;
                    float xMove = wantXPos - curXPos;
                    if (Mathf.Abs(xMove) > _scaledRadius/4) {
                        MoveX(xMove);
                    }
                    break;
                case 1:
                    curXPos = transform.position.x - _scaledRadius;
                    wantXPos = collider.transform.position.x + (collider.GetComponent<BoxCollider2D>().size.x * collider.transform.localScale.x) / 2;
                    xMove = wantXPos - curXPos;
                    if (Mathf.Abs(xMove) > _scaledRadius/4) {
                        MoveX(xMove);
                    }
                    break;
                case 2:
                    float curYPos = transform.position.y - _scaledRadius;
                    float wantYPos = collider.transform.position.y + (collider.GetComponent<BoxCollider2D>().size.y * collider.transform.localScale.y) / 2;
                    float yMove = wantYPos - curYPos;
                    if (Mathf.Abs(yMove) > _scaledRadius/4) {
                        MoveY(yMove);
                    }
                    break;
                case 3:
                    curYPos = transform.position.y + _scaledRadius;
                    wantYPos = collider.transform.position.y - (collider.GetComponent<BoxCollider2D>().size.y * collider.transform.localScale.y) / 2;
                    yMove = wantYPos - curYPos;
                    if (Mathf.Abs(yMove) > _scaledRadius/4) {
                        MoveY(yMove);
                    }
                    break;
            }
        }
    }
}
