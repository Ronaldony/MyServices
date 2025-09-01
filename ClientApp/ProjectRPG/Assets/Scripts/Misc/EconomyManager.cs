using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;

    const string COIN_AMOUNT__TEXT = "Gold Amount Text";

    public void UpdateCurrentGold(int gold)
    {
        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT__TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = gold.ToString("D3");
    }
}
