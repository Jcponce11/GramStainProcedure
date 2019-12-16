using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRTK;

public class MicroScopeCameraSwitcher : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera noSlideCamera;
    public Camera cleanSlideCamera;
    public Camera smearedCamera;
    public int totalCams;
    public GameObject MicroscopeInteractable;

    

    void Start()
    {
        totalCams = Camera.allCamerasCount;
    }

    private void Update()
    {
        if (MicroscopeInteractable.GetComponent<VRTK_InteractableObject>().IsUsing() && MicroscopeInteractable.GetComponent<MicroscopeBehaviour>().isSlideLoaded)
        {
            Debug.Log("Slide Loaded!");
        }

        if (MicroscopeInteractable.GetComponent<VRTK_InteractableObject>().IsUsing())
        {
            firstPersonCamera.enabled = false;
            noSlideCamera.enabled = true;
            Debug.Log("No Slide Loaded!");

        }
        else
        {
            firstPersonCamera.enabled = true;
            noSlideCamera.enabled = false;
        }

    }




 /*   public void ShowCleanSlideView()
    {
        firstPersonCamera.enabled = false;
        cleanSlideCamera.enabled = true;
        smearedCamera.enabled = false;
    }

    public void ShowFirstPersonView()
    {

        firstPersonCamera.enabled = true;
        cleanSlideCamera.enabled = false;
        smearedCamera.enabled = false;
    }

    public void ShowSmearedSlideCamera()
    {
        firstPersonCamera.enabled = false;
        cleanSlideCamera.enabled = false;
        smearedCamera.enabled = true;
    }

    public void activateCamera(Camera parameterCam)
    {
        print(parameterCam.tag);
        disableAllCameras();
        switchCamera(parameterCam.tag);
    }

    public void disableAllCameras()
    {
        var CamerasInScene = new List<Camera> {firstPersonCamera, cleanSlideCamera, smearedCamera};
        foreach(Camera cam in CamerasInScene)
        {
            print(cam.name);
        }
    }

    public void switchCamera(string activeCamera)
    {
        switch (activeCamera)
        {
            case "FirstPersonCamera":
                ShowFirstPersonView();
                break;
            case "CleanSlideCamera":
                ShowCleanSlideView();
                break;
            case "SmearedSlideCamera":
                break;
            default:
                //some code 
                break;
        } 
    } */
}