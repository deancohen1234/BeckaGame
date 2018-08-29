using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastGetName : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButton("Fire1"))
        {
            string name = GetNameFromRaycast();

            Debug.Log("Raycaster hit: " + name);
        }
	}

    private string GetNameFromRaycast()
    {
        RaycastHit hit;

        string name = "";

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            name = hit.collider.gameObject.name;
        }

        return name;
    }
}
