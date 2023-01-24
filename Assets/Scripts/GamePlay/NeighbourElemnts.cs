using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourElemnts : MonoBehaviour
{
    [SerializeField] private DrawController drawController;
    public GameObject nativeElement;
    public List<GameObject> Neighbours = new List<GameObject>();

    public GameObject currentElement;
    // Start is called before the first frame update
    void Start()
    {
       // dot = Instantiate(dotPrefab, dotPosition.position, Quaternion.identity, dotParent);
        currentElement = Instantiate(nativeElement,gameObject.transform.position, Quaternion.identity, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (drawController.refreshGrid)
        {
            if (transform.GetComponentInChildren<ButtonBehaviour>().isOrigin)
            {
                Destroy(currentElement);
            }

            if (transform.GetComponentInChildren<ButtonBehaviour>().connected)
            {
                Destroy(currentElement);
                //drawController.refreshGrid = false;
            }
            
        }
    }
}
