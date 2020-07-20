using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : class
{
    public static T Instance;

    protected void Awake()
    {
        Instance = this as T;
    }
}
