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
    

    // Start is called before the first frame update
    void Start()
    {
        isBurnerActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        ActivateFlameParticleSystem();
        ActivateFlameHeat();
    }

    private void ActivateFlameHeat()
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
        }
        else if (BunsenRotater.GetNormalizedValue() < .75f)
        {
            isBurnerActive = false;
            particleSystemReference.Stop();
        }
    }
}
