using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    public List<Transform> points;
    
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        points = new List<Transform>();
    }
    public void AddPoint(Transform point)
    {
        lr.positionCount++;
        points.Add(point);
    }

    public void RemovePoint(Transform point)
    {
//        Debug.Log("Removing: "+point.position);
        points.Remove(point);
        lr.positionCount--;
    }

    public void RefreshGrid()
    {
        points.Clear();
        lr.positionCount = 0;
    }

    [Button]
    public void testClear()
    {
        Debug.Log("Clearing..");
        points.Clear();
    }
    
    private void LateUpdate()
    {
        if (points.Count >= 2)
        {
            for (int i = 0; i < points.Count; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
    }
}
