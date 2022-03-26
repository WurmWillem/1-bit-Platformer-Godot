using Godot;
using System;
using System.Collections;

public class Player : KinematicBody2D
{
	private const int ACCELERATION = 550;
	private const int MAXSPEED = 90;
	private const float FRICTION = 0.3f;
	private const float AIRFRICTION = 0.01f; //0.03
	private const float AIRFRICTIONWITHINPUT = 0.0f;
	private const int GRAVITY = 250; //200
	private const int JUMPFORCE = 140; //128
	
	private float fallMult = 1f;

	private bool isWallJumping = false;
	private float wallJumpTimer = 0.12f;
	private float resetWallJumpTimer = 0.12f;

	
	private Vector2 motion = Vector2.Zero;
	
	Sprite sprite = null;
	AnimationPlayer animationPlayer = null;
	
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite>("Sprite");
	}
	

	public override void _PhysicsProcess(float delta)
	{

		var xInput = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"); // Get input: 0, 1 or 2

		//GD.Print(isWallJumping);

		if (xInput != 0 && !isWallJumping) 
		{
			ProcessMovement(xInput, delta);
		}
		else { animationPlayer.Play("Stand"); } 
		

		if (Input.IsActionJustPressed("jump") && IsOnFloor() != true) //Walljump
		{
			WallJump(xInput, delta);
		}


		if (isWallJumping)
		{
			FreezePlayer(delta);
		}


		if (IsOnFloor())  
		{
			if (xInput == 0) { motion.x = Mathf.Lerp(motion.x, 0, FRICTION); }
			
			if (Input.IsActionJustPressed("jump")) 
			{ 
				motion.y = -JUMPFORCE; 
			}
		}
		else 
		{
			InAir(xInput);
		} 
		

		motion.y += GRAVITY * fallMult * delta; // Apply gravity
		
		motion = MoveAndSlide(motion, Vector2.Up); // Apply forces to the kinematic body 2D
	}


	private void WallJump(float XInput, float delta) 
	{
		if (GetNode<RayCast2D>("RayCastLeft").IsColliding())
		{
			motion.y = -JUMPFORCE / 1.4f;
			motion.x = JUMPFORCE / 1.25f;

			

			isWallJumping = true;
		}
		else if (GetNode<RayCast2D>("RayCastRight").IsColliding())
		{
			motion.y = -JUMPFORCE / 1.4f;
			motion.x = -JUMPFORCE / 1.25f;

			isWallJumping = true;
		}
	}

	private void FreezePlayer(float delta)
    {
		wallJumpTimer -= delta;
		
		if (wallJumpTimer <= 0)
		{
			isWallJumping = false;
			wallJumpTimer = resetWallJumpTimer;
		}
	}

	private void ProcessMovement(float xInput, float delta)
    {
		animationPlayer.Play("Run");

		motion.x += xInput * ACCELERATION * delta;
		motion.x = Mathf.Clamp(motion.x, -MAXSPEED, MAXSPEED);

		sprite.FlipH = xInput < 0;
	}

	private void InAir(float xInput) 
	{
		animationPlayer.Play("Jump");
			
		if (Input.IsActionJustReleased("jump") && motion.y < -JUMPFORCE / 2) 
		{  
			motion.y = -JUMPFORCE / 2;
		}
			
		fallMult = 1f;
			
		if (motion.y >= 0f) { fallMult = 1.75f; }
			
		if (xInput == 0)  { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTION); }
			
		else { motion.x = Mathf.Lerp(motion.x, 0, AIRFRICTIONWITHINPUT); }
	}
	
	public void _on_Heart_body_entered(object body)
	{
		string currentScene = GetTree().GetCurrentScene().GetName();

		GD.Print(currentScene);

		if (currentScene == "Level 1") { GetTree().ChangeScene("res://Level 2.tscn"); }
		if (currentScene == "Level 2") { GetTree().ChangeScene("res://Level 3.tscn"); }
		if (currentScene == "Level 3") { GetTree().ChangeScene("res://Level 4.tscn"); }
		if (currentScene == "Level 4") { GetTree().ChangeScene("res://Level 5.tscn"); }
	}
	
	public void SpikesEntered(object body)
	{
		if (body is KinematicBody2D) {
			
			GetTree().ReloadCurrentScene();
		}
	}
}
