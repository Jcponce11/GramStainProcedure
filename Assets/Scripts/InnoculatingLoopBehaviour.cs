using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InnoculatingLoopBehaviour : MonoBehaviour
{

    public GramState currentLoopState;


    // Start is called before the first frame update
    void Start()
    {
        currentLoopState = GramState.clean;   
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private void OnTriggerEnter(Collider collision)
    {
        SetGramState(collision);
    }

    private void SetGramState(Collider InnoculatorCollider)
    {
        if (InnoculatorCollider.tag == "GramPositive")
        {
            currentLoopState = GramState.gramPositive;
            Debug.Log(currentLoopState);
        }
        else if (InnoculatorCollider.tag == "GramNegative")
        {
            currentLoopState = GramState.gramNegative;
            Debug.Log(currentLoopState);
        }
    }
}
