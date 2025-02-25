using Godot;
using System;
using System.Collections.Generic;

public partial class PathFinder : RefCounted
{
    Vector2[] DIRECTIONS = [Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down];

    Grid grid;
    AStar2D astar = new AStar2D();

    public PathFinder(Grid grid, Vector2[] walkableCells)
    {
        this.grid = grid;
        var cell_mappings = new Dictionary<int, Vector2>();
        foreach (var cell in walkableCells)
        {
            cell_mappings.Add(grid.as_index(cell), cell);
        }

        addAndConnectPoints(cell_mappings);
    }

    private void addAndConnectPoints(Dictionary<int, Vector2> cell_mappings)
    {
        foreach(var point in cell_mappings)
        {
            astar.AddPoint(point.Key, point.Value);
        }

        foreach(var point in cell_mappings)
        {
            foreach (var neighborIndex in findNeighborIndices(point, cell_mappings))
            {
                astar.ConnectPoints(point.Key, neighborIndex);
            }
        }
    }

    private IEnumerable<long> findNeighborIndices(KeyValuePair<int, Vector2> point, Dictionary<int, Vector2> cell_mappings)
    {
        var ret = new List<long>();

        foreach (var direction in DIRECTIONS)
        {
            var neighbor = point.Value + direction;
            if (!cell_mappings.ContainsValue(neighbor))
            {
                continue;
            }

            if (!astar.ArePointsConnected(point.Key, grid.as_index(neighbor)))
            {
                ret.Add(grid.as_index(neighbor));
            }
        }

        return ret;
    }

    public Vector2[] FindPath(Vector2 start, Vector2 end)
    {
        var start_index = grid.as_index(start);
        var end_index = grid.as_index(end);
        if (astar.HasPoint(start_index) && astar.HasPoint(end_index))
        {
            return astar.GetPointPath(start_index, end_index);
        }
        else
        {
            return default;
        }
    }
}
