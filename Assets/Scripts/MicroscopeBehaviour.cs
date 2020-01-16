using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MicroscopeBehaviour : MonoBehaviour
{
    public bool isSlideLoaded;
    private VRTK_SnapDropZone snapDropZoneToCheck;
    private GameObject currentSlideStateMachine;
    private SlideState loadedSlideState;
    private GramState loadedSlideGramState;


    // Start is called before the first frame update
    void Start()
    {
        InitialSlideLoadCheck();
        InitializeSnapDropZoneVariable();
        InitialCheckForSnappedSlide();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMicroScopeSnapDropZone();
    }

    /// <summary>
    /// Constantly Checks the Microscope Drop Zone for a snapped object.
    /// </summary>///
 
    private void CheckMicroScopeSnapDropZone()
    {
        if (snapDropZoneToCheck.isSnapped)
        {
            SetSlideLoaded(true);
            RetrieveSlideInfo();
        }
        else
        {
            SetSlideLoaded(false);
            loadedSlideState = SlideState.clean;
            ResetMicroscopeState();
        }
    }

    /// <summary>
    /// Reads data from Slide that has been placed into the microscope.
    /// </summary>

    private void RetrieveSlideInfo()
    {
        currentSlideStateMachine = GetComponentInChildren<VRTK_SnapDropZone>().GetCurrentSnappedObject();
        loadedSlideState = currentSlideStateMachine.GetComponent<SlideStateMachine>().currentSlideState;
        loadedSlideGramState = currentSlideStateMachine.gameObject.GetComponent<SlideStateMachine>().currentGramState;
    }

    /// <summary>
    /// Cleans up the state of the Microscope once a slide has been removed.
    /// </summary>
    private void ResetMicroscopeState()
    {
        currentSlideStateMachine = null;
        loadedSlideGramState = GramState.clean;
    }

    /// <summary>
    /// Sets the variable denoting whether or not a slide is currently loaded into the Microscope.
    /// </summary>
    /// <param name="isSlideLoaded"></param>
    /// 
    private void SetSlideLoaded(bool load)
    {
        isSlideLoaded = load;
    }

    /// <summary>
    /// Initializes the variable for the slide State Machine on the microscope, which is always unloaded on start.
    /// </summary>
    private void InitialCheckForSnappedSlide()
    {
        currentSlideStateMachine = GetComponentInChildren<VRTK_SnapDropZone>().GetCurrentSnappedObject();
    }

    private void InitializeSnapDropZoneVariable()
    {
        snapDropZoneToCheck = GetComponentInChildren<VRTK_SnapDropZone>();
    }

    private void InitialSlideLoadCheck()
    {
        isSlideLoaded = false;
    }
}
