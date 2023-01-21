using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class LoginUIAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform okButton;
    [SerializeField] private RectTransform rewilderLogo;
    [SerializeField] private RectTransform welcomeMessageText;
    [SerializeField] private RectTransform introMessageText;
    [SerializeField] private RectTransform nameInputField;
    

    [SerializeField] private CanvasGroup introMessage;

    [SerializeField] private CanvasGroup welcomeMessage;
    [SerializeField] private CanvasGroup logoRewilder;
    [SerializeField] private CanvasGroup InputUIField;
    [SerializeField] private CanvasGroup OKUIButton;

    public float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        fadeTime = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.LeanMoveLocal(new Vector2(0, -217), 1.5f).setEaseInOutCubic();
        }
    }
    
    [Button]
    public void MoveInputUI()
    {
        transform.LeanMoveLocal(new Vector2(0, 364), 1.5f).setEaseInOutCubic();
        okButton.transform.LeanMoveLocal(new Vector2(0, 177), 1.5f).setEaseInOutCubic();
        rewilderLogo.transform.LeanMoveLocal(new Vector2(36, 660), 1.5f).setEaseInOutCubic();
        rewilderLogo.transform.LeanScale(new Vector2(0.75f, 0.75f), 1.5f).setEaseInOutCubic();
        introMessage.LeanAlpha(0, 0.5f);
    }

    public void FadeOutInputUI()
    {
        welcomeMessageText.transform.LeanMoveLocal(new Vector2(0,824),fadeTime).setEaseOutQuart();
        welcomeMessage.LeanAlpha(0, fadeTime-0.4f).setEaseOutQuart();
        
        rewilderLogo.transform.LeanMoveLocal(new Vector2(36,710),fadeTime).setEaseOutQuart();
        logoRewilder.LeanAlpha(0, fadeTime-0.4f).setEaseOutQuart();
        
        transform.LeanMoveLocal(new Vector2(0,414),fadeTime).setEaseOutQuart();
        InputUIField.LeanAlpha(0, fadeTime-0.4f).setEaseOutQuart();
        
        okButton.transform.LeanMoveLocal(new Vector2(0,227),fadeTime).setEaseOutQuart();
        OKUIButton.LeanAlpha(0, fadeTime-0.4f).setEaseOutQuart();
    }

    [Button]
    public void test()
    {
        transform.LeanMoveLocal(new Vector2(0, -217), 1.5f).setEaseInOutCubic();
    }
  
}
