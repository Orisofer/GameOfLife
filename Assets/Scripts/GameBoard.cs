using System.Collections;
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

    private void Awake()
    {
        m_YieldTime = new WaitForSeconds(m_UpdateInterval);
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
            Vector2Int cell = pattern.m_Cells[i] - center;
            m_CurrentState.SetTile((Vector3Int)cell, m_AliveTile);
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

    }
}
