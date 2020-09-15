using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    ///refers to the cell on wich the builder is located
    public GameObject mCellObj;
    ///refers to the number of floor the builder is located on
    private int mFloor;

    // Start is called before the first frame update
    void Start()
    {
        mFloor = 0;
        //TODO : if mCellObj is null => select a random cell free instead
        if (mCellObj != null)
        {   
            //Reset the position of the builder based on the position of the cell
            Vector3 lCellPos = mCellObj.transform.position;
            Vector3 lNewPos = new Vector3(lCellPos.x, gameObject.transform.position.y, lCellPos.z);
            gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);

            //declare the cell as occupied
            Cell lCellScr = mCellObj.GetComponent<Cell>();
            lCellScr.mIsFree = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
