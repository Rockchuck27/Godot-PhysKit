

using Godot;

public static class Extensions
{
    public static float ToJumpVelocity(this float desiredHeight, float risingGravityMagnitude)
    {
        return Mathf.Sqrt(2 * risingGravityMagnitude * desiredHeight);

    }

    public static Vector3 FlattenVector(this Vector3 vector3)
    {
        return new Vector3(vector3.X, 0, vector3.Z);
    }
}
