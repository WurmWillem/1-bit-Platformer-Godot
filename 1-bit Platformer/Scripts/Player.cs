using Godot;
using System;
using System.Collections;

public class Player : KinematicBody2D
{
	private const int ACCELERATION = 600;
	private const int MAXSPEED = 90;
	private const float FRICTION = 0.4f;
	private const float AIRFRICTION = 0.02f; //0.03
	private const float AIRFRICTIONWITHINPUT = 0.0f;
	private const int GRAVITY = 250; //200
	private const int JUMPFORCE = 140; //128
	
	private float fallMult = 1f;

	private bool isWallJumping = false;
	private float wallJumpTimer = 0.12f;
	private float resetWallJumpTimer = 0.12f;

	private int dashSpeed = 175;
	private float DashTimer = 0.18f;
	private float resetDashTimer = 0.18f;

	private bool isDashing = false;
	private bool canDash = true;

	private Vector2 velocity = Vector2.Zero;
	
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


		if (xInput != 0 && !isWallJumping && !isDashing) 
		{
			ProcessMovement(xInput, delta);
		}
		else { animationPlayer.Play("Stand"); } 
		

		if (Input.IsActionJustPressed("dash") && canDash)
        {
			Dash(delta);
		}


		if (Input.IsActionJustPressed("jump") && IsOnFloor() != true) //Walljump
		{
			WallJump(xInput, delta);
		}


		if (isWallJumping)
		{
			FreezePlayerDuringWallJump(delta);
		}


		if (IsOnFloor())  
		{
			CheckJump(xInput);

			canDash = true;
		}
		else 
		{
			InAir(xInput);
		}


		if (isDashing)
		{
			FreezePlayerDuringDash(delta);
		}
		else
        {
			velocity.y += GRAVITY * fallMult * delta; // Apply gravity
		}

		velocity = MoveAndSlide(velocity, Vector2.Up); // Apply forces to the kinematic body 2D
	}


	private void CheckJump(float xInput)
    {
		if (xInput == 0) { velocity.x = Mathf.Lerp(velocity.x, 0, FRICTION); }

		if (Input.IsActionJustPressed("jump"))
		{
			velocity.y = -JUMPFORCE;
		}
	}

	private void WallJump(float XInput, float delta) 
	{
		if (GetNode<RayCast2D>("RayCastLeft").IsColliding())
		{
			velocity.y = -JUMPFORCE / 1.4f;
			velocity.x = JUMPFORCE / 1.25f;

			

			isWallJumping = true;
		}
		else if (GetNode<RayCast2D>("RayCastRight").IsColliding())
		{
			velocity.y = -JUMPFORCE / 1.4f;
			velocity.x = -JUMPFORCE / 1.25f;

			isWallJumping = true;
		}
	}

	private void Dash(float delta)
    {
		if (Input.IsActionPressed("ui_left"))
		{
			velocity.x = -dashSpeed;
		}

		else if (Input.IsActionPressed("ui_right"))
		{
			velocity.x = dashSpeed;
		}
		
		else if (sprite.FlipH == true)
		{
			velocity.x = -dashSpeed;
		}
		else if (sprite.FlipH == false)
		{
			velocity.x = dashSpeed;
		}

		velocity.y = 0;

		isDashing = true;
		canDash = false;
	}

	private void FreezePlayerDuringWallJump(float delta)
    {
		wallJumpTimer -= delta;
		
		if (wallJumpTimer <= 0)
		{
			isWallJumping = false;
			wallJumpTimer = resetWallJumpTimer;
		}
	}

	private void FreezePlayerDuringDash(float delta)
    {
		DashTimer -= delta;

		if (DashTimer <= 0)
		{
			isDashing = false;
			DashTimer = resetDashTimer;

			velocity = velocity / 1.2f;
		}
	}

	private void ProcessMovement(float xInput, float delta)
    {
		animationPlayer.Play("Run");

		velocity.x += xInput * ACCELERATION * delta;

		
		velocity.x = Mathf.Clamp(velocity.x, -MAXSPEED, MAXSPEED);

		sprite.FlipH = xInput < 0;
	}

	private void InAir(float xInput) 
	{
		animationPlayer.Play("Jump");
			
		if (Input.IsActionJustReleased("jump") && velocity.y < -JUMPFORCE / 2) 
		{  
			velocity.y = -JUMPFORCE / 2;
		}
			
		fallMult = 1f;
			
		if (velocity.y >= 0f) { fallMult = 1.75f; }
			
		if (xInput == 0)  { velocity.x = Mathf.Lerp(velocity.x, 0, AIRFRICTION); }
			
		else { velocity.x = Mathf.Lerp(velocity.x, 0, AIRFRICTIONWITHINPUT); }
	}
	
	public void _on_Heart_body_entered(object body)
	{
		string currentScene = GetTree().CurrentScene.Name;

		if (currentScene == "Level 1") { GetTree().ChangeScene("res://Level 2.tscn"); }
		if (currentScene == "Level 2") { GetTree().ChangeScene("res://Level 3.tscn"); }
		if (currentScene == "Level 3") { GetTree().ChangeScene("res://Level select.tscn"); }
		if (currentScene == "Level 4") { GetTree().ChangeScene("res://Level 5.tscn"); }
	}
	public void SpikesEntered(object body)
	{
		if (body is KinematicBody2D) {

			GetTree().ReloadCurrentScene();
		}
	}
}