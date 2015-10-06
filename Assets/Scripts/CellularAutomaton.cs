using UnityEngine;

/// <summary>
/// This class allows for creation and evolution of cellular automata. 
/// </summary>
public class CellularAutomaton
{

    // Allows access to the generated information.
    public int[,] Data
    {
        get { return m_cells; }
    }

    private int m_width;
    private int m_depth;
    private int m_deadValue;
    private int m_aliveValue;
    private int[,] m_cells;

    public CellularAutomaton(int width, int depth, int deadValue = -1,
        int aliveValue = 1)
    {
        m_width = width;
        m_depth = depth;
        m_deadValue = deadValue;
        m_aliveValue = aliveValue;
        m_cells = MapHelpers.EmptyMap(m_width, m_depth, m_deadValue);
    }

    /// <summary>
    /// Makes the specified fraction of cells alive.
    /// </summary>
    public void Spawn(float fraction)
    {
        int quantity = (int)Mathf.Round(m_width * m_depth *
            Mathf.Clamp(fraction, 0, 1));
        int loopCount = 0;
        int loopCutoff = m_width * m_depth;
        while (quantity > 0)
        {
            int x = UnityEngine.Random.Range(0, m_width - 1);
            int z = UnityEngine.Random.Range(0, m_depth - 1);
            if (m_cells[x, z] == m_deadValue)
            {
                m_cells[x, z] = m_aliveValue;
                quantity--;
            }
            // If the cell grid contains too many 'alive' cells to achieve
            // the desired fraction, prevent the loop from executing forever.
            loopCount++;
            if (loopCount > loopCutoff)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Performs n iterations of the cellular automata using the specified
    /// cell birth threshold and survival threshold. The cell birth threshold
    /// is the number of living neighbours that will trigger birth in a 
    /// given cell. The survival threshold is the minimum number of living
    /// neighbours required to sustain the cell.
    /// </summary>
    public void Iterate(int birthThreshold, int survivalThreshold,
        int iterations)
    {
        int neighbours = -1;
        bool isAlive = false;
        for (int i = 0; i < iterations; i++)
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_depth; z++)
                {
                    neighbours = GetLivingNeighbors(x, z);
                    isAlive = m_cells[x, z] == m_aliveValue;
                    if (isAlive)
                    {
                        if (neighbours >= survivalThreshold)
                            isAlive = true;
                        else isAlive = false;
                    }
                    else if (neighbours >= birthThreshold)
                        isAlive = true;
                    m_cells[x, z] = isAlive ? 
                        m_aliveValue : m_deadValue;
                }
            }
        }
    }

    /// <summary>
    /// Returns the number of living cells adjacent to cell x, z.
    /// </summary>
    private int GetLivingNeighbors(int x, int z)
    {
        int count = 0;
        // Check cell on the right.
        if (x != m_width - 1)
            if (m_cells[x + 1, z] == m_aliveValue)
                count++;
        // Check cell on the bottom right.
        if (x != m_width - 1 && z != m_depth - 1)
            if (m_cells[x + 1, z + 1] == m_aliveValue)
                count++;
        // Check cell on the bottom.
        if (z != m_depth - 1)
            if (m_cells[x, z + 1] == m_aliveValue)
                count++;
        // Check cell on the bottom left.
        if (x != 0 && z != m_depth - 1)
            if (m_cells[x - 1, z + 1] == m_aliveValue)
                count++;
        // Check cell on the left.
        if (x != 0)
            if (m_cells[x - 1, z] == m_aliveValue)
                count++;
        // Check cell on the top left.
        if (x != 0 && z != 0)
            if (m_cells[x - 1, z - 1] == m_aliveValue)
                count++;
        // Check cell on the top.
        if (z != 0)
            if (m_cells[x, z - 1] == m_aliveValue)
                count++;
        // Check cell on the top right.
        if (x != m_width - 1 && z != 0)
            if (m_cells[x + 1, z - 1] == m_aliveValue)
                count++;
        return count;
    }
}