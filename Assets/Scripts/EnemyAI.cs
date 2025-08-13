using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, AI
{
    [SerializeField] private ObstacleData obstacleData;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;

    private int currentX;
    private int currentZ;
    private bool isMoving;
    private List<Vector3> path = new List<Vector3>();

    private void OnEnable()
    {
        PlayerMovement.OnPlayerMoveComplete += HandlePlayerMoveComplete;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerMoveComplete -= HandlePlayerMoveComplete;
    }

    void Start()
    {
        currentX = Mathf.RoundToInt(transform.position.x);
        currentZ = Mathf.RoundToInt(transform.position.z);

        if (UnitManager.Instance != null)
            UnitManager.Instance.RegisterUnit(gameObject, currentX, currentZ);
    }

    public void UpdateAI()
    {
        // Optional for event-driven AI
    }

    private void HandlePlayerMoveComplete()
    {
        if (isMoving) return;

        Vector2Int playerPos = new Vector2Int(
            Mathf.RoundToInt(playerTransform.position.x),
            Mathf.RoundToInt(playerTransform.position.z)
        );

        Vector2Int enemyPos = new Vector2Int(currentX, currentZ);
        if (IsAdjacent(enemyPos, playerPos)) return;

        Vector2Int targetAdjacent = GetAdjacentToPlayer(playerPos);

        if (targetAdjacent != Vector2Int.zero)
        {
            path = FindPath(currentX, currentZ, targetAdjacent.x, targetAdjacent.y);
            if (path != null && path.Count > 0)
                StartCoroutine(MoveAlongPath(path));
        }
    }

    bool IsAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) == 1;
    }

    Vector2Int GetAdjacentToPlayer(Vector2Int playerPos)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int adjacent = playerPos + dir;
            if (IsInsideGrid(adjacent.x, adjacent.y) && !IsObstacle(adjacent.x, adjacent.y))
                return adjacent;
        }
        return Vector2Int.zero;
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
        if (IsObstacle(targetX, targetZ)) return null;

        Queue<Vector3> queue = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Vector3 start = new Vector3(startX, 0, startZ);
        Vector3 target = new Vector3(targetX, 0, targetZ);

        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector3[] directions = { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

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

        if (!cameFrom.ContainsKey(target)) return null;

        List<Vector3> finalPath = new List<Vector3>();
        Vector3 tile = target;
        while (tile != start)
        {
            finalPath.Insert(0, tile);
            tile = cameFrom[tile];
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

            Vector3 targetPos = new Vector3(step.x, 0.9f, step.z);
            Vector3 direction = (targetPos - transform.position).normalized;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

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
    }
}
