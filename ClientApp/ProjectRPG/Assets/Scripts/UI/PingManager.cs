using GamePackets;
using TMPro;
using UnityEngine;

public class PingManager : Singleton<PingManager>
{
    [SerializeField] private TMP_Text _pingText;
    private GamePacketManager _gpm;
    private int _nowPing;

    // Start is called before the first frame update
    void Start()
    {
        _gpm = GamePacketManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        var ping = _gpm.GetPing();

        if (_nowPing != ping)
        {
            _nowPing = ping;
            _pingText.text = _nowPing.ToString() + " ms";
        }
    }
}
