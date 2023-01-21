using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WelcomeScreen : MonoBehaviour
{
    public static WelcomeScreen welcomeScreen;
    public TMP_InputField userNameInputField;
    public float waitTime;
    public string userName;
    
    
    private void Awake()
    {
        if (welcomeScreen == null)
        {
            welcomeScreen = this;
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

    // Update is called once per frame
    void Update()
    {
    
    }
    
    public void SetUserDetails()
    {
        
        userName = "Hi "+userNameInputField.text;
       // loginUIAnimation.FadeOutInputUI();
        StartCoroutine(WaitAndLoadScene());
        
    }

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(waitTime); 
        SceneManager.LoadScene("CFPCalculator");
    }
}
