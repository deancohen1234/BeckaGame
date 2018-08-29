using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualController : MonoBehaviour {

    public Material m_NoiseMaterial;
    public float m_ChangeSpeed = 0.2f;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		if (Input.GetKey(KeyCode.Comma))
        {
            PingPongValue("_ClipAmount");
        }
	}

    void PingPongValue(string propertyName)
    {
        float pingPongValue = Mathf.PingPong(Time.time * m_ChangeSpeed, 1);

        m_NoiseMaterial.SetFloat(propertyName, pingPongValue);
    }
}
