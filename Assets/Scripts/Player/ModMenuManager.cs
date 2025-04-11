using System.Collections;
using UnityEngine;

public class ModMenuManager : MonoBehaviour
{
    public static ModMenuManager Instance { get; private set; }
    
    public GameObject modMenuCanvas;
    public bool IsMenuOpen { get; private set; }
    
    private CanvasGroup canvasGroup;
    public float fadeDuration = 0.3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        canvasGroup = modMenuCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = modMenuCanvas.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        modMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleModMenu();
            Debug.Log("attempting mod menu");
        }
    }
    
    public void ToggleModMenu()
    {
        IsMenuOpen = !IsMenuOpen;
        
        if (IsMenuOpen)
        {
            modMenuCanvas.SetActive(true);
            modMenuCanvas.active = true;
            StartCoroutine(FadeMenu(true));
        }
        else
        {
            StartCoroutine(FadeMenu(false));
        }
        
        Time.timeScale = IsMenuOpen ? 0f : 1f;
    }

    IEnumerator FadeMenu(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float currentAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime/fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        if (!fadeIn) modMenuCanvas.SetActive(false);
    }
}