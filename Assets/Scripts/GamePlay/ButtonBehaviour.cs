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
    public bool isOrigin;

    public bool connected;

    public bool isReadytoRemove;

    public bool canDelete;

    private void Awake()
    {
        isOrigin = false;
        connected = false;
        isReadytoRemove = false;
        canDelete = false;
        drawController = GameObject.Find("Line Drawing Canvas").GetComponent<DrawController>();
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
        //isReadytoRemove = true;
        gameObject.tag = "IsOrigin";
    }

    public void ConnectLine()
    {
        
        if (drawController.drawingStarted)                     // check and execute further only if the drawing has been already started
        {
            if (!isOrigin)                                     // to make sure connecting line is not the drawing start point
            {
                if (!connected)                                // if not connected already and to avoid the instantiation of multiple dots and lines
                {
                    //isReadytoRemove = true;
                    connected = drawController.StartConnecting(dotPointPosition,element);
                    if (connected)
                    {
                        gameObject.tag = "ToRemove";
                        GameObject _origin = GameObject.FindGameObjectWithTag("IsOrigin");
                        Debug.Log("Before: "+_origin.tag);
                        _origin.gameObject.tag = "ToRemove";
                        Debug.Log("After: "+_origin.tag);
                        Debug.Log("connect_element "+element);
                    }
                    
                    
                }
            }
        }
    }
    
    private void Update()
    {
        if (gameObject.CompareTag("IsOrigin"))
        {
            Debug.Log("Need to check connection happeing or not");
            if (!drawController.drawingStarted)
            {
                this.gameObject.tag = "Elements";
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
/*
        if (canDelete)
        {
            Destroy(this.gameObject);
        }
        */
    }
}
