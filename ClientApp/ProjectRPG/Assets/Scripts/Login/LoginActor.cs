using GameCore;
using GameCore.GamePacket;
using GamePackets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneLoadStepType
{
    None = 0,
    
    Ready = 1,
    Process = 2,
    Complete = 3,
}

public class LoginActor : Singleton<LoginActor>
{
    [SerializeField] private TMP_InputField _inputIdField;
    [SerializeField] private TMP_InputField _inputPassField;
	[SerializeField] private GameObject _button;

    private LoginManager _loginManager;

    private string _loadScene;
    private SceneLoadStepType LoadStepType;

    private void Start()
    {
        _loginManager = GetComponent<LoginManager>();
    }

    public void LoadScene(string scene)
    {
        _loadScene = scene;

        LoadStepType = SceneLoadStepType.Ready;
    }

    private void Update()
    {
        switch (LoadStepType)
        {
            case SceneLoadStepType.Ready:
                {
                    LoadStepType = SceneLoadStepType.Process;
                    SceneManager.LoadScene(_loadScene);
                }
                break;
            case SceneLoadStepType.Process:
                {
                    var nowScene = SceneManager.GetActiveScene();

                    // 현재 씬 검사.
                    if (nowScene.name == _loadScene)
                    {
                        LoadStepType = SceneLoadStepType.Complete;

                        // Spawn units.
                        GamePacketManager.Instance.Send<GamePacketCS_EnterPlayer>(GamePacketCode.CS_EnterPlayer, new GamePacketCS_EnterPlayer());
                    }
                }
                break;
        }
    }

    public void OnClickLogin()
    {
        StartCoroutine(_loginManager.Request_Auth(_inputIdField.text));
    }
}
