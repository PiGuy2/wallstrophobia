using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    public float speed = 5f;
    public bool gridMove = true;
    public float gridSpace = 1.5f;
    public float gridMoveTime = 0.2f;
    
    private Rigidbody2D playerRb;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 velocity;

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
            if (playerRb.position != endPos) {
                // Vector2.SmoothDamp(playerRb.position, endPos, ref velocity, gridMoveTime);
                // playerRb.velocity = velocity;
                playerRb.position = Vector2.MoveTowards(playerRb.position, endPos, 0.2f);
                if (Vector2.Distance(playerRb.position, endPos) < 0.05f) {
                    playerRb.position = endPos;
                }
            } else if (v != 0 || h != 0) {
                Vector2 move;
                if (v != 0) {
                    move = new Vector2(0f, v);
                } else {
                    move = new Vector2(h, 0f);
                }
                move.Normalize();
                move = Vector2.Scale(move, new Vector2(gridSpace, gridSpace));

                startPos = playerRb.position;
                endPos = startPos + move;
                // A turn occurred
            } else {
                // Do nothing
            }
        } else {
            playerRb.velocity = new Vector2(h * speed, v * speed);
        }
    }
}
