using UnityEngine;

[CreateAssetMenu(menuName = "Game Of Life/Pattern", fileName = "GOLPattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] m_Cells;

    public Vector2Int GetCenter()
    {
        if (m_Cells == null || m_Cells.Length == 0) return Vector2Int.zero;

        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        for (int i = 0; i < m_Cells.Length; i++)
        {
            if (m_Cells[i].x < min.x)
            {
                min.x = m_Cells[i].x;
            }

            if (m_Cells[i].y < min.y)
            {
                min.y = m_Cells[i].y;
            }

            if (m_Cells[i].x > max.x)
            {
                max.x = m_Cells[i].x;
            }

            if (m_Cells[i].y < max.y)
            {
                max.y = m_Cells[i].y;
            }
        }

        int resultCenterX = Mathf.RoundToInt(max.x - min.x / 2);
        int resultCenterY = Mathf.RoundToInt(max.y - min.y / 2);

        return new Vector2Int(resultCenterX, resultCenterY);
    }
}
