using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidDropBehaviour : MonoBehaviour
{
    public SoakedLiquidType DropLiquidType;

    // Start is called before the first frame update
    void Start()
    {
        switch (DropLiquidType)
        {
            case SoakedLiquidType.CrystalViolet:
                DropLiquidType = SoakedLiquidType.CrystalViolet;
                break;

            case SoakedLiquidType.Safrannin:
                DropLiquidType = SoakedLiquidType.Safrannin;
                break;

            default:
                DropLiquidType = SoakedLiquidType.None;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GlassSlide")
        {
            SetSlideLiquidState(other);
        }
    }

    public void SetSlideLiquidState(Collider liquidCollision)
    {
        liquidCollision.GetComponent<SlideStateMachine>().currentLiquid = DropLiquidType;
    }
}
