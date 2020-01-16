using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InnoculatingLoopBehaviour : MonoBehaviour
{

    public GramState currentLoopState;
    private Material thisLoopMaterial;

    // Start is called before the first frame update
    void Start()
    {
        thisLoopMaterial = GetComponentInParent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void Awake()
    {
        currentLoopState = GramState.clean;
    }

    void OnTriggerEnter(Collider collision)
    {
        CheckCollisionTag(collision);
    }

     void CheckCollisionTag(Collider InnoculatorCollider)
    {
        /* if (currentLoopState == GramState.clean)
        {
            if (InnoculatorCollider.tag == "GramPositive" || InnoculatorCollider.tag == "GramNegative")
            {
                ChooseGramState(InnoculatorCollider);
            }
        }
        else if (InnoculatorCollider.tag != currentLoopState.ToString() && InnoculatorCollider.tag != "GlassSlide")
        {
            Debug.Log("Loop Contaminated! Please Discard this loop and get a fresh one! ");
            Debug.Log(currentLoopState.ToString());
            thisLoopMaterial.color = Color.black;
        } */

        
        if (InnoculatorCollider.tag == "GramPositive" || InnoculatorCollider.tag == "GramNegative" && currentLoopState == GramState.clean)
        {
            if (currentLoopState == GramState.clean)
            {
                ChooseGramState(InnoculatorCollider);
                thisLoopMaterial.color = Color.yellow;
            }
            else if(InnoculatorCollider.tag != currentLoopState.ToString())
            {
                Debug.Log("Loop Contaminated! Please Discard this loop and get a fresh one! ");
                Debug.Log(currentLoopState);
                thisLoopMaterial.color = Color.black;
            }
        } 
    }

    void ChooseGramState(Collider smearedCollision)
    {
        if (smearedCollision.tag == "GramPositive")
        {
            currentLoopState = GramState.GramPositive;

            Debug.Log(currentLoopState);
        }
        else if (smearedCollision.tag == "GramNegative")
        {
            currentLoopState = GramState.GramNegative;
            Debug.Log(currentLoopState);
        }
        thisLoopMaterial.color = Color.yellow;
    }
}
