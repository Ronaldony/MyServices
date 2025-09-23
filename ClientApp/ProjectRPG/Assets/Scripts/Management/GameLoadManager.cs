using System.Collections;
using UnityEngine;

public enum LoadStepType
{
    None = 0,
    Ready = 1,
    Process = 2,
    Complete = 3,
}

public class GameLoadManager : Singleton<GameLoadManager>
{
    private const int SecForLoadStep = 2;

    private LoadStepType _loadStep;

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_loadStep)
        {
            case LoadStepType.None:
                break;
            case LoadStepType.Ready:
                {
                    // Setup inventory.
                    GameUIManager.Instance.UpdateActive(true);
                    Inventory.Instance.Setup(PlayerDataManager.Instance.InventoryInfo);
                    WeaponInventory.Instance.ToggleActiveSlot(1);

                    _loadStep = LoadStepType.Process;
                }
                break;
            case LoadStepType.Process:
                {
                    StartCoroutine(UpdateLoadStepCoroutine(LoadStepType.Complete));
                }
                break;
            case LoadStepType.Complete:
                gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Update load step.
    /// </summary>
    public void UpdateLoadStep(LoadStepType loadStep)
    {
        _loadStep = loadStep;
    }

    public bool IsLoadComplete() => _loadStep == LoadStepType.Complete ? true : false;

    /// <summary>
    /// Update load step routine.
    /// </summary>
    private IEnumerator UpdateLoadStepCoroutine(LoadStepType loadStep)
    {
        yield return new WaitForSeconds(SecForLoadStep);

        _loadStep = loadStep;
    }
}
