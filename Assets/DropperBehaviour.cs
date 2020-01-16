using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DropperBehaviour : MonoBehaviour
{
    public VRTK_InteractableObject dropperObject;
    private bool switchGravity;
    public GameObject liquidDrop;
    public Transform dropSpawnPoint;
    public float dropSpeed = 100f;
    public float dropLife = 5f;

    // Start is called before the first frame update


    protected virtual void OnEnable()
    {
        dropperObject = (dropperObject == null ? GetComponent<VRTK_InteractableObject>() : dropperObject);

        if (dropperObject != null)
        {
            dropperObject.InteractableObjectUsed += InteractableObjectUsed;
        }
    }

    protected virtual void OnDisable()
    {
        if(dropperObject != null)
        {
            dropperObject.InteractableObjectUsed -= InteractableObjectUsed;
        }
    }

    protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        SqueezeDropper();
    }

    protected virtual void SqueezeDropper()
    {
        if(liquidDrop != null && dropSpawnPoint != null)
        {
            GameObject clonedDrop = Instantiate(liquidDrop, dropSpawnPoint.position, dropSpawnPoint.rotation);
            Rigidbody dropperRigidBody = clonedDrop.GetComponent<Rigidbody>();
            float destroyTime = 0f;
            if(dropperRigidBody != null)
            {
                dropperRigidBody.AddForce(clonedDrop.transform.forward * dropSpeed);
                destroyTime = dropLife;
            }
            Destroy(clonedDrop, destroyTime);
        }
    }
}
