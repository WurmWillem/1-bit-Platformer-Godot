using Godot;
using System;

public class Player : KinematicBody2D
{
	private const int ACCELERATION = 512;
	private const int MAXSPEED = 85;
	private const float FRICTION = 0.2f;
	private const float AIRFRICTION = 0.03f;
	private const float AIRFRICTIONWITHINPUT = 0.01f;
	private const int GRAVITY = 200;
	private const int JUMPFORCE = 128;
	
	private float fallMult = 1f;
	
	private Vector2 motion = Vector2.Zero;
	
	Sprite sprite = null;
	AnimationPlayer animationPlayer = null;
	
	public override void _Ready()
	{
		sprite = GetNode<Sprite>("Sprite");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
			
			if (motion.y >= 0) { fallMult = 1.75f; }
			
			if (xInput == 0)  { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTION); }
			
			else { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTIONWITHINPUT); }
		}
		
		motion.y += GRAVITY * fallMult * delta;
		
		//GD.Print(motion.y);

		motion = MoveAndSlide(motion, Vector2.Up); // Apply force the kinematic body 2D
		
		//GD.Print(MoveAndSlide(motion, Vector2.Up));
	}
	
	
}
