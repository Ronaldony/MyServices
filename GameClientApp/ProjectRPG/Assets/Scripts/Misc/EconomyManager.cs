using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    private int currentGold = 0;

    const string COIN_AMOUNT__TEXT = "Gold Amount Text";

    public void UpdateCurrentGold()
    {
        currentGold++;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT__TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = currentGold.ToString("D3");
    }
}
