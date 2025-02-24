using Godot;
using System;

public partial class Grid : Resource
{
	[Export]
	public Vector2 size = new Vector2(20, 20);
	[Export]
	public Vector2 cell_size = new Vector2(80, 80);

	public Vector2 half_cell_size => cell_size / 2;

	public Vector2 calculate_map_position(Vector2 grid_position)
    {
        return grid_position * cell_size + half_cell_size;
    }

	public Vector2 calculate_grid_coordinates(Vector2 map_position)
	{
        return (map_position / cell_size).Floor();
    }

	public bool is_within_bounds(Vector2 cell_coordinates)
	{
        return cell_coordinates.X >= 0 && cell_coordinates.X < size.X && cell_coordinates.Y >= 0 && cell_coordinates.Y < size.Y;
    }

	public Vector2 clamp(Vector2 grid_position)
	{
        return new Vector2(Mathf.Clamp(grid_position.X, 0, size.X - 1), Mathf.Clamp(grid_position.Y, 0, size.Y - 1));
    }

	public int as_index(Vector2 cell)
	{
        return (int)(cell.X + cell.Y * size.X);
    }

}
