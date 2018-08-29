using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class MonkeyBallMain : MonoBehaviour {


    public GameObject m_MBPlatform;
    public GameObject m_MBBase;
    public GameObject m_MBCamera;
    public GameObject m_Player;

    private GameObject m_Camera;
    private bool freezePlayer = false;
    private Vector3 m_frozenLocation;

    // Use this for initialization
    void Start () {
        m_Camera = m_Player.GetComponentInChildren<Camera>().gameObject;
    }
	
	// Update is called once per frame
	void Update () {


        //Grab platform--
        if (Input.GetButtonDown("Fire1"))
        {
            //print("Raycast Object: " + FP_Raycast.Update());
            
            
            
            // ----- Interact with MB Game -----
            if (m_MBPlatform == FP_Raycast.Update())
            {
                float dist = Vector3.Distance(m_MBPlatform.transform.position, m_Player.transform.position);
                if (dist < 2)
                {
                    print("Found Platform!");

                    freezePlayer = true;
                    m_frozenLocation = m_Player.transform.position;

                    StartGame();
                }
                else
                {
                    print("User is not close enough to game");
                }
            }
        }

        

        if (freezePlayer)
        {
            m_Player.transform.position = m_frozenLocation;

            if (Input.GetKeyDown(KeyCode.E))
            {
                freezePlayer = false;
                StopGame();
            }

            //Z is leftRight    AD
            //X is forwardback    WS
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            float Hor = m_MBPlatform.transform.rotation.eulerAngles.z;          Hor += horizontal/10;
            float Vert = m_MBPlatform.transform.rotation.eulerAngles.x;         Vert -= vertical/10;
            float defaultY = 0;
            Vector3 Rotation = new Vector3(Vert, defaultY, Hor);

            m_MBPlatform.transform.rotation = Quaternion.Euler(Rotation);
        }
	}




    void StartGame()
    {

        //rotate Platform by X and Z based off of Keys A and D
        //float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        //float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        //print("Horizontal: " + horizontal);
        //print("Verticle: " + vertical);


    }

    void StopGame()
    {

    }
}









//Ideas: 
//Spawn a camera in the postion of current player camera. Lerp that to a good position.
//Lerp the player position in until player and pedistal touch, lock position/lp by changing WASD to rotate box instead.
//Lerp the player camera from player positon to set pedistal position, swap WASD

//m_Player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>(); //remove
//m_Player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>()
//Destroy (GetComponent (myScript ));
//GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().enabled = true;

//m_Player.transform.parent = this.gameObject.transform;
//m_Player.transform.position = m_MBCamera.transform.position;
//m_Player.transform.rotation = m_MBCamera.transform.rotation;


//m_Camera.transform.parent = this.gameObject.transform;
//m_Player.SetActive(false);
//HeadBob hb = m_Camera.GetComponent<HeadBob>() //Can't find headbob 

//m_Camera.transform.position = m_MBCamera.transform.position;
//m_Camera.transform.rotation = m_MBCamera.transform.rotation;

//Seperate the camera from the person, attach to base, set in position; 
