using Cinemachine;

public class ScreenSnakeManager : Singleton<ScreenSnakeManager>
{
    private CinemachineImpulseSource source;

    protected override void Awake()
    {
        base.Awake();

        source = GetComponent<CinemachineImpulseSource>();
    }

    public void SnakeScreen()
    {
        source.GenerateImpulse();
    }
}
