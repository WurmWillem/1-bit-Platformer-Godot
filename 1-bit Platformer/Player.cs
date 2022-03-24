using Godot;
using System;

public class Player : KinematicBody2D
{
	private const int ACCELERATION = 550;
	private const int MAXSPEED = 90;
	private const float FRICTION = 0.4f;
	private const float AIRFRICTION = 0f; //0.03
	private const float AIRFRICTIONWITHINPUT = 0.01f;
	private const int GRAVITY = 250; //200
	private const int JUMPFORCE = 140; //128
	
	private float fallMult = 1f;
	
	private Vector2 motion = Vector2.Zero;
	
	Sprite sprite = null;
	AnimationPlayer animationPlayer = null;
	
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite>("Sprite");
	}
	
	public override void _Process(float delta) 
	{
		var mouseInput = Input.GetActionStrength("click");
		
		if (mouseInput == 1) 
		{
			GD.Print("click at " + GetViewport().GetMousePosition());
		}
		
		//GetNode<Sprite>("/root/Test/Cursor").Position = GetViewport().GetMousePosition();
		
	}
	
	public override void _PhysicsProcess(float delta)
	{
		
		var xInput = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"); // Get input: 0,1 or 2
		
		if (xInput != 0) 
		{
			animationPlayer.Play("Run");
			
			motion.x += xInput * ACCELERATION * delta;
			motion.x = Mathf.Clamp(motion.x, -MAXSPEED, MAXSPEED);
			
			sprite.FlipH = xInput < 0;
		}
		else { animationPlayer.Play("Stand"); } 
		
		
		
		if (IsOnFloor())  
		{
			if (xInput == 0) { motion.x = Mathf.Lerp(motion.x, 0, FRICTION); }
			
			if (Input.IsActionJustPressed("ui_up")) 
			{ 
				motion.y = -JUMPFORCE; 
			}
		}
		else 
		{
			animationPlayer.Play("Jump");
			
			if (Input.IsActionJustReleased("ui_up") && motion.y < -JUMPFORCE / 2) 
			{  
				motion.y = -JUMPFORCE / 2;
			}
			
			fallMult = 1f;
			
			if (motion.y >= 0f) { fallMult = 1.75f; }
			
			if (xInput == 0)  { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTION); }
			
			else { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTIONWITHINPUT); }
		} 
		
		motion.y += GRAVITY * fallMult * delta; // Apply gravity
		
		motion = MoveAndSlide(motion, Vector2.Up); // Apply forces to the kinematic body 2D
		
	}
	
	public void _on_Heart_body_entered(object body)
	{
		GetTree().ChangeScene("res://Level Select.tscn");
	}
	
	
}



