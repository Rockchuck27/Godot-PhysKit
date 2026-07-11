using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class PlayerController : RigidBody3D
{
    [Export]
    public float groundedAccel;
    [Export]
    public float groundedDeceleration;
    [Export]
    public float maxSpeed;
    [Export]
    public float jumpPower;
    [Export]
    public Camera3D camera;

    [Export]
    public float cameraSensitivity;
    [Export]
    public float sensitivityDivisor;




    private float _pitch, _yaw;

    private RayCast3D _groundedRaycast;

    private bool _grounded;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _groundedRaycast = GetNode<RayCast3D>("Grounded Raycast");
        Console.WriteLine("test");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        camera.Basis = Basis.FromEuler(
            new Vector3(Mathf.DegToRad(_pitch), 0, 0)
        );

        if (_groundedRaycast.IsColliding())
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }


    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion mouseMotion)
        {
            _pitch -= mouseMotion.Relative.Y * (cameraSensitivity / sensitivityDivisor);
            _yaw -= mouseMotion.Relative.X * (cameraSensitivity / sensitivityDivisor);
        }

    }

    public override void _PhysicsProcess(double delta)
    {

        //MOVEMENT ----------
        Vector2 inputVector = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back"
        );

        //Prevents players from getting extra speed by providing a diagonal input
        if (inputVector.Length() > 1)
        {
            inputVector = inputVector.Normalized();
        }

        Vector3 desiredVelocity = (new Vector3(inputVector.X, 0, inputVector.Y) * maxSpeed).Rotated(Vector3.Up, Rotation.Y);

        Vector3 velocityError = desiredVelocity - LinearVelocity;
        velocityError = new Vector3(velocityError.X, 0, velocityError.Z);


        if (desiredVelocity == Vector3.Zero)
        {
            ApplyCentralForce(velocityError * groundedDeceleration);
        }
        else
        {

            ApplyCentralForce(velocityError * groundedAccel);
        }


        //JUMPING -------------
        if (Input.IsActionJustPressed("jump") && _grounded)
        {
            ApplyImpulse(Vector3.Up * jumpPower);
        }


        if (Input.IsActionJustPressed("uncapture_mouse"))
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }


    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        Basis = Basis.FromEuler(new Vector3(0, Mathf.DegToRad(_yaw), 0));

        if (LinearVelocity.FlattenVector().Length() > maxSpeed)
        {
            LinearVelocity = new Vector3(LinearVelocity.Normalized().X * maxSpeed, LinearVelocity.Y, LinearVelocity.Normalized().Z * maxSpeed);
        }
    }

}
