using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
    public float moveSpeed = 0.3f;
    public Vector2 gridSpace = new Vector2(2.0f, 2.0f);
    public Vector2Int gridSize = new Vector2Int(12, 12);
    public Vector2 gridOrigin = new Vector2(2.0f, 2.0f);
    public float gridMoveTime = 0.2f;

    public GameObject wallPrefab;
    public GameObject basicEnemyPrefab;
    public GameObject gahstEnemyPrefab;
    public GameObject bombEnemyPrefab;
    
    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 velocity;
    private Vector2Int facing = new Vector2Int(1, 0);

    private bool collisionDetected = false;
    private bool death = false;
    private float deathTime;

    private bool enemyTurn = true;
    private int enemyTurnStep = 0;

    private Transform highlight;

    public int gameScore;

    public List<Vector2Int> wallLocations;
    public List<GameObject> walls;
    public List<EnemyScript> enemies;

    private int initialSpawns = 2;

    // Start is called before the first frame update
    void Start () {
        playerRb = this.gameObject.GetComponent<Rigidbody2D>();
        playerAnimator = this.gameObject.GetComponent<Animator>();

        startPos = playerRb.position;
        endPos = playerRb.position;

        highlight = gameObject.transform.GetChild(0);

        wallLocations = new List<Vector2Int>();
        enemies = new List<EnemyScript>();

        gameScore = 0;
    }

    // Update is called once per frame
    void Update () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        float lH = Input.GetAxisRaw("Look Horizontal");
        float lV = Input.GetAxisRaw("Look Vertical");

        GetComponent<SpriteRenderer>().sortingLayerID = GetSortingLayer(GetPlayerLocation().y);

        // Change which direction character is looking
        if ((lH == 0) ^ (lV == 0)) {
            facing = Vector2Int.CeilToInt((new Vector2(lH, lV)).normalized);
        }

        // See if square character is looking at is in the grid
        Vector2Int facingLoc = GetPlayerLocation() + facing;
        Vector2Int facingLocClamped = facingLoc;
        facingLocClamped.Clamp(new Vector2Int(0, 0), gridSize - new Vector2Int(1, 1));
        if (facingLoc != facingLocClamped || wallLocations.Contains(facingLoc)) {
            highlight.gameObject.SetActive(false);
        } else {
            highlight.gameObject.SetActive(true);
        }

        bool turn = false;
        if (Input.GetKeyDown("r")) {
            SceneManager.LoadScene("Main");
        } else if (death) {
            if (Time.time > deathTime + 5) {
                SceneManager.LoadScene("Main");
            }
        } else if (enemyTurn) {
            if (enemyTurnStep == 0) {
                foreach (EnemyScript enemy in enemies) {
                    enemy.DoMove();
                }
                enemyTurnStep++;
            } else if (enemyTurnStep == 1) {
                // Check if enemies are done
                bool allDone = true;
                foreach (EnemyScript enemy in enemies) {
                    if (enemy.busy) {
                        allDone = false;
                        break;
                    }
                }
                if (allDone) {
                    enemyTurnStep++;
                }
            } else {
                // Spawn a new enemy
                // Starts at 1/3 and reaches 2/1 @ 50 kills
                if (Random.Range(0, Mathf.Max(10, 60 - gameScore)) < 10 || initialSpawns > 0) {
                    Debug.Log("Spawning");
                    List<Vector2Int> spawnLocations = new List<Vector2Int>();
                    Vector2Int playerLoc = GetPlayerLocation();
                    for (int x = 0; x < gridSize.x; x++) {
                        for (int y = 0; y < gridSize.y; y++) {
                            if (Mathf.Abs(x - playerLoc.x) + Mathf.Abs(y - playerLoc.y) > 2) {
                                if (!wallLocations.Contains(new Vector2Int(x, y))) {
                                    spawnLocations.Add(new Vector2Int(x, y));
                                }
                            }
                        }
                    }
                    Vector2Int enemyLocation = spawnLocations[Random.Range(0, spawnLocations.Count)];
                    Vector2 enemyPos = GridLocationToCoordinates(enemyLocation) + new Vector2(1.1f, 1.12f);
                    
                    EnemyScript newEnemy;

                    if (Random.Range(0, Mathf.Max(45, 90 - Mathf.Max(gameScore - 15, 0))) < 11 && gameScore > 5) {
                        // Starts at 1/4. Starts increasing @ 15 kills, maxes out at 1/2 @ 60 kills
                        newEnemy = Instantiate(gahstEnemyPrefab, enemyPos, new Quaternion()).GetComponent<EnemyScript>();
                    } else if (Random.Range(0, Mathf.Max(40, 100 - Mathf.Max(gameScore - 20, 0))) < 5 || gameScore > 1) {
                        // Starts at 1/10. Starts increasing @ 20 kills, maxes out at 1/4 @ 80 kills
                        newEnemy = Instantiate(bombEnemyPrefab, enemyPos, new Quaternion()).GetComponent<EnemyScript>();
                    } else {
                        newEnemy = Instantiate(basicEnemyPrefab, enemyPos, new Quaternion()).GetComponent<EnemyScript>();
                    }

                    newEnemy.SetPS(this);
                    enemies.Add(newEnemy);
                    initialSpawns--;
                }
                enemyTurnStep++;
                if (enemyTurnStep > 3) {
                    enemyTurnStep = 0;
                    enemyTurn = false;
                }
            }
        } else if (collisionDetected) {
            // If player ran into a wall
            endPos = startPos;
            collisionDetected = false;
        } else if (playerRb.position != endPos) {
            // If player is moving
            playerRb.position = Vector2.MoveTowards(playerRb.position, endPos, moveSpeed);
            if (playerRb.position == endPos) {
                turn = true;
            }
        } else if (Input.GetButtonDown("Place Wall") && highlight.gameObject.activeSelf) {
            // If player is placing wall
            Vector2Int wallLoc = GetPlayerLocation() + facing;
            wallLocations.Add(wallLoc);
            Vector2 wallPos = GridLocationToCoordinates(wallLoc);
            wallPos += new Vector2(0.84f, 2.4f);
            
            GameObject wall = Instantiate(wallPrefab, wallPos, new Quaternion());
            wall.GetComponent<SpriteRenderer>().sortingLayerID = GetSortingLayer(wallLoc.y);
            walls.Add(wall);

            for (int i = enemies.Count - 1; i >= 0; i--) {
                EnemyScript enemy = enemies[i];
                if (enemy.GetEnemyLocation() == wallLoc) {
                    if (enemy.canGoThroughWalls) {
                        gameScore++;
                    }
                    if (enemy.canExplode) {
                        gameScore += 2;
                    }
                    enemies.Remove(enemy);
                    GameObject.Destroy(enemy.gameObject);
                    gameScore += 1;
                }
            }

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
            facing = Vector2Int.CeilToInt((endPos - startPos).normalized);
        } else {
            // Do nothing
        }

        if (turn) {
            enemyTurn = true;
        }

        highlight.localPosition = new Vector2(0.1535f, -0.463f) + Vector2.Scale(facing, gridSpace);

        if (gameScore == 0) {
            GameObject.Find("Score Text").GetComponent<Text>().text = "No Kills Yet";
        } else if (gameScore == 1) {
            GameObject.Find("Score Text").GetComponent<Text>().text = "1 Point";
        } else {
            GameObject.Find("Score Text").GetComponent<Text>().text = gameScore + " Points";
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        collisionDetected = true;
    }

    void OnTriggerEnter2D (Collider2D trigger) {
        playerAnimator.SetBool("Death", true);
        deathTime = Time.time;
        death = true;
    }

    public Vector2Int GetPlayerLocation () {
        return CoordinatesToGridLocation(gameObject.transform.position - new Vector3(0, 1, 0));
    }

    public Vector2Int CoordinatesToGridLocation (Vector2 coords) {
        Vector2 scaleFactor = new Vector2(1f / gridSpace.x, 1f / gridSpace.y);
        Vector2 pos = Vector2.Scale(coords, scaleFactor);
        pos -= Vector2.Scale(gridOrigin, scaleFactor);
        return Vector2Int.RoundToInt(pos);
    }

    public Vector2 GridLocationToCoordinates (Vector2Int loc) {
        Vector2 pos = Vector2.Scale(loc, gridSpace);
        pos += gridOrigin;
        return pos;
    }

    public int GetSortingLayer (int row) {
        return SortingLayer.NameToID("Row " + row);
    }

    void OnDrawGizmos () {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        float size = 0.2f;
        Gizmos.DrawCube(gridOrigin, new Vector3(size, size, size));
    }
}
