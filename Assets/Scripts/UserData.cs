using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UserData : MonoBehaviour
{
    [SerializeField] private LoginUIAnimation loginUIAnimation;
    public static UserData userdata;
    public TMP_InputField userNameInputField;
    public float waitTime;

    public string userName;

    private void Awake()
    {
        if (userdata == null)
        {
            userdata = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        waitTime = 0.7f;
    }

    public void SetUserDetails()
    {
        
        userName = "Hi "+userNameInputField.text;
        loginUIAnimation.FadeOutInputUI();
        StartCoroutine(WaitAndLoadScene());
        
    }

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(waitTime); 
        SceneManager.LoadSceneAsync("CFPCalculator");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
