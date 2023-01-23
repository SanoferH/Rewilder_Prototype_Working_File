using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private DrawController drawController;
    [SerializeField] private Transform dotPointPosition;

    public string element;
    private bool isOrigin;

    private bool connected;

    private void Awake()
    {
        isOrigin = false;
        connected = false;
    }

    private void Start()
    {
        if (gameObject.GetComponent<FlowerIdentifier>())
        {
            element = "Flower";
        }
        if (gameObject.GetComponent<LeafIdentifier>())
        {
            element = "Leaf";
        }
        if (gameObject.GetComponent<GrassIdentifier>())
        {
            element = "Grass";
        }
    }

    public void InitiateDrawLine()                              //will get called when user selects this element to start drawing line
    {
        isOrigin = true;                                        //make current button as origin
        drawController.StartDrawing(dotPointPosition,element);
    }

    public void ConnectLine()
    {
        
        if (drawController.drawingStarted)                     // check and execute further only if the drawing has been already started
        {
            if (!isOrigin)                                     // to make sure connecting line is not the drawing start point
            {
                if (!connected)                                // to avoid the instantiation of multiple dots and lines
                {
                    connected = drawController.StartConnecting(dotPointPosition,element);
                    Debug.Log("connect_element "+element);
                }
            }
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }
}
