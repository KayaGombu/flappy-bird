using Godot;
using System;

public partial class Bird : CharacterBody2D
{
    [Export]
    private float Gravity = 980.0f;

    [Export]
    private float jumpHeight = 300.0f;
    [Signal]
    public delegate void HitEventHandler();

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        //Gravity
        velocity.Y += (float)delta * Gravity;

        if (Input.IsActionJustPressed("jump"))
        {
            velocity = Vector2.Up * jumpHeight;       
        }

        Velocity = velocity;
        MoveAndSlide();
    }



}
