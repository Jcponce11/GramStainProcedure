using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunsenBurner : MonoBehaviour
{
    public bool BurnerOn;
    public ParticleSystem particleSystemReference;

    // Start is called before the first frame update
    void Start()
    {
        BurnerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleBurner();
    }

    private void ToggleBurner()
    {
        if (BurnerOn)
            particleSystemReference.Play();
        if (!BurnerOn)
            particleSystemReference.Stop();
    }
}
