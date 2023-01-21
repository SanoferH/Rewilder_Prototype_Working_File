using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalculateCarbonFootPrint : MonoBehaviour
{
    [SerializeField] private RectTransform hiMessage;
    [SerializeField] private RectTransform CFPMessage;
    
    [SerializeField] private CanvasGroup hiNameUI;
    [SerializeField] private CanvasGroup CFPTextUI;
    
    [SerializeField] private CanvasGroup LoadingGIF;
    
    
    
    private String userName;

    public TextMeshProUGUI hiUserName;

   // private UserData _userData;

    private void Awake()
    {
        hiUserName.text = WelcomeScreen.welcomeScreen.userName;
        hiNameUI.alpha = 0;
        CFPTextUI.alpha = 0;
        LoadingGIF.alpha = 0;
    }

    void Start()
    {
        hiNameUI.LeanAlpha(1, 1f);
        hiMessage.transform.LeanMoveLocal(new Vector2(0,200),1.5f).setEaseInOutCubic();
        StartCoroutine(WaitAndShowMessage());
    }
    
    IEnumerator WaitAndShowMessage()
    {
        yield return new WaitForSeconds(1.5f); 
        CFPTextUI.LeanAlpha(1, 1);
        CFPMessage.transform.LeanMoveLocal(new Vector2(18,0),1.5f).setEaseInOutCubic();
        LoadingGIF.LeanAlpha(1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
