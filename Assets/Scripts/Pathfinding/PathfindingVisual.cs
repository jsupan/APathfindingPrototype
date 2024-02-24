using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utilities;

public class PathfindingVisual : MonoBehaviour
{
    private Grid<PathNode> grid;
    private Mesh mesh;
    private bool updateMesh;

    private int updateIndex;
    private Vector3 quadsize;
    private PathNode gridObject;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void SetGrid(Grid<PathNode> grid)
    {
        this.grid = grid;
        UpdatePathfindingVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<PathNode>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdatePathfindingVisual();
        }
    }

    private void UpdatePathfindingVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                updateIndex = x * grid.GetHeight() + y;
                quadsize = new Vector3(1, 1) * grid.GetCellSize();
                gridObject = grid.GetGridObject(x, y);

                if (gridObject.isWalkable)
                {
                    quadsize = Vector3.zero;
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, updateIndex, grid.GetWorldPosition(x, y) + quadsize * 0.5f, 0f, quadsize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
