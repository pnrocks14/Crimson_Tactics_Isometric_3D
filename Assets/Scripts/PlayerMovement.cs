using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For legacy UI Text

public class PlayerMovement : MonoBehaviour
{
    public static System.Action OnPlayerMoveComplete;
    [SerializeField] private ObstacleData obstacleData;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource click;

    [Header("Game Over UI")]
    [SerializeField] private Text gameOverText; // legacy UI Text

    private int currentX = 0;
    private int currentZ = 0;
    private bool isMoving = false;
    private List<Vector3> path = new List<Vector3>();

    void Start()
    {
        UnitManager.Instance?.RegisterUnit(gameObject, currentX, currentZ);

        if (gameOverText != null)
            gameOverText.enabled = false; // Hide game over text initially
    }

    void Update()
    {
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            if (click != null) click.Play();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                int targetX = Mathf.RoundToInt(hit.point.x);
                int targetZ = Mathf.RoundToInt(hit.point.z);

                if (IsInsideGrid(targetX, targetZ) && !IsObstacle(targetX, targetZ))
                {
                    path = FindPath(currentX, currentZ, targetX, targetZ);
                    if (path != null && path.Count > 0)
                        StartCoroutine(MoveAlongPath(path));
                }
            }
        }
    }

    public bool HasAvailableMoves()
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        foreach (var dir in directions)
        {
            int nx = currentX + dir.x;
            int nz = currentZ + dir.y;
            if (IsInsideGrid(nx, nz) && !IsObstacle(nx, nz))
                return true;
        }
        return false;
    }

    bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && x < 10 && z >= 0 && z < 10;
    }

    bool IsObstacle(int x, int z)
    {
        if (obstacleData == null) return false;
        bool staticObstacle = obstacleData.obstacles[x].row[z];
        bool unitOccupied = UnitManager.Instance != null && UnitManager.Instance.IsOccupied(x, z, gameObject);
        return staticObstacle || unitOccupied;
    }

    List<Vector3> FindPath(int startX, int startZ, int targetX, int targetZ)
    {
        Queue<Vector3> queue = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Vector3 start = new Vector3(startX, 0, startZ);
        Vector3 target = new Vector3(targetX, 0, targetZ);

        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector3[] directions = {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1), new Vector3(0, 0, -1)
        };

        while (queue.Count > 0)
        {
            Vector3 current = queue.Dequeue();
            if (current == target) break;

            foreach (var dir in directions)
            {
                Vector3 neighbor = current + dir;
                int nx = (int)neighbor.x;
                int nz = (int)neighbor.z;

                if (IsInsideGrid(nx, nz) && !IsObstacle(nx, nz) && !cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(target))
            return null;

        List<Vector3> finalPath = new List<Vector3>();
        Vector3 currentPathTile = target;

        while (currentPathTile != start)
        {
            finalPath.Insert(0, currentPathTile);
            currentPathTile = cameFrom[currentPathTile];
        }

        return finalPath;
    }

    IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;
        if (animator != null) animator.SetBool("IsWalking", true);

        foreach (var step in path)
        {
            int newX = (int)step.x;
            int newZ = (int)step.z;

            if (UnitManager.Instance != null && UnitManager.Instance.IsOccupied(newX, newZ, gameObject))
            {
                break;
            }

            Vector3 direction = (new Vector3(step.x, 0.9f, step.z) - transform.position).normalized;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            Vector3 targetPos = new Vector3(step.x, 0.9f, step.z);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 5f);
                yield return null;
            }

            currentX = newX;
            currentZ = newZ;
            UnitManager.Instance?.RegisterUnit(gameObject, currentX, currentZ);
        }

        if (animator != null) animator.SetBool("IsWalking", false);
        isMoving = false;
        OnPlayerMoveComplete?.Invoke();

        if (!HasAvailableMoves())
        {
            EndGameOutOfRoom();
        }
    }

    private void EndGameOutOfRoom()
    {
        Debug.Log("Game Over: Out of Room!");
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over: Out of Room!";
            gameOverText.enabled = true;
        }
        Time.timeScale = 0f;
    }
}
