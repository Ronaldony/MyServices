using TMPro;
using UnityEngine;

public class PositionTag : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2Int _lastPos;
    [SerializeField] private Transform _transform;
    [SerializeField] private TMP_Text _positionText;

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

        Vector2 nowPos = _transform.position;
        var nowPosInt = Vector2Int.RoundToInt(nowPos);

        if (_lastPos != nowPosInt)
        {
            _lastPos = nowPosInt;
            _positionText.text = $"({_lastPos.x}/{_lastPos.y})";
        }
    }
}
