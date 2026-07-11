

using Godot;

public static class Extensions
{
    public static Quaternion ToQuaternion(this Vector3 vector3)
    {
        Vector3 eulerAnglesInRadians = new Vector3(
            Mathf.DegToRad(vector3.X),
            Mathf.DegToRad(vector3.Y),
            Mathf.DegToRad(vector3.Z)
        );

        return Quaternion.FromEuler(eulerAnglesInRadians);
    }

    public static Vector3 FlattenVector(this Vector3 vector3)
    {
        return new Vector3(vector3.X, 0, vector3.Z);
    }
}
