using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    [Space(10)]
    [Header("DontDestroyOnLoad")]
    [SerializeField] bool _dontDestroyOnLoad = false;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if(_instance != this as T)
        {
            Destroy(gameObject);
        }
    }
}
