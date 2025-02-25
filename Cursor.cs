using Godot;
using System;

[Tool]
public partial class Cursor : Node2D
{
	[Signal] public delegate void AcceptPressedEventHandler();
	[Signal] public delegate void MovedEventHandler(Vector2 new_cell);

	[Export] public Grid grid = ResourceLoader.Load("res://Grid.tres") as Grid;
	[Export] public float ui_cooldown = 0.1f;

	Vector2 cell = Vector2.Zero;

	Timer timer;

    public Vector2 Cell 
	{ 
		get => cell; 
		set
		{ 
			var new_cell = grid.clamp(value);
			if (new_cell.IsEqualApprox(cell)) return;

			cell = new_cell;
            Position = grid.calculate_map_position(cell);
			EmitSignal("Moved", cell);
			timer.Start();
        } 
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		timer = GetNode("%Timer") as Timer;
		timer.WaitTime = ui_cooldown;
		Position = grid.calculate_map_position(cell);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseMotion mouse_motion)
		{
			Cell = grid.calculate_grid_coordinates(mouse_motion.Position);
		}
		else if (@event.IsActionPressed("click") || @event.IsActionPressed("ui_accept"))
		{
			EmitSignal("AcceptPressed");
			GetViewport().SetInputAsHandled();
        }

		var shouldMove = @event.IsPressed();
		
		if (@event.IsEcho())
		{
			shouldMove = shouldMove && timer.IsStopped();
		}

		if (shouldMove)
		{
			if (@event.IsAction("ui_right"))
			{
				Cell += Vector2.Right;
			}
			else if (@event.IsAction("ui_left"))
			{
				Cell += Vector2.Left;
			}
			else if (@event.IsAction("ui_down"))
			{
				Cell += Vector2.Down;
			}
			else if (@event.IsAction("ui_up"))
			{
				Cell += Vector2.Up;
			}
		}

    }

    public override void _Draw()
    {
		if (grid != null)
		{
			DrawRect(new Rect2(-grid.cell_size / 2, grid.cell_size), Colors.AliceBlue, false, 2.0f);
		}
    } 
}
