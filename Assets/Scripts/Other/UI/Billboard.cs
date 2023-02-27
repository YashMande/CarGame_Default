using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamTransform;
    // Start is called before the first frame update
    void Start()
    {
        mainCamTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamTransform.rotation* Vector3.forward, mainCamTransform.rotation * Vector3.up);

    }
}
