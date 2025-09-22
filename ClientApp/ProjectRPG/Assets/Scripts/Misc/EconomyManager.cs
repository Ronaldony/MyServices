using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    [SerializeField] private TMP_Text goldText;

    private int _totalGold;
    const string COIN_AMOUNT__TEXT = "Gold Amount Text";

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlusGold(int gold)
    {
        _totalGold += gold;

        goldText.text = _totalGold.ToString("D3");
    }
}
