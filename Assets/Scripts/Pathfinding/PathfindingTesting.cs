using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using utilities;

public class PathfindingTesting : MonoBehaviour
{
    [SerializeField] private PathfindingVisual pathfindingVisual;
    [SerializeField] private PathfindingMovement pathfindingActor;

    [Header("Grid")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 10f;

    [Header("Actors")]
    [SerializeField] private int numberOfActors = 3;
    [SerializeField] private float minDistanceBetweenActors = 5f;

    [Header("Obstacles")]
    [SerializeField] private float obstacleDensity = 0.5f;

    private Transform pathfindingPrefab;
    private Pathfinding pathfinding;
    private Vector3 mousePosition;

    private List<Vector3Int> occupiedPositions;
    private Vector3Int randomPosition;
    private Vector3 gridPosition;
    private List<Transform> transformList;
    private Transform[,] transformArray;

    private List<PathNode> path;

    private void Start()
    {
        pathfinding = new Pathfinding(width, height, cellSize);
        SetupActors(pathfinding.GetGrid());
        GenerateObstacles(pathfinding.GetGrid());
        pathfindingVisual.SetGrid(pathfinding.GetGrid());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = MouseWorldPosition.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mousePosition, out int x, out int y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for(int i=0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x, path[i+1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
                }
            }

            pathfindingActor.SetTargetPosition(mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {

            mousePosition = MouseWorldPosition.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mousePosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }

    public void SetupActors(Grid<PathNode> grid)
    {
        transformArray = new Transform[grid.GetWidth(), grid.GetHeight()];
        occupiedPositions = new List<Vector3Int>();

        for (int i = 0; i < numberOfActors; i++)
        {
            do
            {
                randomPosition = new Vector3Int(Random.Range(0, grid.GetWidth()), Random.Range(0, grid.GetHeight()), 0);
            } while (IsTooCloseToOtherActors(randomPosition, occupiedPositions, minDistanceBetweenActors));

            occupiedPositions.Add(randomPosition);

            gridPosition = (Vector3)randomPosition * grid.GetCellSize() + .5f * grid.GetCellSize() * Vector3.one;
            Transform actorNode = CreateActor(gridPosition);
            transformArray[randomPosition.x, randomPosition.y] = actorNode;
            //transformList.Add(actorNode);
        }
    }

    private bool IsTooCloseToOtherActors(Vector3Int position, List<Vector3Int> occupiedPositions, float minDistance)
    {
        foreach (Vector3Int occupiedPos in occupiedPositions)
        {
            if (Vector3Int.Distance(position, occupiedPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }


    public void GenerateObstacles(Grid<PathNode> grid)
    {
        int totalNodes = grid.GetWidth() * grid.GetHeight();
        int numberOfObstacles = Mathf.RoundToInt(totalNodes * obstacleDensity);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            do
            {
                randomPosition = new Vector3Int(Random.Range(0, grid.GetWidth()), Random.Range(0, grid.GetHeight()), 0);
            } while (!pathfinding.GetNode(randomPosition.x, randomPosition.y).isWalkable || IsActorAtPosition(randomPosition, occupiedPositions));

            pathfinding.GetNode(randomPosition.x, randomPosition.y).SetIsWalkable(!pathfinding.GetNode(randomPosition.x, randomPosition.y).isWalkable);
        }
    }

    private bool IsActorAtPosition(Vector3Int position, List<Vector3Int> actorPositions)
    {
        foreach (Vector3Int actorPos in actorPositions)
        {
            if (actorPos == position)
            {
                return true;
            }
        }
        return false;
    }

    private Transform CreateActor(Vector3 position)
    {
        pathfindingPrefab = Instantiate(pathfindingActor.transform, position, Quaternion.identity);
        return pathfindingPrefab;
    }
}
