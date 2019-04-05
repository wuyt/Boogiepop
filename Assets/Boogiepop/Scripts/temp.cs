using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour {

    public void OnPress()
    {
        Debug.Log("press");
    }

    public void OnRelease()
    {
        Debug.Log("release");
    }

    private void Update()
    {
        Debug.Log(Time.time);
    }
}
