using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MetaballMovements : MonoBehaviour
{
    [SerializeField] private float DiminishTime;

    [SerializeField] private GameObject Bubbles;

    [SerializeField] private GameObject MetaballParent;
    
    [SerializeField] private float variableSizeRange = 100.0f;
    
    [SerializeField] private float timeValueRange = 2.0f;
    public float randomSizeRange;
    public float timeRange;

    private float currentSize;
    // Start is called before the first frame update
    void Start()
    {
       // DiminishTime = 1.5f;
        //  Debug.Log(gameObject.GetComponent<RectTransform>().rect.width);
        currentSize = gameObject.GetComponent<RectTransform>().rect.width;
        randomSizeRange = Random.Range(60.0f, variableSizeRange);
        timeRange = Random.Range(1.0f, timeValueRange);
        WaveEffect();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeRandomSize();
    }

    [Button]
    public void DeletePatch()
    {
        RectTransform rt = GetComponent<RectTransform>();
        LeanTween.size(rt, new Vector2(-400.0f,-400.0f), DiminishTime).setEaseOutQuint();
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(DeletePatchObject()); // to avoid the error by calling an inactive object
        }
        
    }

    IEnumerator DeletePatchObject()
    {
        yield return new WaitForSeconds(DiminishTime);
       // Destroy(this.gameObject);  // instead of destroying making this object disable helps to solve the patch glitch 
       this.gameObject.SetActive(false);
    }

    
    [Button]
    public void WaveEffect()
    {
        RectTransform rt = GetComponent<RectTransform>();
        LeanTween.size(rt, new Vector2(currentSize + randomSizeRange, currentSize + randomSizeRange), timeRange)
            .setLoopPingPong();
    }

    public void ChangeRandomSize()
    {
        StartCoroutine(ChangeSize());
    }

    IEnumerator ChangeSize()
    {
        yield return new WaitForSeconds(1.5f);
        randomSizeRange = Random.Range(30.0f, variableSizeRange);
    }
   
}
