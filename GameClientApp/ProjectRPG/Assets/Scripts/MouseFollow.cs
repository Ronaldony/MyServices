using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = transform.position - mousePos;

        transform.right = -direction;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
    }
}
