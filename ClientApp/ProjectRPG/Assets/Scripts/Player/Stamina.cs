using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : Singleton<Stamina>
{
    public int CurrentStamina { get; private set; }

    [SerializeField] private Sprite fullStaminaImage, emptyStaminaImage;
    [SerializeField] private int timeBetweenStaminaRefresh = 3;

    private float nowTimePassed = 0;
    private int startingStamina = 3;
    private int maxStamina;
    const string STAMINA_CONTAINER_TEXT = "Stamina Container";

    protected override void Awake()
    {
        base.Awake();

        maxStamina = startingStamina;
        CurrentStamina = startingStamina;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (CurrentStamina < maxStamina)
        {
            nowTimePassed += Time.deltaTime;
            if (nowTimePassed > timeBetweenStaminaRefresh)
            {
                RefreshStamina();
                nowTimePassed = 0;
            }
        }
    }

    public void UseStamina()
    {
        CurrentStamina--;
        UpdateStaminaImages();
    }

    public void RefreshStamina(int refresh = 1)
    {
        CurrentStamina += refresh;

        if (CurrentStamina > maxStamina)
        {
            CurrentStamina = maxStamina;
        }

        UpdateStaminaImages();
    }

    private void UpdateStaminaImages()
    {
        for (int i = 0; i < maxStamina; i++)
        {
            if (i <= CurrentStamina - 1)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = fullStaminaImage;
            } 
            else
            {
                transform.GetChild(i).GetComponent<Image>().sprite = emptyStaminaImage;
            }
        }
    }
}
