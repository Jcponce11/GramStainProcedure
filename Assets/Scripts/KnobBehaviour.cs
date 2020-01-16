using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;
using VRTK.Controllables.ArtificialBased;

public class KnobBehaviour : MonoBehaviour
{
    public bool isBurnerActive;
    [SerializeField] VRTK_ArtificialRotator BunsenRotater;
    public ParticleSystem particleSystemReference;
    [SerializeField] GameObject HotArea;
    private GameObject indicatorMaterialObject;
    

    // Start is called before the first frame update
    void Start()
    {
        isBurnerActive = false;
        indicatorMaterialObject = GameObject.Find("Indicator");
    }

    // Update is called once per frame
    void Update()
    {
        ActivateFlameParticleSystem();
        ActivateFlameHeat();
        Debug.Log(indicatorMaterialObject);
    }

    void ActivateFlameHeat()
    {
        if (isBurnerActive)
            HotArea.SetActive(true);
        else
            HotArea.SetActive(false);
    }

    void ActivateFlameParticleSystem()
    {
        if (BunsenRotater.GetNormalizedValue() > .75f)
        {
            isBurnerActive = true;
            particleSystemReference.Play();
            indicatorMaterialObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (BunsenRotater.GetNormalizedValue() < .75f)
        {
            isBurnerActive = false;
            particleSystemReference.Stop();
            indicatorMaterialObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
