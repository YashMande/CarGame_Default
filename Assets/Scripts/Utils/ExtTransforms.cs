
using UnityEngine;
public static class Transforms
{
    public static void DestroyChildren(this Transform t, bool destroyImmdediately = false)
    {
        foreach (Transform child in t)
        {
            if(destroyImmdediately)
            {
                MonoBehaviour.DestroyImmediate(child.gameObject);
            }
            else
            {
                MonoBehaviour.Destroy(child);
            }
        }
    }
}
