using UnityEngine;

public class HPFloatingBar : MonoBehaviour
{
    // 유닛 머리 위 위치.
    // 따라다닐 유닛
    private Camera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        transform.position = target.position + offset;
    }
}
