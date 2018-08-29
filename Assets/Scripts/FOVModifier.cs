using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVModifier : MonoBehaviour {

    public GameObject character; 
    public GameObject cam;

    private float FOVWalk;
    private float FOVRun = 80;
    private float FOVTimer = 0;
    private bool FOVSwitch = false;

    public AnimationCurve RunCurve = new AnimationCurve(new Keyframe(0, 60f), new Keyframe(1, 80f));


    // Use this for initialization
    void Start () {
        FOVWalk = cam.GetComponent<Camera>().fieldOfView;
        //RunCurve.AddKey(0, FOVWalk);
        //RunCurve.AddKey(1, FOVRun);
    }
	
	// Update is called once per frame
	void Update () {

        //float speed = character.GetComponent<Rigidbody>().velocity.magnitude;
        //Debug.Log("Char Velocity: " + speed);
        Debug.Log("Curve Platform: " + RunCurve.Evaluate(FOVTimer));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FOVWalk, FOVRun, FOVTimer);
            cam.GetComponent<Camera>().fieldOfView = RunCurve.Evaluate(FOVTimer);
            FOVTimer += 3 * Time.deltaTime;

            FOVTimer = Mathf.Clamp01(FOVTimer);
        }

        else
        {
            //cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FOVWalk, FOVRun, FOVTimer);
            cam.GetComponent<Camera>().fieldOfView = RunCurve.Evaluate(FOVTimer);
            FOVTimer -= 2 * Time.deltaTime;

            FOVTimer = Mathf.Clamp01(FOVTimer);
        }
        
    }
}