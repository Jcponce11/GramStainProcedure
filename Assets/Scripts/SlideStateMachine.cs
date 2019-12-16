using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlideState {
        clean, smeared, smearedAndHeateFixed, smearedAndViolet, violetFloodedAndWashed,
        violetWashedIodine, initialDecolorize, decolorizedAndWashed,
        decolorizedWashedAndSafranin, VioletIodineSafraninAndWashed, completeBlotted }

public enum GramState
{
   clean, gramPositive, gramNegative, gramVariable, gramIndeterminate
}

public class SlideStateMachine : MonoBehaviour
{

    public SlideState currentSlideState;
    public GramState currentGramState;
    public bool hasBeenHeatFixed;

    // Start is called before the first frame update
    void Start()
    {
        currentSlideState = SlideState.clean;
        currentGramState = GramState.clean;
        hasBeenHeatFixed = false;
    }
    // Update is called once per frame
    void Update()
    {
        

        
    }

    void OnTriggerEnter(Collider innoculation)
    {
        currentGramState = innoculation.gameObject.GetComponent<InnoculatingLoopBehaviour>().currentLoopState;
    }

    public void HeatFixSlide(bool other)
    {
        hasBeenHeatFixed = other;
    }
}
