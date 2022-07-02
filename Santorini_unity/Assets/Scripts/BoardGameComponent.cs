using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameComponent : MonoBehaviour
{
    ///Material assigned at the first time
    ///(use to reset properly the original material after being deselected)
    private Material mDefaultMaterial = null;

    ///getter of the default material : material assigned at the first time
    public Material getDefaultMaterial()
    {
        return mDefaultMaterial;
    }

    ///setter of the default material : material assigned at the first time
    ///if the material is already setted, it can be set again
    ///return : true if the material had been set with success, false if the material was already set before
    public bool setDefaultMaterial(Material pMat)
    {
        if(mDefaultMaterial == null)
        {
            mDefaultMaterial = pMat;
            return true;
        }
        return false;
    }

    //Set material of the BoardGameComponent as its mDefaultMaterial
    public void ResetMaterial()
    {
        ApplyMaterial(mDefaultMaterial);
    }

    public void ApplyMaterial(Material pMat)
    {
        Renderer lRenderer = gameObject.GetComponent<Renderer>();
        lRenderer.enabled = true;
        lRenderer.sharedMaterial = pMat;
    }
}
