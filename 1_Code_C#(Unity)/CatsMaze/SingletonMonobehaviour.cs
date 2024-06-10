using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T m_instance;

    private static readonly object _lock = new();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (m_instance == null)
                {
                    //find existing instance
                    m_instance = GameObject.FindObjectOfType<T>();

                    if (m_instance == null)
                    {
                        //create new instance
                        GameObject go = new GameObject(typeof(T).Name);
                        m_instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }

                    //initialize instance if necessary
                    if (!m_instance.initialized)
                    {
                        m_instance.initialized = true;
                    }
                }
            }
            return m_instance;
        }
    }

    protected bool initialized { get; set; }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
