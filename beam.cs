using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beam : MonoBehaviour
{
    public GameObject MS;
    public float lapsetime = 0.0f;
 
    // Update is called once per frame
    void Update()
    {
        Vector3 dirToGo = Vector3.zero;
        Rigidbody rBody = GetComponent<Rigidbody>();
        rBody.velocity = Vector3.zero;
        
        if(this.CompareTag("attack"))
        {
            lapsetime += 1.0f;
            dirToGo = transform.forward;

            if(lapsetime >= 60)
            {
                this.tag = "locked";
            }
                        
            else if(lapsetime >= 0)
            {
                rBody.AddForce(dirToGo * 8.0f, ForceMode.VelocityChange);
            }
        } 
        else if(this.CompareTag("locked"))
        {
            transform.rotation = MS.transform.rotation;
            this.transform.localPosition = MS.transform.position;

            if (MS.CompareTag("attack"))
            {
                lapsetime = 0.0f;
                this.tag = "attack";
                MS.tag = "attacking";
            }
        }
    }
}
