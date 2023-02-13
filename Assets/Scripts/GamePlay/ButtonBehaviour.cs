using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private DrawController drawController;
    [SerializeField] private Transform dotPointPosition;

    public List<GameObject> neighbours;

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
        if (drawController.drawingStarted)
        {
            Debug.Log("Play Appear animation and then play idle animation"); 
        }
        else
        {
            Debug.Log("Directly play idle animation"); 
        }
        GetComponent<UISpriteAnimation>().Func_PlayAppearUIAnim();
       // Debug.Log("Animation can be played now");
        neighbours = new List<GameObject>();
       // GetComponentInParent<NeighbourElemnts>().Neighbours
       foreach (GameObject elements in GetComponentInParent<NeighbourElemnts>().Neighbours)
       {
           neighbours.Add(elements);
       }
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
     //   GetComponent<UISpriteAnimation>().Func_PlayPressUIAnim();
    }

    public void ConnectLine()
    {
        if (drawController.drawingStarted)                     // check and execute further only if the drawing has been already started
        {
            if (!isOrigin)                                     // to make sure connecting line is not the drawing start point
            {
                if (!connected)                                // if not connected already and to avoid the instantiation of multiple dots and lines
                {
                    if (GameObject.FindGameObjectWithTag("IsOrigin") != null)
                    {
                        GameObject lastConnectedElement = GameObject.FindGameObjectWithTag("IsOrigin");
                        
                        if (gameObject.GetComponentInParent<NeighbourElemnts>().Neighbours
                            .Contains(lastConnectedElement.transform.parent.gameObject))
                        {
                            connected = drawController.StartConnecting(dotPointPosition,element);
                            if (connected)
                            {
                               // GetComponent<UISpriteAnimation>().Func_PlayPressUIAnim();
                                gameObject.tag = "LastConnected";
                                if (GameObject.FindGameObjectWithTag("IsOrigin") != null)
                                {
                                    GameObject _origin = GameObject.FindGameObjectWithTag("IsOrigin");
                                   // _origin.gameObject.tag = "ToRemove";
                                   _origin.gameObject.tag = "ToRemove";
                                }
                            }
                        }
                        
                    }
                    
                    if (GameObject.FindGameObjectWithTag("LastConnected") != null)
                    {
                        GameObject lastConnectedElement = GameObject.FindGameObjectWithTag("LastConnected");
                        if (gameObject.GetComponentInParent<NeighbourElemnts>().Neighbours
                            .Contains(lastConnectedElement.transform.parent.gameObject))
                        {
                            connected = drawController.StartConnecting(dotPointPosition,element);
                            if (connected)
                            {
                                gameObject.tag = "LastConnected";
                                lastConnectedElement.tag = "ToRemove";
                            }
                        }
                    }
                   
                    
                    
                }
            }
        }
        
        /*
        if (drawController.drawingStarted)                     // check and execute further only if the drawing has been already started
        {
            if (!isOrigin)                                     // to make sure connecting line is not the drawing start point
            {
                if (!connected && CheckIsNeighbour())                                // if not connected already and to avoid the instantiation of multiple dots and lines
                {
                    //isReadytoRemove = true;
                    connected = drawController.StartConnecting(dotPointPosition,element);
                    if (connected)
                    {
                        gameObject.tag = "ToRemove";
                        if (GameObject.FindGameObjectWithTag("IsOrigin") != null)
                        {
                            GameObject _origin = GameObject.FindGameObjectWithTag("IsOrigin");
                            _origin.gameObject.tag = "ToRemove";
                        }
                        
//                        Debug.Log("Before: "+_origin.tag);
                       
                       // Debug.Log("After: "+_origin.tag);
                       // Debug.Log("connect_element "+element);
                    }
                    
                    
                }
            }
        }
        */
    }

    private bool CheckIsNeighbour()
    {
        Debug.Log("Element Name :");
        return true;
    }
    
    private void Update()
    {
        if (gameObject.CompareTag("IsOrigin"))
        {
          //  Debug.Log("Need to check connection happeing or not");
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
