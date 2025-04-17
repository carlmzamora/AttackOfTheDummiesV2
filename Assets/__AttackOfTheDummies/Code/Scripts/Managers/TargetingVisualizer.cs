using UnityEngine;

public class TargetingVisualizer : MonoBehaviour
{
    [SerializeField] private RadiusReticle defaultRadiusReticle;
    public RadiusReticle RadiusReticle => defaultRadiusReticle;

    public static TargetingVisualizer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        defaultRadiusReticle.gameObject.SetActive(false);
    }
}