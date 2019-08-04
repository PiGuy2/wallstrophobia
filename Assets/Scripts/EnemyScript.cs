using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
    public bool busy = false;

    private PlayerScript playerScript;
    private Rigidbody2D enemyRb;

    private Vector2 endPos;

    // Start is called before the first frame update
    void Start () {
        enemyRb = gameObject.GetComponent<Rigidbody2D>();

        endPos = enemyRb.position;
    }

    // Update is called once per frame
    void Update () {
        if (enemyRb.position != endPos) {
            enemyRb.position = Vector2.MoveTowards(enemyRb.position, endPos, playerScript.moveSpeed);
        } else {
            busy = false;
        }
    }
    
    public void SetPS (PlayerScript pS) {
        playerScript = pS;
    }

    public void DoMove () {
        busy = true;

        Vector2 moveDirection = playerScript.GetPlayerLocation() - GetEnemyLocation();
        float xDist = Mathf.Abs(moveDirection.x);
        float yDist = Mathf.Abs(moveDirection.y);

        List<Vector2Int> walls = GetNearbyWalls();
        int highScore = 0;
        Vector2Int scoreDirection = new Vector2Int(0, 0);

        Vector2Int[] possibleMovements = {new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)};
        foreach (Vector2Int current in possibleMovements) {
            int currentScore = 0;
            if (walls.Contains(current)) {
                continue;
            }
            if ((moveDirection - current).sqrMagnitude < moveDirection.sqrMagnitude) {
                currentScore = 3;
            } else if ((xDist == 0 && current.y == 0) || (yDist == 0 && current.x == 0)) {
                currentScore = 2;
            } else {
                currentScore = 1;
            }
            if (currentScore > highScore || (currentScore == highScore && Random.Range(0, 2) == 0)) {
                scoreDirection = current;
                highScore = currentScore;
            }
        }

        endPos = enemyRb.position + Vector2.Scale(scoreDirection, playerScript.gridSpace);
    }

    public Vector2Int GetEnemyLocation () {
        return playerScript.CoordinatesToGridLocation(gameObject.transform.position - new Vector3(2.0f, 2.0f, 0));
    }

    List<Vector2Int> GetNearbyWalls () {
        List<Vector2Int> wallLocs = new List<Vector2Int>();
        Vector2Int loc = GetEnemyLocation();
        foreach (Vector2Int wallLoc in playerScript.wallLocations) {
            Vector2Int diff = wallLoc - loc;
            if ((Mathf.Abs(diff.x) == 1) && (Mathf.Abs(diff.y) == 0)) {
                wallLocs.Add(diff);
            }
            if ((Mathf.Abs(diff.x) == 0) && (Mathf.Abs(diff.y) == 1)) {
                wallLocs.Add(diff);
            }
        }
        return wallLocs;
    }
    
}
