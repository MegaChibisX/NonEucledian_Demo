using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{

    public static Vector3 TransformVectorFromTo(Vector3 vec, Transform from, Transform to)
    {
        vec = from.InverseTransformDirection(vec);
        vec = to.TransformDirection(vec);

        return vec;
    }
    public static Vector3 TransformScaleFromTo(Vector3 vec, Transform from, Transform to)
    {
        //vec = from.InverseTransformDirection(vec);
        //vec = to.TransformDirection(vec);

        vec = Vector3.Scale(vec,
               new Vector3(to.lossyScale.x / from.lossyScale.x,
                           to.lossyScale.y / from.lossyScale.y,
                           to.lossyScale.z / from.lossyScale.z));

        return vec;
    }

}
