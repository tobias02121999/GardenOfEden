using UnityEngine;

/// <summary>
/// Inherit from this class to make a script a singleton
/// Usage: public class ClassName : Singleton<ClassName>{}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;


    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning Null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Look for existing instance
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if there is none
                    if (m_Instance == null)
                    {
                        // Creating the GameObject to attach the singleton to
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return m_Instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }

    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}
