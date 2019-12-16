using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class innoculationLoopBehaviour : MonoBehaviour
{
    public bool smearState;
    public bool? gramState;

    // Start is called before the first frame update
    void Start()
    {
        smearState = false;
        gramState = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
