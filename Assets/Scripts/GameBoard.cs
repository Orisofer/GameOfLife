using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap m_CurrentState;
    [SerializeField] private Tilemap m_NextState;
    [SerializeField] private Tile m_AliveTile;
    [SerializeField] private Tile m_DeadTile;
    [SerializeField] private Pattern m_Pattern;
    [SerializeField] private float m_UpdateInterval;

    private WaitForSeconds m_YieldTime;

    private HashSet<Vector3Int> m_AliveCells;
    private HashSet<Vector3Int> m_CellsToCheck;

    private Vector3Int CELL_UP = new Vector3Int(1, 0);
    private Vector3Int CELL_DOWN = new Vector3Int(-1, 0);
    private Vector3Int CELL_TOP_RIGHT = new Vector3Int(1, 1);
    private Vector3Int CELL_CENTER_RIGHT = new Vector3Int(0, 1);
    private Vector3Int CELL_BOTTOM_RIGHT = new Vector3Int(-1, 1);
    private Vector3Int CELL_TOP_LEFT = new Vector3Int(-1, 1);
    private Vector3Int CELL_CENTER_LEFT = new Vector3Int(-1, 0);
    private Vector3Int CELL_BOTTOM_LEFT = new Vector3Int(-1, -1);


    private void Awake()
    {
        m_YieldTime = new WaitForSeconds(m_UpdateInterval);

        m_AliveCells = new HashSet<Vector3Int>();
        m_CellsToCheck = new HashSet<Vector3Int>();
    }

    private void Start()
    {
        SetPattern(m_Pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        ClearBoard();

        Vector2Int center = pattern.GetCenter();

        for (int i = 0; i < m_Pattern.m_Cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.m_Cells[i] - center);
            m_CurrentState.SetTile(cell, m_AliveTile);
            m_AliveCells.Add(cell);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SimulateCo());
    }

    private void ClearBoard()
    {
        m_CurrentState.ClearAllTiles();
        m_NextState.ClearAllTiles();
    }

    private IEnumerator SimulateCo()
    {
        while(enabled)
        {
            UpdateState();
            yield return m_YieldTime;
        }
    }

    private void UpdateState()
    {
        AddCellsToCheck();
        CheckNeighbors();
        SwapStates();
    }

    private void AddCellsToCheck()
    {
        m_CellsToCheck.Clear();

        foreach (Vector3Int cell in m_AliveCells)
        {
            m_CellsToCheck.Add(cell);

            m_CellsToCheck.Add(cell + CELL_UP);
            m_CellsToCheck.Add(cell + CELL_DOWN);
            m_CellsToCheck.Add(cell + CELL_TOP_RIGHT);
            m_CellsToCheck.Add(cell + CELL_CENTER_RIGHT);
            m_CellsToCheck.Add(cell + CELL_BOTTOM_RIGHT);
            m_CellsToCheck.Add(cell + CELL_TOP_LEFT);
            m_CellsToCheck.Add(cell + CELL_CENTER_LEFT);
            m_CellsToCheck.Add(cell + CELL_BOTTOM_LEFT);
        }
    }

    private void CheckNeighbors()
    {
        foreach (Vector3Int cell in m_CellsToCheck)
        {
            int aliveNeighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && aliveNeighbors == 3)
            {
                m_NextState.SetTile(cell, m_AliveTile);
                m_AliveCells.Add(cell);
            }
            else if (alive && aliveNeighbors < 2 || aliveNeighbors > 3)
            {
                m_NextState.SetTile(cell, m_DeadTile);
                m_AliveCells.Remove(cell);
            }
            else
            {
                m_NextState.SetTile(cell, m_CurrentState.GetTile(cell));
            }
        }
    }

    private int CountNeighbors(Vector3Int cell)
    {
        int result = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);

                if (x == 0 && y == 0) continue;

                if(IsAlive(neighbor))
                {
                    result++;
                }
            }
        }

        return result;
    }

    private void SwapStates()
    {
        Tilemap oldMap = m_CurrentState;
        m_CurrentState = m_NextState;
        m_NextState = oldMap;

        m_NextState.ClearAllTiles();
    }

    private bool IsAlive(Vector3Int cell)
    {
        return m_CurrentState.GetTile(cell) == m_AliveTile;
    }
}
