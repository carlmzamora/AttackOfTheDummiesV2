using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameRefs : MonoBehaviour
{
    [SerializeField] private bool _globalDebugMode;
    public bool DebugMode
    {
        get => _globalDebugMode;
        set
        {
            if (_globalDebugMode != value)
            {
                _globalDebugMode = value;
                OnDebugModeChanged?.Invoke(_globalDebugMode);
            }
        }
    }

    public Vector3Variable playerPosition;
    
    public event System.Action<bool> OnDebugModeChanged = delegate { };

    public static InGameRefs Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        OnDebugModeChanged?.Invoke(_globalDebugMode);
    }
#endif
}