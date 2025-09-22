using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private bool _isPlayer;

    private Camera mainCamera;

    private string _playerName;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        if (_isPlayer)
        {
            var playerName = PlayerDataManager.Instance.PlayInfo.PlayerName;

            // Player.
            _text.text = $"{playerName}";
        }
        else
        {
            // Unit.
            _text.text = $"{_playerName}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
    }

    /// <summary>
    /// Set unit id.
    /// </summary>
    public void SetPlayerName(string name)
    {
        _playerName = name;
    }
}
