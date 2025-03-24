using UnityEngine;
using UnityEngine.SceneManagement;

public class Destroyer : MonoBehaviour
{
    void Awake()
    {
        // 씬이 로드될 때 호출될 이벤트 등록
        SceneManager.sceneUnloaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene)
    {
        // 씬 전환 후 현재 UI Canvas를 제거
        Destroy(gameObject);

        // 이벤트 중복 등록 방지를 위해 이벤트 해제
        SceneManager.sceneUnloaded -= OnSceneLoaded;
    }
}
