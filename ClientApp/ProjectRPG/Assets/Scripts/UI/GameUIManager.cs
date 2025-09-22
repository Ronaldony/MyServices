public class GameUIManager : Singleton<GameUIManager>
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
