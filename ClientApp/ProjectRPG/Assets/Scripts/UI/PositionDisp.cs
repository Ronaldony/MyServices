using TMPro;
using UnityEngine;

public class PositionDisp : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2Int _lastPos;
    [SerializeField] TMP_Text _positionText;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        _lastPos = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;

        var nowPos = Vector2Int.RoundToInt(PlayerController.Instance.GetPosition());

        if (_lastPos != nowPos)
        {
            _lastPos = nowPos;
            _positionText.text = $"({_lastPos.x}/{_lastPos.y})";
        }
    }
}
