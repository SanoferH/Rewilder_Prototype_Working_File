using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScreenAnim : MonoBehaviour
{
    
    [SerializeField] private RectTransform welcomeScreenPositionRectTransform;
    [SerializeField] private CanvasGroup welcomeScreenUI;
    
    [SerializeField] private RectTransform rewilderLogoRectTransform;
    [SerializeField] private CanvasGroup rewilderLogoUI;
    // Start is called before the first frame update
    void Start()
    {
        welcomeScreenPositionRectTransform.transform.LeanMoveLocal(new Vector2(0, 260), 1.5f).setEaseInOutCubic();
        welcomeScreenUI.LeanAlpha(1, 1.0f).setEaseOutQuart();
        rewilderLogoRectTransform.transform.LeanScale(new Vector2(1.1f, 1.1f), 3.0f).setEaseInOutCubic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
