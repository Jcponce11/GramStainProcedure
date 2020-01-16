using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlideState {
        clean, smeared, smearedAndHeatFixed, smearedAndViolet, violetFloodedAndWashed,
        violetWashedIodine, initialDecolorize, decolorizedAndWashed,
        decolorizedWashedAndSafranin, VioletIodineSafraninAndWashed, completeBlotted }

public enum SoakedLiquidType
{
    None, Water, CrystalViolet, Safrannin, Iodine, GramsAlcohol
}

public enum GramState
{
   clean, GramPositive, GramNegative, gramVariable, gramIndeterminate
}

public class SlideStateMachine : MonoBehaviour
{

    public SlideState currentSlideState;
    public GramState currentGramState;
    public bool hasBeenHeatFixed;
    private bool isCurrentlySoaked;
    public SoakedLiquidType currentLiquid;
    private Material thisSlideMaterial;

    // Start is called before the first frame update
    void Start()
    {
        currentSlideState = SlideState.clean;
        currentGramState = GramState.clean;
        hasBeenHeatFixed = false;
        isCurrentlySoaked = false;
        currentLiquid = SoakedLiquidType.None;
        thisSlideMaterial = GetComponent<Renderer>().material;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void GramStateChange(Collider innoculation)
    {
        currentGramState = innoculation.gameObject.GetComponent<InnoculatingLoopBehaviour>().currentLoopState;

        if (currentGramState != GramState.clean)
        {
            thisSlideMaterial.color = Color.yellow;
            currentSlideState = SlideState.smeared;
        }

    }

    void OnTriggerEnter(Collider innoculation)
    {
        GramStateChange(innoculation);
    }

    public void HeatFixSlide(bool other)
    {
        hasBeenHeatFixed = other;
    }
}
