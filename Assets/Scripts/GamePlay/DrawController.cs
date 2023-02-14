using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawController : MonoBehaviour
{
    [Header("Dots")] 
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotParent;
    
    [Header("Lines")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform lineParent;

    [Header("Metaball")]
    [SerializeField] private GameObject MetaballPanel;
    private LineController currentLine;
    private GameObject dot;
    private GameObject next_dot;
    private GameObject currentDot;
    private Transform dotPosition;
    private Transform next_dotPosition;
    public bool drawingStarted;
    private int dotNumber;

    private string origin_element;

    private string next_element;
    
    //Tutorial Level1
   
    private bool _levelCompleted;
    private bool _level2Completed;
    private bool _level1GameCompleted;
    public bool refreshGrid;
    public bool drawController_IsFilled;

    private bool connectedNextPoint;
    
    [Header("Level 1 Tutorial")]
    [SerializeField] private CanvasGroup flowerCanvasGroup;

    [SerializeField] private CanvasGroup message1Box;
    
    [SerializeField] private CanvasGroup message2Box;
    
    [SerializeField] private GameObject messge2BoxGameObject;

    [SerializeField] private GameObject lineParentGameObject;
    
    
    //
    private void Awake()
    {
        drawingStarted = false;
        dotNumber = 0;
        refreshGrid = false;
        connectedNextPoint = false;
        //Tutorial Level1
        _levelCompleted = false;
        message2Box.alpha = 0;
        messge2BoxGameObject.SetActive(false);
        //
    }

    public void StartDrawing(Transform dotPoint,string _element)            //Start drawing line from dotPosition
    {
        dotPosition = dotPoint;
        origin_element = _element;
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent).GetComponent<LineController>();
        currentLine.tag = "ToRemove";
        dot = Instantiate(dotPrefab, dotPosition.position, Quaternion.identity, dotParent);
        dot.name = "Origin Dot";
        dot.tag = "ToRemove";
        //Determine the origin Button element

        if (dotPosition.GetComponentInParent<FlowerIdentifier>())
        {
          //  Debug.Log("Starting with a Flower");
        }
        
        //
        currentLine.AddPoint(dot.transform);
        currentDot = Instantiate(dotPrefab, dotPosition.position, Quaternion.identity, dotParent);
        currentDot.name = "Current Dot";
        currentDot.tag = "ToRemove";
        currentLine.AddPoint(currentDot.transform);
        drawingStarted = true;
    }
    
    public bool StartConnecting(Transform dotPoint, string _element)         //connect the line with next_dotPosition
    {
      //  Debug.Log("StartConnecting,,");
        next_dotPosition = dotPoint;
        next_element = _element;
       // Debug.Log("origin_element "+origin_element);
       // Debug.Log("next_element "+next_element);
        if (next_element != origin_element)
        {
            return false;
        }
        next_dot = Instantiate(dotPrefab, next_dotPosition.position, Quaternion.identity, dotParent);
        dotNumber++;
        next_dot.name = "Dot " + dotNumber;
        next_dot.tag = "ToRemove";
        //Determine the next Button element

        if (next_dot.GetComponentInParent<FlowerIdentifier>())
        {
            Debug.Log("Continuing with a Flower");
        }
        
        //
        
        currentLine.AddPoint(next_dot.transform);
        currentLine.RemovePoint(currentDot.transform);
        Destroy(currentDot);
        currentDot = Instantiate(dotPrefab, dotPosition.position, Quaternion.identity, dotParent);
        currentDot.name = "Current Dot";
        currentLine.AddPoint(currentDot.transform);
        connectedNextPoint = true;
        //Below code lines are just for Tutorial Level 1
        _levelCompleted = true;
        
        //
        
        return true;
    }
    
    private void Update()
    {
        if (drawingStarted)
        {
            if (Input.GetMouseButton(0))
            {
                currentDot.transform.position = GetMousePosition();
            }

            if (Input.GetMouseButtonUp(0))
            {
                currentLine.RemovePoint(currentDot.transform);
                
                Destroy(currentDot);
                drawingStarted = false;
                if (!connectedNextPoint)
                {
                    Destroy(dot);                   //removes the origin dot if not connected to next point
                  Debug.Log("Destroyed the origin");
                    
                    // currentLine.RefreshGrid();
                   // Destroy(currentLine);
                    // connectedNextPoint = false;
                }

                if (SceneManager.GetActiveScene().name == "Tutorial Level 1")
                {
                    if (_levelCompleted)
                    {
                        Level1Completed();
                    }
                }

                if (SceneManager.GetActiveScene().name == "Tutorial Level 2")
                {
                    if (_level2Completed)
                    {
                        
                    }
                    else
                    {
//                        Debug.Log("Delete these and instantiate new and start connecting more");

                        //Removing the finished Game Elements
                        GameObject[] _elementsToRemove = GameObject.FindGameObjectsWithTag("ToRemove");

                        foreach (GameObject _elements in _elementsToRemove)
                        {
                            Destroy(_elements.gameObject);
                        }

                        GameObject lastConnectedElement = GameObject.FindGameObjectWithTag("LastConnected");
                        Destroy(lastConnectedElement.gameObject);

                        drawController_IsFilled = false;
                        //Instantiating the new Game Elements


                        // Destroy(currentLine);

                        /*
                        if (currentLine.points.Count >= 2)
                        {
                            
                            currentLine.RefreshGrid();
                            refreshGrid = true;
                            foreach (Transform child in dotParent)
                            {
                                Destroy(child.gameObject);
                            }
                        }
                        */

                        // refreshGrid = true;
                    }
                    
                }

                if (SceneManager.GetActiveScene().name == "Level1Game")
                {
                    if (_level1GameCompleted)
                    {
                        
                    }
                    else
                    {
                        GameObject[] _elementsToRemove = GameObject.FindGameObjectsWithTag("ToRemove");

                        foreach (GameObject _elements in _elementsToRemove)
                        {
                            Destroy(_elements.gameObject);
                        }

                        GameObject lastConnectedElement = GameObject.FindGameObjectWithTag("LastConnected");
                        Destroy(lastConnectedElement.gameObject);

                        drawController_IsFilled = false;
                    }
                }
               
            }
        }

        if (MetaballPanel.transform.childCount == 0)
        {
            MetaballPanel.SetActive(false);
        }
    }
    
    private Vector3 GetMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = dotPosition.position.z;
        return worldMousePosition;
    }

    private void Level1Completed()
    {
      //  Debug.Log("Level1 Completed");
        flowerCanvasGroup.LeanAlpha(0, 1.0f).setEaseOutQuart();
        message1Box.LeanAlpha(0, 1.0f).setEaseOutQuart();
        messge2BoxGameObject.SetActive(true);
        lineParentGameObject.SetActive(false);
        message2Box.LeanAlpha(1, 2.5f).setEaseOutQuart();
    }
}
