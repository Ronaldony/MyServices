using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    // Inspector에서 각 씬에 맞는 Canvas Prefab 할당
    [SerializeField] private GameObject _loginCanvas;
    [SerializeField] private GameObject _gameCanvas;

    private GameObject _currentCanvas;

    private Scene _scene;

    protected override void Awake()
    {
        base.Awake();

        // 씬 로드 완료 시 OnSceneLoaded 메서드 호출
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
    /// Update ui canvas.s
    /// </summary>
    private void UpdateUICanvas(Scene scene)
    {
        // 씬 전환 간 UI canvas 교체.
        if (scene.name == "Login")
        {
            // 로그인.
            _loginCanvas?.SetActive(true);
            _gameCanvas?.SetActive(false);
        }
        else
        {
            // 게임 씬.
            _loginCanvas?.SetActive(false);
            _gameCanvas?.SetActive(true);
        }
    }
}
