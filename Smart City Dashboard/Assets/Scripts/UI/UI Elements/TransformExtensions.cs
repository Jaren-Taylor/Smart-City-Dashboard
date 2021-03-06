using UnityEngine;

public static class TransformExtensions
{
    public static RectTransform RectTransform(this Transform transform) => (RectTransform)transform;
    public static RectTransform RectTransform(this GameObject gameObject) => (RectTransform)gameObject.transform;
}
