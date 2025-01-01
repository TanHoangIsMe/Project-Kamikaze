using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OverHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject overHeadBars;
    [SerializeField] private Image healthBackground;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthLoseFill;
    [SerializeField] private Image shieldFill;

    private Camera cam;
    private OnFieldCharacter champion;
    private float speed = 1f;

    private void Awake()
    {
        // set camera 
        cam = Camera.main;
        if (cam == null ) // if player 2 switch cam
            cam = GameObject.Find("Player 2 Camera").GetComponent<Camera>();

        champion = GetComponent<OnFieldCharacter>();
    }

    private void Update()
    {
        SetHealthBarLookAtCam(); // make health bar look at cam 
    }

    private void SetHealthBarLookAtCam()
    {
        overHeadBars.transform.rotation = cam.transform.rotation;
    }

    public void UpdateHealthFill()
    {
        if (champion != null)
        {
            // calculate health fill amount 
            float fillAmount = champion.CurrentHealth
                / champion.CurrentCharacter.Health;

            // calculate shield amount 
            float shieldAmount = champion.CurrentShield
                / champion.CurrentCharacter.Health;

            // update shield amount to display on UI
            if (shieldAmount > 0 && shieldAmount < 1 - fillAmount)
            {
                shieldAmount += fillAmount;
                shieldFill.fillOrigin = 0; // fill from left side
                // swap location for UI render
                shieldFill.transform.SetSiblingIndex(2);
                healthFill.transform.SetSiblingIndex(3);
            }
            else
            {
                shieldFill.fillOrigin = 1; // fill from right side
                // swap location for UI render
                shieldFill.transform.SetSiblingIndex(3);
                healthFill.transform.SetSiblingIndex(2);
            }

            // update health fill images
            healthFill.fillAmount = fillAmount;
            StartCoroutine(UpdateHealthLoseFill(fillAmount));

            //update shield fill image
            StartCoroutine(UpdateShieldFill(shieldAmount));
        }
    }

    // Using coroutine to reduce lose health fill slowly 
    private IEnumerator UpdateHealthLoseFill(float targetFillAmount)
    {
        float startFillAmount = healthFill.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * speed;
            // make sure duration value between 0 - 1
            float duration = Mathf.Clamp01(elapsedTime);
            healthFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, duration);
            yield return null; 
        }

        // make sure health lose fill = health fill
        healthLoseFill.fillAmount = targetFillAmount;
        yield return new WaitForSeconds(0.5f);

        // turn off health bar if character dead
        if (champion.CurrentHealth <= 0)
            overHeadBars.SetActive(false);
    }

    // Using coroutine to reduce lose health fill slowly 
    private IEnumerator UpdateShieldFill(float shieldAmount)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * speed;
            // make sure duration value between 0 - 1
            float duration = Mathf.Clamp01(elapsedTime);
            shieldFill.fillAmount = Mathf.Lerp(shieldFill.fillAmount, shieldAmount, duration);
            yield return null;
        }
    }
}
