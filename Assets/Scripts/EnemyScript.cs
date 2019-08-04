using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
    public bool busy = false;

    private PlayerScript playerScript;
    private Rigidbody2D enemyRb;

    private Vector2 endPos;

    private int count = 0;

    // Start is called before the first frame update
    void Start () {
        enemyRb = gameObject.GetComponent<Rigidbody2D>();

        endPos = enemyRb.position;
    }

    // Update is called once per frame
    void Update () {
        if (enemyRb.position != endPos) {
            enemyRb.position = Vector2.MoveTowards(enemyRb.position, endPos, 0.2f);
        } else {
            busy = false;
        }
    }
    
    public void SetPS (PlayerScript pS) {
        playerScript = pS;
    }

    public void DoMove () {
        Vector2 moveDirection = playerScript.GetPlayerLocation() - GetEnemyLocation();
        float xDist = Mathf.Abs(moveDirection.x);
        float yDist = Mathf.Abs(moveDirection.y);
        if (xDist > yDist) {
            moveDirection.y = 0;
        } else if (xDist < yDist) {
            moveDirection.x = 0;
        } else {
            if (Random.Range(0, 2) == 0) {
                moveDirection.x = 0;
            } else {
                moveDirection.y = 0;
            }
        }
            moveDirection.Normalize();
        endPos = enemyRb.position + Vector2.Scale(moveDirection, playerScript.gridSpace);
        busy = true;
    }

    public Vector2Int GetEnemyLocation () {
        return playerScript.CoordinatesToGridLocation(gameObject.transform.position - new Vector3(2.0f, 2.0f, 0));
    }
}
