using Godot;
using System;

public partial class PlayerInteraction : RayCast3D
{
    //grabPivotScene is nothing more than a Rigidbody3D with a mass of 1, that's it
    [Export] public PackedScene grabPivotScene;
    //Called InteractDistance because soon this package will have the ability to interact with other objects, such as talking to an NPC
    [Export] public float maxInteractDistance;
    [Export] public float grabStrength;
    //Prevents jitter, but if turned up to high can make it harder to move objects
    [Export] public float grabDampening;
    [Export] public float grabRotationStrength;
    [Export] public float grabRotationDampening;
    //Just a generic6DofJoint with all linear and angular limits set to 0, mimicking the "FixedJoint" component from Unity, which essentialy fuses two rigidbodies together
    [Export] public PackedScene fixedJointScene;
    [Export] public float grabDistanceAdjustmentIncrement;

    private Basis objectRotationOffset;
    private RigidBody3D activeRightGrabPivot;
    private Generic6DofJoint3D activeFixed6DofJoint;
    private RigidBody3D activeRightGrabObject;
    private float currentGrabDistance;
    private Vector3 targetGrabPosition;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TargetPosition = new Vector3(0, 0, -maxInteractDistance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        //GRABBING OBJECT---------------------------
        if (IsColliding())
        {
            Node colliderAsNode = GetCollider() as Node;
            if (colliderAsNode.GetMeta("grabbable", false).AsBool())
            {

                //GRABBING OBJECT---------------------------
                // Soon I will add logic to be able to grab objects with both hands independently, currently we only deal with the right hand grabbing objects
                if (Input.IsActionJustPressed("right_grab") && activeRightGrabPivot == null)
                {
                    activeRightGrabObject = colliderAsNode as RigidBody3D;


                    activeRightGrabPivot = grabPivotScene.Instantiate<RigidBody3D>();
                    activeRightGrabPivot.Position = GetCollisionPoint();
                    GetTree().Root.AddChild(activeRightGrabPivot);

                    currentGrabDistance = GlobalPosition.DistanceTo(GetCollisionPoint());

                    //Rotation is applied right to left, so here we undo the rotation of this Node, then apply the rotation of the activeRightGrabPivot, telling us the rotation difference between this Node and the activeRightGrabPivot
                    objectRotationOffset = activeRightGrabPivot.GlobalBasis * GlobalBasis.Inverse();

                    activeFixed6DofJoint = fixedJointScene.Instantiate<Generic6DofJoint3D>();
                    activeFixed6DofJoint.Position = GetCollisionPoint();
                    GetTree().Root.AddChild(activeFixed6DofJoint);
                    activeFixed6DofJoint.NodeA = activeRightGrabPivot.GetPath();
                    activeFixed6DofJoint.NodeB = activeRightGrabObject.GetPath();


                }
            }

        }

        //DROPPING OBJECT---------------------------
        if (activeRightGrabPivot != null)
        {

            //DROPPING OBJECT
            Vector3 toGrabbedObject = activeRightGrabObject.GlobalPosition - GlobalPosition;
            //Dot product is some straight up magic, even my calculus teacher doesn't know why it works.
            // All you need to know, is that if the dot product of two vectors is positive, they are less than 90 degrees apart, and if the product is negative, they are more than 90 degrees apart
            // What this means for us, is that if the dot product of the direction the player is facing and the direction the object is from this Node is negative, the object is behind the player
            if (Input.IsActionJustReleased("right_grab") || toGrabbedObject.Dot(-GlobalBasis.Z) < 0)
            {
                Drop();
            }

            if (Input.IsActionJustPressed("decrease_grab_distance"))
            {
                currentGrabDistance -= grabDistanceAdjustmentIncrement;
            }
            else if (Input.IsActionJustPressed("increase_grab_distance"))
            {
                currentGrabDistance += grabDistanceAdjustmentIncrement;
            }
            currentGrabDistance = Mathf.Clamp(currentGrabDistance, 0, maxInteractDistance);

        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (activeRightGrabPivot != null)
        {
            //POSITION OF GRABBED OBJECT
            targetGrabPosition = GlobalPosition + -GlobalTransform.Basis.Z * currentGrabDistance;
            Vector3 toTargetPosition = targetGrabPosition - activeRightGrabPivot.Position;
            activeRightGrabPivot.ApplyForce(toTargetPosition * grabStrength);
            activeRightGrabPivot.ApplyForce(-activeRightGrabPivot.LinearVelocity * grabDampening);

            //ROTATION OF GRABBED OBJECT
            Basis targetBasis = GlobalBasis * objectRotationOffset;
            Quaternion toTargetBasis = (targetBasis * activeRightGrabPivot.GlobalBasis.Inverse()).GetRotationQuaternion();
            activeRightGrabPivot.ApplyTorque(toTargetBasis.GetAxis() * toTargetBasis.GetAngle() * grabRotationStrength);
            activeRightGrabPivot.ApplyTorque(-activeRightGrabPivot.AngularVelocity * grabRotationDampening);
        }
    }


    public void Drop()
    {
        activeRightGrabPivot.QueueFree();
        activeFixed6DofJoint.QueueFree();
        activeRightGrabPivot = null;
    }
}
