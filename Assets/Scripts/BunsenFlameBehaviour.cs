using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunsenFlameBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "GlassSlide")
        {
            Debug.Log("Collision Detected");
            if (other.gameObject.GetComponent<SlideStateMachine>().currentGramState == GramState.clean)
                other.gameObject.GetComponent<SlideStateMachine>().HeatFixSlide(false);
            else
                other.gameObject.GetComponent<SlideStateMachine>().HeatFixSlide(true);
        }
    }
}
