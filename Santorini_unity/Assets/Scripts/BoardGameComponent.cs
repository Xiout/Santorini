using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameComponent : MonoBehaviour
{
    /// <summary>
    /// Material assigned at the first time
    /// Use to reset back to the original material after being deselected
    /// </summary>
    private Material mDefaultMaterial = null;

    /// <summary>
    /// getter of the default material : material assigned at the first time
    /// </summary>
    public Material getDefaultMaterial()
    {
        return mDefaultMaterial;
    }

    /// <summary>
    ///setter of the default material : material assigned at the first time
    ///if the material is already setted, it cannot be set again
    ///return : true if the material had been set with success, false if the material was already set before
    /// </summary>
    public bool setDefaultMaterial(Material pMat)
    {
        if(mDefaultMaterial == null)
        {
            mDefaultMaterial = pMat;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Set material of the BoardGameComponent as its mDefaultMaterial
    /// </summary>
    public void ResetMaterial()
    {
        ApplyMaterial(mDefaultMaterial);
    }

    /// <summary>
    /// Apply material on the BoardGameComponent
    /// </summary>
    /// <param name="pMat"></param>
    public void ApplyMaterial(Material pMat)
    {
        Renderer lRenderer = gameObject.GetComponent<Renderer>();
        lRenderer.enabled = true;
        lRenderer.sharedMaterial = pMat;
    }
}
