using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginActor : Singleton<LoginActor>
{
    [SerializeField] private TMP_InputField _inputIdField;
    [SerializeField] private TMP_InputField _inputPassField;
	[SerializeField] private GameObject _button;

    private LoginManager _loginManager;

    private bool _isLoadingScene;
    private string _loadScene;

    private void Start()
    {
        _loginManager = GetComponent<LoginManager>();
    }

    public void LoadScene(string scene)
    {
        _loadScene = scene;
        _isLoadingScene = true;
    }

    private void Update()
    {
        if (_isLoadingScene)
        {
            _isLoadingScene = false;
            SceneManager.LoadScene(_loadScene);
        }
    }

    public void OnClickLogin()
    {
        StartCoroutine(_loginManager.Request_Auth(_inputIdField.text));
    }
}
