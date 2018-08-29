using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO make the potentially one main raycaster instead of having each object do its own raycast check
public class MemoryObject : MonoBehaviour {

    //object will always be the same index, the order in the memorygame is the thing that is changing
    public int m_Index;

    public Color m_StartColor;
    public Color m_LitColor;

    private Action<int> m_GameCallback;

    //needs to be awake for color change on first object to work. I don't know why
    private void Awake()
    {
        m_StartColor = GetComponent<MeshRenderer>().material.color;
    }

    private void Update()
    {
        //when player hits Mouse1
        if (Input.GetMouseButtonDown(0))
        {
            GameObject g = FP_Raycast.Update();

            if (g == gameObject)
            {
                //send main game script the object that was hit
                m_GameCallback.DynamicInvoke(m_Index);
            }
        }
    }

    //setting callback so when object is hit, it will notify main game
    public void SetCallback(Action<int> action)
    {
        m_GameCallback = action;
    }
}
