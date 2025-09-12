using UnityEngine;

public class HPFloatingBar : MonoBehaviour
{
    // 따라다닐 유닛
    public Transform target;

    // 유닛 머리 위 위치.
    private Vector3 offset;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        offset = new Vector3(0, 2f, 0);
    }

    void Update()
    {
        if (target == null) return;

        // 유닛의 월드 좌표 -> 화면 좌표 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);

        transform.position = screenPos;
    }
}
