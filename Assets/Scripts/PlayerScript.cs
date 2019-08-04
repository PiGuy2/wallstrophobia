using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    public float speed = 5f;
    public bool gridMove = true;
    public Vector2 gridSpace = new Vector2(2.0f, 2.0f);
    public Vector2Int gridSize = new Vector2Int(12, 12);
    public Vector2 gridOrigin = new Vector2(2.0f, 2.0f);
    public float gridMoveTime = 0.2f;

    public GameObject wallPrefab;
    
    private Rigidbody2D playerRb;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 velocity;
    private Vector2 facing = new Vector2(1, 0);

    private bool collisionDetected = false;
    private bool enemyTurn = false;

    private Transform highlight;

    // Start is called before the first frame update
    void Start () {
        playerRb = this.gameObject.GetComponent<Rigidbody2D>();

        startPos = playerRb.position;
        endPos = playerRb.position;

        highlight = gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        float lH = Input.GetAxisRaw("Look Horizontal");
        float lV = Input.GetAxisRaw("Look Vertical");

        // Change which direction character is looking
        if ((lH == 0) ^ (lV == 0)) {
            facing = (new Vector2(lH, lV)).normalized;
        }

        // See if square character is looking at is in the grid
        Vector2Int facingLoc = CoordinatesToGridLocation(playerRb.position + Vector2.Scale(facing, gridSpace));
        Vector2Int facingLocClamped = facingLoc;
        facingLocClamped.Clamp(new Vector2Int(0, 0), gridSize - new Vector2Int(1, 1));
        if (facingLoc != facingLocClamped) {
            highlight.gameObject.SetActive(false);
        } else {
            highlight.gameObject.SetActive(true);
        }

        bool turn = false;
        if (gridMove) {
            if (enemyTurn) {
                Debug.Log("Enemy Turn");
                enemyTurn = false;
            } else if (collisionDetected) {
                // If player ran into a wall
                endPos = startPos;
                collisionDetected = false;
            }
            if (playerRb.position != endPos) {
                // If player is moving
                playerRb.position = Vector2.MoveTowards(playerRb.position, endPos, 0.2f);
            } else if (Input.GetButtonDown("Place Wall") && highlight.gameObject.activeSelf) {
                // If player is placing wall
                Vector2 wallPos = playerRb.position + Vector2.Scale(facing, gridSpace);
                wallPos += new Vector2(-0.11f, 0.72f);
                Instantiate(wallPrefab, wallPos, new Quaternion());
                turn = true;
            } else if (v != 0 || h != 0) {
                // If player is starting a motion
                Vector2 move;
                if (v != 0) {
                    move = new Vector2(0f, v);
                } else {
                    move = new Vector2(h, 0f);
                }
                move.Normalize();
                move = Vector2.Scale(move, gridSpace);

                startPos = playerRb.position;
                endPos = startPos + move;
                facing = (endPos - startPos).normalized;
                turn = true;
            } else {
                // Do nothing
            }

            if (turn) {
                enemyTurn = true;
            }
        } else {
            playerRb.velocity = new Vector2(h * speed, v * speed);
        }

        highlight.localPosition = new Vector2(0.1535f, -0.463f) + Vector2.Scale(facing, gridSpace);
    }

    void OnCollisionEnter2D (Collision2D collision) {
        collisionDetected = true;
    }

    Vector2Int GetPlayerLocation () {
        return CoordinatesToGridLocation(gameObject.transform.position);
    }

    Vector2Int CoordinatesToGridLocation (Vector2 coords) {
        Vector2 scaleFactor = new Vector2(1f / gridSpace.x, 1f / gridSpace.y);
        Vector2 pos = Vector2.Scale(coords, scaleFactor);
        pos -= Vector2.Scale(gridOrigin, scaleFactor);
        return Vector2Int.RoundToInt(pos);
    }
}
