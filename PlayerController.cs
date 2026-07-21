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
    public float jumpHeight;
    [Export]
    public Camera3D camera;

    [Export]
    public float cameraSensitivity;
    [Export]
    public float sensitivityDivisor;

    [Export]
    public float baseGravityMagnitude = 9.8f;
    [Export]
    public float gravityScaleWhenFalling;

    [Export]
    public float cutFactorWhenJumpReleasedEarly;




    private float _pitch, _yaw;

    private RayCast3D _groundedRaycast;

    private bool _grounded;

    // INPUT FLAGS
    //

    private bool _jumpFlag;
    private bool _cutJumpFlag;


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


        //JUMPING -------------
        if (Input.IsActionJustPressed("jump") && _grounded)
        {
            _jumpFlag = true;
        }

        if (Input.IsActionJustReleased("jump") && LinearVelocity.Y > 0)
        {
            _cutJumpFlag = true;

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


    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        Vector3 currentLinearVelocity = state.LinearVelocity;
        Basis = Basis.FromEuler(new Vector3(0, Mathf.DegToRad(_yaw), 0));

        if (currentLinearVelocity.FlattenVector().Length() > maxSpeed)
        {
            LinearVelocity = new Vector3(currentLinearVelocity.Normalized().X * maxSpeed, currentLinearVelocity.Y, currentLinearVelocity.Normalized().Z * maxSpeed);
        }

        if(_jumpFlag){
            currentLinearVelocity.Y = jumpHeight.ToJumpVelocity(baseGravityMagnitude);
            _jumpFlag = false;
        }

        if (_cutJumpFlag)
        {
            currentLinearVelocity.Y *= cutFactorWhenJumpReleasedEarly;
            _cutJumpFlag = false;
        }



        //Handling Gravity Manually
        float appliedGravityMagnitude = currentLinearVelocity.Y < 0 ? baseGravityMagnitude * gravityScaleWhenFalling : baseGravityMagnitude;
        currentLinearVelocity.Y -= appliedGravityMagnitude * (float)state.Step;


        state.LinearVelocity = currentLinearVelocity;

    }

}
