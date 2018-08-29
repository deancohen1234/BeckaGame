using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGameManager : MonoBehaviour {

    public Transform m_PuzzleParent; //used to determine the center of the artwork, and thus figure out if pieces are in the right location
    public Transform[] m_PuzzlePieces;

    public float m_PuzzleDistanceThreshold = 0.1f;

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Z))
        {
            if (CheckIfCorrect())
            {
                Debug.Log("Puzzle is Correct");
            }
            else
            {
                Debug.Log("Puzzle is Not Correct");
            }
        }
	}

    //if all puzzle pieces are correct then return true
    private bool CheckIfCorrect()
    {
        bool puzzleIsCorrect = true;

        foreach (Transform t in m_PuzzlePieces)
        {
            float distance = Vector3.Distance(m_PuzzleParent.position, t.position); //TODO could factor out need for parent and just check for local position == 0

            if (distance >= m_PuzzleDistanceThreshold)
            {
                puzzleIsCorrect = false;
                return puzzleIsCorrect;
            }
        }

        return puzzleIsCorrect;
    }
}
