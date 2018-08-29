using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO make one player controller class that does the player controller things
public class LazerConduit : MonoBehaviour {

    public bool m_IsStartingLazer; //is this lazer for main activator
    public float m_RotateSpeed; //the speed at which the conduits rotate
    public bool m_Charged = false; //is the lazer currently activated

    public Transform m_Player;
    public ParticleSystem m_LazerSystem;

    private bool m_IsSelected; //is the player currently selecting this conduit
    private Vector3 m_FrozenPosition; //the player's locked position when they select a conduit
    private Color m_StartingColor; //starting color of cube when not activated

    private LazerConduit m_ActivatedLazerConduit; //the conduit that this conduit it currently activating
	// Use this for initialization
	void Start ()
    {
		if (m_IsStartingLazer == true)
        {
            TryActivateLazer();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        ManageInput();

        if (m_IsSelected)
        {
            m_Player.position = m_FrozenPosition;
            OrientLazer();
        }

        if (m_Charged)
        {
            RaycastLazer();
        }
    }

    private void RaycastLazer()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            //if ray hits a conduit
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Lazer"))
            {
                LazerConduit conduit = hit.collider.gameObject.GetComponent<LazerConduit>();

                if (conduit == null) { return; }

                //if true then successful activation and we can set activated conduit
                if (conduit.TryActivateLazer()) 
                {
                    m_ActivatedLazerConduit = conduit;
                }
            }

            //if ray is hitting the goal
            else if (hit.collider.gameObject.tag == "Goal")
            {
                hit.collider.gameObject.GetComponent<Goal>().MadeItGoal();
            }

            //if you are not hitting a conduit
            else
            {
                if (m_ActivatedLazerConduit != null)
                {
                    m_ActivatedLazerConduit.DeActivateLazer();
                }
            }

            
        }
        //if connection between conduits breaks
        else
        {
            if (m_ActivatedLazerConduit != null)
            {
                m_ActivatedLazerConduit.DeActivateLazer();
            }
        }
    }

    private void ManageInput()
    {
        //select conduit
        if (Input.GetMouseButtonDown(0))
        {
            GameObject g = FP_Raycast.Update();

            if (g == gameObject)
            {
                m_IsSelected = true;
                m_FrozenPosition = m_Player.position;
            }
        }

        //deselect conduit
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_IsSelected = false;
        }
    }

    private void OrientLazer()
    {
        float keyboardX = Input.GetAxis("Horizontal") * m_RotateSpeed * Time.deltaTime;
        float keyboardY = Input.GetAxis("Vertical") * m_RotateSpeed * Time.deltaTime * -1; //-1 because it feels better to have the lazer rotate not inverted

        transform.Rotate(keyboardY, keyboardX, 0);
    }

    //return false if lazer hit is already activated or can not be activated
    public bool TryActivateLazer()
    {
        //conduit already is being charged
        if (m_Charged == true)
        {
            return false;
        }

        GameObject baseCube = transform.GetChild(0).gameObject;
        baseCube.GetComponent<MeshRenderer>().material.color = Color.red;

        m_Charged = true;

        if (!m_LazerSystem.isPlaying)
        {
            m_LazerSystem.Play();
        }

        return true;

    }

    //deactivate this lazer as well as a potential lazer that it activating
    public void DeActivateLazer()
    {
        m_Charged = false;
        m_LazerSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        GameObject baseCube = transform.GetChild(0).gameObject;
        baseCube.GetComponent<MeshRenderer>().material.color = Color.grey;

        // if lazer is activating another lazer then turn that lazer off
        if (m_ActivatedLazerConduit != null)
        {
            m_ActivatedLazerConduit.DeActivateLazer();
            m_ActivatedLazerConduit = null;
        }
    }
}
