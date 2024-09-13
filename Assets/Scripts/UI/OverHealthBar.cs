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
        cam = Camera.main;
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

    private void TurnOffOverHeadBar()
    {           
        healthBackground.gameObject.SetActive(false);
        healthFill.gameObject.SetActive(false);
        healthLoseFill.gameObject.SetActive(false);
        shieldFill.gameObject.SetActive(false);
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

            // update health fill images
            healthFill.fillAmount = fillAmount;
            StartCoroutine(UpdateHealthLoseFill(fillAmount));

            //calculate shield image position
            SetShieldImageNewPosition(fillAmount, shieldAmount);

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
            TurnOffOverHeadBar();
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

    private void SetShieldImageNewPosition(float fillAmount, float shieldAmount)
    {
        RectTransform rectTransform = shieldFill.GetComponent<RectTransform>();

        float newX = (0.9f * fillAmount) - (0.9f * shieldAmount);

        rectTransform.localPosition = new Vector3(newX, 0, 0);
    }
}
