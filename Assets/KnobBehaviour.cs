using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobBehaviour : MonoBehaviour
{
    public Animator anim;
    bool isBurnerActive;
    // Start is called before the first frame update
    void Start()
    {
        isBurnerActive = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        isBurnerActive = !isBurnerActive;
        anim.Play("BunsenButtonPress");
    }
}
