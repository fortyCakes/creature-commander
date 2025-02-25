using Godot;
using Godot.NativeInterop;
using System;
using System.Threading.Tasks;

[Tool]
public partial class Unit : Path2D
{
	[Export] public Grid grid = ResourceLoader.Load("res://Grid.tres") as Grid;
	[Export] public int move_range = 6;
	[Export] public Texture2D skin;
	[Export] public Vector2 skin_offset = Vector2.Zero;
	[Export] public int move_speed = 600;

	public Vector2 cell { 
		get => _cell; 
		set { 
			if (grid == null)
            {
                grid = ResourceLoader.Load("res://Grid.tres") as Grid;
            }
            _cell = grid.clamp(value); 
		} 
	}

	public bool is_selected = false;

	public bool is_walking = false;

	Sprite2D _sprite;
	AnimationPlayer _anim_player;
	PathFollow2D _path_follow;
    private Vector2 _cell = Vector2.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		grid = ResourceLoader.Load("res://Grid.tres") as Grid;
        _sprite = GetNode("%Sprite") as Sprite2D;
		_anim_player = GetNode("%AnimationPlayer") as AnimationPlayer;
        _path_follow = GetNode("%PathFollow2D") as PathFollow2D;

		SetProcess(false);

		// Initialise to where the unit is placed, then snap to cell.
		cell = grid.calculate_grid_coordinates(Position);
		Position = grid.calculate_map_position(cell);

		if (!Engine.IsEditorHint())
        {
            Curve = new Curve2D();
        }

		set_skin(skin);
		set_skin_offset(skin_offset);

        walk_along(new Vector2[] { new Vector2(2,2), new Vector2(2,5), new Vector2(8,5), new Vector2(8,7) });
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_path_follow.Progress += (float)(delta * move_speed);

        if (_path_follow.ProgressRatio >= 1)
        {
			_set_is_walking(false);
			_path_follow.ProgressRatio = 0;
            Position = grid.calculate_map_position(cell);
			Curve.ClearPoints();
			EmitSignal("WalkFinished");
        }
    }

	public void walk_along(Vector2[] path)
	{
		if (path.Length == 0)
        {
            return;
        }

		Curve.AddPoint(Vector2.Zero);
        foreach (var point in path)
		{
			Curve.AddPoint(grid.calculate_map_position(point) - Position);
        }
		cell = path[path.Length - 1];
		_set_is_walking(true);
    }

	public void set_is_selected(bool value)
	{
        is_selected = value;
        if (is_selected)
		{
			_anim_player.Play("selected");
		} 
		else
		{
			_anim_player.Play("idle");
        }
    }

	public void set_skin(Texture2D value)
	{
		skin = value;
		if (_sprite != null)
		{
            _sprite.Texture = skin;
        }        
    }

	public void set_skin_offset(Vector2 value)
	{
        skin_offset = value;
        if (_sprite != null)
        {
            _sprite.Position = skin_offset;
        }
        
    }

	public void _set_is_walking(bool value)
	{
		is_walking = value;
        SetProcess(is_walking);
	}

	[Signal]
	public delegate void WalkFinishedEventHandler();

    private bool WaitForNodeReady()
    {
        while (true)
		{
			if (this.IsNodeReady())
            {
                return true;
            }
        }
    }
}
