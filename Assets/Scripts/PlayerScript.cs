using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    public float speed = 5f;
    public bool gridMove = true;
    public Vector2 gridSize = new Vector2(2.0f, 2.0f);
    public float gridMoveTime = 0.2f;

    public GameObject wallPrefab;
    
    private Rigidbody2D playerRb;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 velocity;
    private Vector2 facing = new Vector2(1, 0);

    private bool collisionDetected = false;

    // Start is called before the first frame update
    void Start () {
        playerRb = this.gameObject.GetComponent<Rigidbody2D>();

        startPos = playerRb.position;
        endPos = playerRb.position;
    }

    // Update is called once per frame
    void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (gridMove) {
            if (collisionDetected) {
                endPos = startPos;
                collisionDetected = false;
            }
            if (playerRb.position != endPos) {
                // Vector2.SmoothDamp(playerRb.position, endPos, ref velocity, gridMoveTime);
                // playerRb.velocity = velocity;
                playerRb.position = Vector2.MoveTowards(playerRb.position, endPos, 0.2f);
                // if (Vector2.Distance(playerRb.position, endPos) < 0.05f) {
                //     playerRb.position = endPos;
                // }
            } else if (Input.GetButton("Fire")) {
                Vector2 wallPos = playerRb.position + Vector2.Scale(facing, gridSize);
                wallPos += new Vector2(-0.36f, 0.72f);
                Instantiate(wallPrefab, wallPos, new Quaternion());
            } else if (v != 0 || h != 0) {
                Vector2 move;
                if (v != 0) {
                    move = new Vector2(0f, v);
                } else {
                    move = new Vector2(h, 0f);
                }
                move.Normalize();
                move = Vector2.Scale(move, gridSize);

                startPos = playerRb.position;
                endPos = startPos + move;
                facing = (endPos - startPos).normalized;
                // A turn occurred
            } else {
                // Do nothing
            }
        } else {
            playerRb.velocity = new Vector2(h * speed, v * speed);
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        collisionDetected = true;
    }
}
