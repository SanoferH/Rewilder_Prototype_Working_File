using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NeighbourElemnts : MonoBehaviour
{
    [SerializeField] private DrawController drawController;
    [SerializeField] private GameObject UIMetaball;
    [SerializeField] private Transform MetaballParent;
    private GameObject UIMetaballElement;
    public GameObject nativeElement;
    public GameObject Flower;
    public GameObject Leaf;
    public GameObject Grass;
    public List<GameObject> Neighbours = new List<GameObject>();

    public List<GameObject> currentNeigbourElements = new List<GameObject>();

    public GameObject currentElement;

    public bool isFilled;
    // Start is called before the first frame update
    void Start()
    {
       // dot = Instantiate(dotPrefab, dotPosition.position, Quaternion.identity, dotParent);
        currentElement = Instantiate(nativeElement,gameObject.transform.position, Quaternion.identity, this.transform);
       isFilled = false;
       if (UIMetaball)
       {
           UIMetaballElement = Instantiate(UIMetaball, gameObject.transform.position, Quaternion.identity,
               MetaballParent);
       }
    }

    [Button]
    public void deleteCurrentElement()
    {
       // Destroy(currentElement.gameObject);
    }

    [Button]
    public void FillEmptySpace()        //Filling the empty space in Game board with nieghbouring game elements
    {
        
        
//            Debug.Log("Its empty here in the space..: "+gameObject.name);
            foreach (GameObject _neighbour in Neighbours)
            {
                   
               
                if (_neighbour.gameObject.transform.childCount == 1)                               // will consider the nieghbours that are not empty 'child count = 1'
                {
                    Debug.Log("Nieghbour Name: "+_neighbour.gameObject.name);
 //                   Debug.Log("Nieghbour Child Element :"+_neighbour.transform.GetChild(0));
                    currentNeigbourElements.Add(_neighbour.transform.GetChild(0).gameObject);
               
                }
                    
            }

            isFilled = true;
            //if number of currentNeigbourElements is just 1, then select that element to instantiate as currentElement
            
           
            
           
/*
            if (currentNeigbourElements.Count == 1)
            {
                Debug.Log("Going to Fill the next available element "+currentNeigbourElements[0].gameObject.name+" for the position "+gameObject.name);
               // currentElement = Instantiate(nativeElement,gameObject.transform.position, Quaternion.identity, this.transform);

               if (currentNeigbourElements[0].gameObject.name == "Flower")
               {
                   Debug.Log("Its available flower");
                   currentElement = Instantiate(Flower,gameObject.transform.position, Quaternion.identity, this.transform);
                   currentNeigbourElements.Clear();
               }
               
               if (currentNeigbourElements[0].gameObject.name == "Leaf")
               {
                   Debug.Log("Its available leaf");
                   currentElement = Instantiate(Leaf,gameObject.transform.position, Quaternion.identity, this.transform);
                   currentNeigbourElements.Clear();
               }
               
               if (currentNeigbourElements[0].gameObject.name == "Grass")
               {
                   Debug.Log("Its available grass");
                   currentElement = Instantiate(Grass,gameObject.transform.position, Quaternion.identity, this.transform);
                   currentNeigbourElements.Clear();
               }
            }
                  
            //if number of currentNeigbourElements is greater than 1, then randomly select one element from the list to instantiate as currentElement

            if (currentNeigbourElements.Count > 1)
            {
                Debug.Log("Going to Fill the random element "+ PickRandomElement().gameObject.name+" for the position "+gameObject.name);
                
                if (PickRandomElement().gameObject.name == "Flower")
                {
                    Debug.Log("Its random flower");
                    currentElement = Instantiate(Flower,gameObject.transform.position, Quaternion.identity, this.transform);
                    currentNeigbourElements.Clear();
                }
                if (PickRandomElement().gameObject.name == "Leaf")
                {
                    Debug.Log("Its random leaf");
                    currentElement = Instantiate(Leaf,gameObject.transform.position, Quaternion.identity, this.transform);
                    currentNeigbourElements.Clear();
                }
               
                if (PickRandomElement().gameObject.name == "Grass")
                {
                    Debug.Log("Its random grass");
                    currentElement = Instantiate(Grass,gameObject.transform.position, Quaternion.identity, this.transform);
                    currentNeigbourElements.Clear();
                }
            }
          */   

/*Working Properly

        if (currentNeigbourElements.Count == 1)
        {
            currentElement = Instantiate(PickRandomElement(),gameObject.transform.position, Quaternion.identity, this.transform);
            currentNeigbourElements.Clear();
        }

        if (currentNeigbourElements.Count == 2)
        {
          //  currentElement = Instantiate(Leaf,gameObject.transform.position, Quaternion.identity, this.transform);
            currentElement = Instantiate(PickRandomElement(),gameObject.transform.position, Quaternion.identity, this.transform);
            currentNeigbourElements.Clear();
        }
        if (currentNeigbourElements.Count == 3)
        {
           // currentElement = Instantiate(Flower,gameObject.transform.position, Quaternion.identity, this.transform);
           currentElement = Instantiate(PickRandomElement(),gameObject.transform.position, Quaternion.identity, this.transform);
            currentNeigbourElements.Clear();
        }
        
        if (currentNeigbourElements.Count == 4)
        {
           // currentElement = Instantiate(Grass,gameObject.transform.position, Quaternion.identity, this.transform);
           currentElement = Instantiate(PickRandomElement(),gameObject.transform.position, Quaternion.identity, this.transform);
            currentNeigbourElements.Clear();
        }
        */

        if (currentNeigbourElements.Count >= 1)
        {
            currentElement = Instantiate(PickRandomElement(),gameObject.transform.position, Quaternion.identity, this.transform);
            currentNeigbourElements.Clear();
        }
    }

    private GameObject PickRandomElement()
    {
        GameObject _randomElement = new GameObject();
        int elementIndex = Random.Range(0, currentNeigbourElements.Count);
        _randomElement = currentNeigbourElements[elementIndex];
        return _randomElement;
    } 
    
    
    // Update is called once per frame
    void Update()
    {

        if (!drawController.drawController_IsFilled)
        {
            isFilled = false;
        }
        
        if (currentElement == null)
        {
            if (UIMetaballElement != null)
            {
                UIMetaballElement.GetComponent<MetaballMovements>().DeletePatch();
            }
            if (!isFilled)
            {
                FillEmptySpace(); 
            }
            
           
        } 
        /*
        if (currentElement == null && !isFilled)
        {
            FillEmptySpace();
            if (UIMetaball)
            {
                UIMetaballElement.GetComponent<MetaballMovements>().DeletePatch();
            }
        } 
        */ 
        /*
        if (drawController.refreshGrid)
        {
            if (transform.GetComponentInChildren<ButtonBehaviour>().isReadytoRemove)
            {
                //Destroy(currentElement);
                Debug.Log("Refreshing :"+transform.GetChild(1).parent.name);
                transform.GetChild(1).GetComponent<ButtonBehaviour>().canDelete = true;
                
               // Destroy(transform.GetChild(1).gameObject);
               // drawController.refreshGrid = false;
            }
          

        }
        */

   
    }
}
