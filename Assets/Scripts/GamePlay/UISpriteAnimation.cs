using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UISpriteAnimation : MonoBehaviour
{

    public Image m_Image;

    public Sprite[] m_SpriteAppearArray;
    public Sprite[] m_SpriteIdleArray;
    public Sprite[] m_SpritePressArray;
    public float m_Speed = .02f;

    private int m_IndexSprite;
    private int m_PressIndexSprite;
    Coroutine m_CorotineAnim;
    bool IsIdleDone;
    bool IsAppearDone;
    bool IsPressDone;
    private bool pressAnimStarted;


    private void Start()
    {
        pressAnimStarted = false;
    }

    public void Func_PlayIdleUIAnim()
    {
        IsIdleDone = false;
        StartCoroutine(Func_PlayIdleAnimUI());
    }
    
    
    public void Func_PlayAppearUIAnim()
    {
        IsAppearDone = false;
        StartCoroutine(Func_PlayAppearAnimUI());
    }
    
    [Button]
    public void Func_PlayPressUIAnim()
    {
        if (pressAnimStarted == false)
        {
            pressAnimStarted = true;
            Func_StopIdleUIAnim();
            IsPressDone = false;
            StartCoroutine(Func_PlayPressAnimUI()); 
        }
        
    }
    
    [Button]
    public void Func_StopIdleUIAnim()
    {
        Debug.Log("Idle Stopped");
        IsIdleDone = true;
        StopCoroutine(Func_PlayIdleAnimUI());
    }
    
    public void Func_StopAppearUIAnim()
    {
        IsAppearDone = true;
        StopCoroutine(Func_PlayAppearAnimUI());
        Func_PlayIdleUIAnim();
    }
    
    public void Func_StopPressUIAnim()
    {
        IsPressDone = true;
        StopCoroutine(Func_PlayPressAnimUI());
    }
    IEnumerator Func_PlayIdleAnimUI()
    {
        
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteIdleArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteIdleArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsIdleDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayIdleAnimUI());
    }
    
    
    
    IEnumerator Func_PlayAppearAnimUI()
    {
       
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteAppearArray.Length)
        {
            Func_StopAppearUIAnim();
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteAppearArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsAppearDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayAppearAnimUI());
    }
    
    IEnumerator Func_PlayPressAnimUI()
    {
        
        yield return new WaitForSeconds(m_Speed);
        if (m_PressIndexSprite >= m_SpritePressArray.Length)
        {
            m_PressIndexSprite = 0;
        }
        m_Image.sprite = m_SpritePressArray[m_PressIndexSprite];
        m_PressIndexSprite += 1;
        if (IsPressDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayPressAnimUI());
    }
}