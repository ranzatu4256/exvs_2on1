using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class score : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.CompareTag("win_blue"))
        {
            this.GetComponent<Renderer>().material.color = Color.blue;
        }

        if (this.CompareTag("win_red"))
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
