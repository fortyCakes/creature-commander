using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class unit_path : TileMap
{
	[Export] Grid grid = ResourceLoader.Load("res://Grid.tres") as Grid;

	PathFinder pathFinder;
	Vector2[] currentPath;

	public void initialize(Vector2[] walkableCells)
	{
        grid = ResourceLoader.Load("res://Grid.tres") as Grid;
        pathFinder = new PathFinder(grid, walkableCells);
    }

	public void draw(Vector2 start, Vector2 end)
	{
		Clear();
		currentPath = pathFinder.FindPath(start, end);

		foreach(var cell in currentPath)
		{
			var cellVector = new Vector2I((int)cell.X, (int)cell.Y);
			SetCellsTerrainConnect(0, [cellVector], 0, 0);
        }

		UpdateInternals();
    }

    public void stop()
	{
        pathFinder = null;
		Clear();
    }

    public override void _Ready()
    {
		var rect_start = new Vector2(4, 4);
		var rect_end = new Vector2(10, 8);

		var points = new List<Vector2>();

		foreach(var x in Enumerable.Range(0, (int)rect_end.X - (int)rect_start.X + 1))
		{
            foreach (var y in Enumerable.Range(0, (int)rect_end.Y - (int)rect_start.Y + 1))
            {
                points.Add(rect_start + new Vector2(x, y));
            }
        }

		initialize(points.ToArray());
		draw(rect_start, new Vector2(8,7));
    }
}
