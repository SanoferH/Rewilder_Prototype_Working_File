using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScreenUIAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform welcomeMessagePosition;
    [SerializeField] private RectTransform rewilderLogoPosition;
    [SerializeField] private RectTransform introMessagePosition;
    [SerializeField] private RectTransform nameInputFieldPosition;
    [SerializeField] private RectTransform okButtonPosition;
    
    [SerializeField] private CanvasGroup welcomeMessageUI;
    [SerializeField] private CanvasGroup rewilderLogoUI;
    [SerializeField] private CanvasGroup introMessageUI;
    [SerializeField] private CanvasGroup OKButtonUI;
    [SerializeField] private CanvasGroup InputFieldUI;

    public float displacement;
    public float fadeTime;
   // public float waitTime;

    private void Awake()
    {
        fadeTime = 1.5f;
        displacement = 50.0f;
        /*
        welcomeMessageUI.alpha = 0;
        rewilderLogoUI.alpha = 0;
        introMessageUI.alpha = 0;
        InputFieldUI.alpha = 0;
        OKButtonUI.alpha = 0;
        */
        // waitTime = 0.5f;
        //InitialSetup();
        Debug.Log("Awake");
    }

    private void Start()
    {
        Debug.Log("Start");
        welcomeMessagePosition.transform.LeanMoveLocal(new Vector2(0, 800), 0.5f).setEaseInOutCubic();
        welcomeMessageUI.LeanAlpha(1, 0.5f).setEaseInOutCubic();

        //StartCoroutine(ShowLogo());
        
        
    }

    IEnumerator ShowLogo()
    {
        yield return new WaitForSeconds(0.8f);
        
        rewilderLogoPosition.transform.LeanMoveLocal(new Vector2(36, 580), 1.0f).setEaseInOutCubic();
        rewilderLogoUI.LeanAlpha(1, fadeTime).setEaseInQuint();
        rewilderLogoPosition.transform.LeanScale(new Vector2(1.1f, 1.1f), 4.0f).setEaseInOutCubic();

          yield return StartCoroutine(showIntroMessage());

          yield return StartCoroutine(InputUI());
    }

    IEnumerator showIntroMessage()
    {
        yield return new WaitForSeconds(2.0f);
        introMessagePosition.transform.LeanMoveLocal(new Vector2(0, 240), 1.5f).setEaseInOutCubic();
        introMessageUI.LeanAlpha(1, fadeTime).setEaseInQuint();
    }

    IEnumerator InputUI()
    {
        yield return new WaitForSeconds(3.0f);
        nameInputFieldPosition.transform.LeanMoveLocal(new Vector2(0, -20), 1.5f).setEaseInOutCubic();
        InputFieldUI.LeanAlpha(1, fadeTime).setEaseOutQuart();
        
        okButtonPosition.transform.LeanMoveLocal(new Vector2(0, -230), 1.5f).setEaseInOutCubic();
        OKButtonUI.LeanAlpha(1, fadeTime).setEaseOutQuart();
    }

    private void InitialSetup()
    {
        
        welcomeMessagePosition.transform.localPosition =
            new Vector2(0, welcomeMessagePosition.anchoredPosition.y + displacement);
        
        rewilderLogoPosition.transform.localPosition =
            new Vector2(36.0f, rewilderLogoPosition.anchoredPosition.y + displacement);
        
        introMessagePosition.transform.localPosition =
            new Vector2(0, introMessagePosition.anchoredPosition.y + displacement);
        
        nameInputFieldPosition.transform.localPosition =
            new Vector2(0, nameInputFieldPosition.anchoredPosition.y + displacement);
        
        okButtonPosition.transform.localPosition =
            new Vector2(0, okButtonPosition.anchoredPosition.y + displacement);
        
      
    }

    public void FadeInWelcomeUI()
    {
        
    }

    public void FadeOutWelcomeUI()
    {
        
    }

    IEnumerator WaitAndProceed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); 
    }
}
