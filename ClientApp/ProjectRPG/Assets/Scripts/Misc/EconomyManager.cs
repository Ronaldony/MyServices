using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;

    private int _totalGold;
    const string COIN_AMOUNT__TEXT = "Gold Amount Text";

    public void PlusGold(int gold)
    {
        _totalGold += gold;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT__TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = _totalGold.ToString("D3");
    }

    public void SetGold(int gold)
    {
        _totalGold = gold;
    }
}
