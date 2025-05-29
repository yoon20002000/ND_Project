using UnityEngine;

public abstract class MonoSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    protected static readonly object singletonLock = new object();

    protected static T FindOrCreateInstance(bool bDontDestroyOnLoad = false)
    {
        var foundObjs = FindObjectsByType<T>(FindObjectsSortMode.None);

        if (foundObjs.Length > 1)
        {
            Debug.LogError($"[Singleton] Multiple instances of {typeof(T)} found!");
            for (int index = 0; index < foundObjs.Length; ++index)
            {
                Debug.LogError($"Instance : {foundObjs[index].gameObject.name}", foundObjs[index]);
            }

            return null;
        }

        var inst = foundObjs[0];
        if (inst == null)
        {
            var obj = new GameObject(typeof(T).Name);
            inst = obj.AddComponent<T>();
            if (bDontDestroyOnLoad)
            {
                DontDestroyOnLoad(inst);
            }
        }

        return inst;
    }
}

public abstract class MonoSingleton<T> : MonoSingletonBase<T> where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            lock (singletonLock)
            {
                if (instance == null)
                {
                    instance = FindOrCreateInstance(false);
                }
            }

            return instance;
        }
    }
}

public abstract class MonoSingletonPersistent<T> : MonoSingletonBase<T> where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            lock (singletonLock)
            {
                if (instance == null)
                {
                    instance = FindOrCreateInstance(true);
                }
            }

            return instance;
        }
    }
}