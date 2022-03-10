using UnityEngine;

///<summary>
/// return three basic transform component, position, rotation, and scale.
///</summary>
public struct TransformValue
{
    private Vector3 _position;
    private Quaternion _rotation;
    private Vector3 _scale;

    public TransformValue(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
    }

    public TransformValue(Vector3 position, Quaternion rotation)
    {
        _position = position;
        _rotation = rotation;
        _scale = Vector3.one;
    }

    public TransformValue(Transform transformValues)
    {
        _position = transformValues.position;
        _rotation = transformValues.rotation;
        _scale = transformValues.localScale;
    }
}
