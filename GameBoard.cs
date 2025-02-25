using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameBoard : Node2D
{
	Vector2[] DIRECTIONS = { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };

	[Export] Grid grid = ResourceLoader.Load("res://Grid.tres") as Grid;

    Godot.Collections.Dictionary<Vector2, Unit> units = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        grid = ResourceLoader.Load("res://Grid.tres") as Grid;
		Reinitialise();
        Console.WriteLine($"{units}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
    }

    private bool is_occupied(Vector2 cell)
    {
        return units.ContainsKey(cell);
    }

    private void Reinitialise()
    {
        units.Clear();

        foreach(var child in GetChildren())
        {
            if (child is Unit unit)
            {
                units[unit.cell] = unit;
            }
        }
    }

    private Vector2[] GetWalkableCells(Unit unit)
    {
        return FloodFill(unit.cell, unit.move_range);
    }

    private Vector2[] FloodFill(Vector2 cell, int max_distance)
    {
        var filledCells = new List<Vector2>();

        var stack = new Stack<Vector2>([cell]);

        while (stack.Any())
        {
            var current = stack.Pop();

            if (!grid.is_within_bounds(current) || filledCells.Contains(current))
            {
                continue;
            }

            var difference = (current - cell).Abs();
            var distance = difference.X + difference.Y; // Taxicab distance because we're on a grid
            if (distance > max_distance)
            {
                continue;
            }

            filledCells.Add(current);

            foreach (var direction in DIRECTIONS)
            {
                var newCell = current + direction;
                if (is_occupied(newCell))
                {
                    continue;
                }
                if (filledCells.Contains(newCell))
                {
                    continue;
                }
                stack.Push(newCell);
            }
        }

        return filledCells.ToArray();
    }
}
