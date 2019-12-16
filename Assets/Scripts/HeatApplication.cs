using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatApplication : MonoBehaviour
{
    float heatCapacity;
    // Start is called before the first frame update
    void Start()
    {
        heatCapacity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        heatCapacity++;
    }

    private void OnTriggerExit(Collider other)
    { 
        while(heatCapacity > 0)
        {
           heatCapacity--;
        }
    }
}
