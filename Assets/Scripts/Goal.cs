using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public ParticleSystem GoalSystem;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MadeItGoal()
    {
        if (!GoalSystem.isPlaying)
        {
            GoalSystem.Play();
        }
    }
}
