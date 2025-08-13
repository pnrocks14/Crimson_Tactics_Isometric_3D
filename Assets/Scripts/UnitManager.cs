using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private Dictionary<Vector2Int, GameObject> unitPositions = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterUnit(GameObject unit, int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        RemoveUnit(unit);
        unitPositions[pos] = unit;
    }

    public void RemoveUnit(GameObject unit)
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (var kvp in unitPositions)
        {
            if (kvp.Value == unit)
                keysToRemove.Add(kvp.Key);
        }
        foreach (var key in keysToRemove)
            unitPositions.Remove(key);
    }

    public bool IsOccupied(int x, int z, GameObject exclude = null)
    {
        Vector2Int pos = new Vector2Int(x, z);
        return unitPositions.ContainsKey(pos) && unitPositions[pos] != exclude;
    }
}
