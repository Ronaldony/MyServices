using UnityEngine;

public class AreaEntrance : Singleton<AreaEntrance>
{
    [SerializeField] private string transitionName;

    private void Start()
    {
        if (transitionName == SceneManagement.Instance.SceneTransitionName)
        {
            PlayerController.Instance.transform.position = this.transform.position;
            CameraController.Instance.SetPlayerCameraFollow();
            
            // Fade screen clear.
            UIFade.Instance.FadeToClear();
        }
    }
}
