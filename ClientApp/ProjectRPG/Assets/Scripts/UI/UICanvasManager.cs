using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    // Inspector���� �� ���� �´� Canvas Prefab �Ҵ�
    [SerializeField] private GameObject _loginCanvas;
    [SerializeField] private GameObject _gameCanvas;

    private GameObject _currentCanvas;

    private Scene _scene;

    protected override void Awake()
    {
        base.Awake();

        // �� �ε� �Ϸ� �� OnSceneLoaded �޼��� ȣ��
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        //UpdateUICanvas(_scene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _scene = scene;

        UpdateUICanvas(scene);
    }

    /// <summary>
    /// Update ui canvas.
    /// </summary>
    private void UpdateUICanvas(Scene scene)
    {
        // �� ��ȯ �� UI canvas ��ü.
        if (scene.name == "Login")
        {
            // �α���.
            _loginCanvas?.SetActive(true);
            _gameCanvas?.SetActive(false);
        }
        else
        {
            // ���� ��.
            _loginCanvas?.SetActive(false);
            _gameCanvas?.SetActive(true);
        }
    }
}
