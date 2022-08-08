using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformRelative : MonoBehaviour
{

    public Transform mainCmr;
    public Transform mainCmrPortal;
    public Transform myPortal;


    private void Update()
    {
        Vector3 diff = Helper.TransformVectorFromTo(mainCmr.position - mainCmrPortal.position, mainCmrPortal, myPortal);
        diff = Helper.TransformScaleFromTo(diff, mainCmrPortal, myPortal);
        transform.position = myPortal.position + diff;

        Vector3 forw = mainCmr.forward;
        forw = mainCmrPortal.transform.InverseTransformDirection(forw);
        forw = myPortal.transform.TransformDirection(forw);
        Vector3 up = mainCmr.up;
        up = mainCmrPortal.transform.InverseTransformDirection(up);
        up = myPortal.transform.TransformDirection(up);
        transform.rotation = Quaternion.LookRotation(forw, up);

    }

}
