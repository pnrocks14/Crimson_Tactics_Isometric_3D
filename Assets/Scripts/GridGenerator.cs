using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int gridSize = 10;
    [SerializeField] private TextMeshProUGUI text;


    // 2d gameobject array
    private GameObject[,] grid;
    private GameObject lastHighlight;


    void Start()
    {
        //allocate the array
        grid = new GameObject[gridSize, gridSize];
        GenerateGrid();

    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
                tile.name = $"Cube_{x}_{z}";//set the name usingg string interpolation

                tile.AddComponent<TileInfo>().SetPosition(x, z);//dynamically add tileinfo and set pos of the tile
                tile.transform.parent = transform;//seting this tile's parent to the gridGenerator's transform
                grid[x, z] = tile;//storing it

                Transform hoverChild = tile.transform.Find("hoverTile");
                if (hoverChild != null)
                {
                    hoverChild.gameObject.SetActive(false);
                }

            }
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//takes the 2d point input and converts it in 3d ray from camera
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))//checking if the ray hit something
        {
            if (hit.collider != null)
            {
                TileInfo tile = hit.collider.GetComponent<TileInfo>();
                if (tile != null)
                {
                    text.SetText($"Grid Position: ({tile.GetX()}, {tile.GetZ()})");//set the text
                    // text.SetText($"Grid Position: ({tile.GetX() + 1}, {tile.GetZ() + 1})");//uncomment this line if you want 1 based indexing

                    if (lastHighlight != null)
                    {
                        lastHighlight.SetActive(false);
                    }

                    // Show current highlight
                    Transform hoverChild = tile.transform.Find("hoverTile");
                    if (hoverChild != null)
                    {
                        hoverChild.gameObject.SetActive(true);
                        lastHighlight = hoverChild.gameObject;
                    }
                }
            }
        }
        else
        {
            //hide last highlight when not hoverring any tile
            if (lastHighlight != null)
            {
                lastHighlight.SetActive(false);
                lastHighlight = null;
            }
        }
    }
}




