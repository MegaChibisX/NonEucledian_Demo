using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Portal : MonoBehaviour
{

    public Material skyBox;

    public Camera cmr;
    public Color bckClr;

    public Portal otherSide;
    public Gravity gravity;

    protected MeshRenderer rend;

    private void Start()
    {
        cmr = GetComponentInChildren<Camera>();
        rend = GetComponentInChildren<MeshRenderer>();

        SetPrimary();
    }
    private void Update()
    {
        if (cmr.gameObject.activeSelf != rend.isVisible)
            cmr.gameObject.SetActive(rend.isVisible);
    }
    public void SetPrimary()
    {
        MeshRenderer othRend = otherSide.GetComponentInChildren<MeshRenderer>();
        GetComponentInChildren<CopyTransformRelative>().mainCmrPortal = GetComponentInChildren<MeshRenderer>().transform;
        GetComponentInChildren<CopyTransformRelative>().myPortal = othRend.transform;

        RenderTexture tex = new RenderTexture(1920, 1080, -1);
        cmr.targetTexture = tex;
        cmr.backgroundColor = otherSide.bckClr;

        rend.material.SetTexture("_MainTex", tex);

        return;
    }

}
