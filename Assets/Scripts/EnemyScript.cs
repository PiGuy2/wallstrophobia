using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
    public bool busy = false;
    public bool canGoThroughWalls = false;
    public bool canExplode = false;

    private bool fuseLit = false;
    private int fuseLength = 1;

    private PlayerScript playerScript;
    private Rigidbody2D enemyRb;

    private Vector2 endPos;

    // Start is called before the first frame update
    void Start () {
        enemyRb = gameObject.GetComponent<Rigidbody2D>();

        endPos = enemyRb.position;
    }

    void LateUpdate () {
        if (fuseLength == 0) {
            GetComponent<Animator>().SetBool("Detonated", true);
            Vector2Int loc = GetEnemyLocation();
            for (int x = loc.x - 1; x <= loc.x + 1; x++) {
                for (int y = loc.y - 1; y <= loc.y + 1; y++) {
                    for (int i = playerScript.wallLocations.Count - 1; i >= 0; i--) {
                        Vector2Int wallLoc = playerScript.wallLocations[i];
                        if (wallLoc == new Vector2Int(x, y)) {
                            playerScript.wallLocations.Remove(wallLoc);
                        }
                    }
                    for (int i = playerScript.walls.Count - 1; i >= 0; i--) {
                        GameObject wall = playerScript.walls[i];
                        if (playerScript.CoordinatesToGridLocation(wall.transform.position) == new Vector2Int(x, y)) {
                            playerScript.walls.Remove(wall);
                            wall.GetComponent<DetonateScript>().TriggerDetonate();
                        }
                    }
                    for (int i = playerScript.enemies.Count - 1; i >= 0; i--) {
                        EnemyScript enemy = playerScript.enemies[i];
                        if (enemy.GetEnemyLocation() == new Vector2Int(x, y)) {
                            playerScript.enemies.Remove(enemy);
                            enemy.gameObject.GetComponent<DetonateScript>().TriggerDetonate();
                        }
                    }
                }
            }
        }
    }

    void Update () {
        if (enemyRb.position != endPos) {
            enemyRb.position = Vector2.MoveTowards(enemyRb.position, endPos, playerScript.moveSpeed);
        } else {
            busy = false;
        }

        GetComponent<SpriteRenderer>().sortingLayerID = playerScript.GetSortingLayer(GetEnemyLocation().y);
    }
    
    public void SetPS (PlayerScript pS) {
        playerScript = pS;
    }

    public void DoMove () {
        busy = true;

        Vector2 moveDirection = playerScript.GetPlayerLocation() - GetEnemyLocation();
        float xDist = Mathf.Abs(moveDirection.x);
        float yDist = Mathf.Abs(moveDirection.y);

        if (canExplode && moveDirection.sqrMagnitude <= 2) {
            fuseLit = true;
            GetComponent<Animator>().SetBool("Fused", true);
            return;
        }

        if (fuseLit) {
            fuseLength--;
        }

        List<Vector2Int> walls = GetNearbyWalls();
        int highScore = 0;
        Vector2Int scoreDirection = new Vector2Int(0, 0);

        Vector2Int[] possibleMovements = {new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)};
        foreach (Vector2Int current in possibleMovements) {
            int currentScore = 0;
            if (walls.Contains(current) && !canGoThroughWalls) {
                continue;
            }
            Vector2Int newLoc = GetEnemyLocation() + current;
            Vector2Int newLocClamped = newLoc;
            newLocClamped.Clamp(new Vector2Int(0, 0), playerScript.gridSize - new Vector2Int(1, 1));
            if (newLoc != newLocClamped) {
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
