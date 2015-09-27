using UnityEngine;

/// <summary>
/// This class assists with the creation and evolution of cellular automata. 
/// </summary>
public class CellularAutomaton
{
    public int[,] Data
    {
        get { return m_cells; }
    }

    private int m_width;
    private int m_depth;
    private int[,] m_cells;
    private const int DEAD = TerrainType.Generator_Empty;
    private const int ALIVE = TerrainType.Generator_Default;

    /// <summary>
    /// Represents a single cell in a cellular automata.
    /// </summary>
    private class Cell
    {
        public Vector2 Position { get; private set; }
        public bool IsAlive { get; set; }

        public Cell(Vector2 position)
        {
            Position = position;
            IsAlive = false;
        }
    }

    public CellularAutomaton(int width, int depth)
    {
        m_width = width;
        m_depth = depth;
        m_cells = MapHelpers.EmptyMap(m_width, m_depth,
            (int)TerrainType.Generator_Empty);
    }
    /// <summary>
    /// Makes a random percentage of all cells alive.
    /// </summary>
    /// <param name="fraction">The fraction of cells to make alive where 
    /// 0.1f = 10%.</param>
    public void MakeAlive(float fraction)
    {
        int quantity = (int)Mathf.Round(m_width * m_depth *
            Mathf.Clamp(fraction, 0, 1));
        while (quantity > 0)
        {
            int x = UnityEngine.Random.Range(0, m_width - 1);
            int z = UnityEngine.Random.Range(0, m_depth - 1);
            if (m_cells[x, z] == DEAD)
            {
                m_cells[x, z] = ALIVE;
                quantity--;
            }
        }
    }
    /// <summary>
    /// Performs n iterations of the cellular automata using the specified
    /// cell birth threshold and survival threshold.
    /// </summary>
    /// <param name="birthThreshold">Number of living neighbours that will 
    /// trigger birth.</param>
    /// <param name="survivalThreshold">Number of living neighbouts that 
    /// will sustain cell.</param>
    /// <param name="iterations">Number of iterations to perform.</param>
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
                    isAlive = m_cells[x, z] == ALIVE;
                    if (isAlive)
                    {
                        if (neighbours >= survivalThreshold)
                            isAlive = true;
                        else isAlive = false;
                    }
                    else if (neighbours >= birthThreshold)
                        isAlive = true;
                    m_cells[x, z] = isAlive ? ALIVE : DEAD;
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
            if (m_cells[x + 1, z] == ALIVE)
                count++;
        // Check cell on the bottom right.
        if (x != m_width - 1 && z != m_depth - 1)
            if (m_cells[x + 1, z + 1] == ALIVE)
                count++;
        // Check cell on the bottom.
        if (z != m_depth - 1)
            if (m_cells[x, z + 1] == ALIVE)
                count++;
        // Check cell on the bottom left.
        if (x != 0 && z != m_depth - 1)
            if (m_cells[x - 1, z + 1] == ALIVE)
                count++;
        // Check cell on the left.
        if (x != 0)
            if (m_cells[x - 1, z] == ALIVE)
                count++;
        // Check cell on the top left.
        if (x != 0 && z != 0)
            if (m_cells[x - 1, z - 1] == ALIVE)
                count++;
        // Check cell on the top.
        if (z != 0)
            if (m_cells[x, z - 1] == ALIVE)
                count++;
        // Check cell on the top right.
        if (x != m_width - 1 && z != 0)
            if (m_cells[x + 1, z - 1] == ALIVE)
                count++;
        return count;
    }
}