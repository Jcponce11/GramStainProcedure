using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCameraName : MonoBehaviour
{
    public Text camName;
        

    // Start is called before the first frame update
    void Start()
    {
        camName = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        camName.text = camName.name;
    }
}
