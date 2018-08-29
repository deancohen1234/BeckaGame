using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//game where each round you increase the number of things you have to remember
//will have dynamic number of objects for memory
//memoryobject will callback to game when it has either been hit successfully in time, or failed
//memorygame will handle the order and judge whether order is correct
//memoryobject will handle player interaction and send to memorygame hits


public class MemoryGame : MonoBehaviour {

    public MemoryObject[] m_MemoryObjects;
    public int m_NumObjectsToRemember = 3;
    public float m_TimeToSeeMemory = 1f;
    public float m_TimeBetweenMemory = 1f;

    private int m_GameOrderIndex = 0; //index of the GameOrder
    private bool m_OrderIsBeingShown = false; //when order is being shown stop player from interacting with board

    private int[] m_GameOrder; //order the player needs to hit the objects in (should be of length numMemoryObjects)

    private List<int> m_CurrentPlayerOrder; //the order the player is hitting the memory object in


    #region Initialization Methods
    void Start()
    {
        //InitializeObjects(); //assign memory objects their respective indices
        //ShuffleGameOrder(); //shuffle the game order to make a random pattern
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            InitializeObjects();
            ShuffleGameOrder();
        }
    }

    private void InitializeObjects()
    {
        m_CurrentPlayerOrder = new List<int>();

        m_GameOrder = new int[m_NumObjectsToRemember];

        for (int i = 0; i < m_MemoryObjects.Length; i++)
        {
            MemoryObject mo = m_MemoryObjects[i];
            mo.m_Index = i;
            mo.SetCallback(OnObjectHit);

            //m_GameOrder[i] = mo.m_Index;
        }

        for (int i = 0; i < m_GameOrder.Length; i++)
        {
            int arrIndex = Random.Range(0, m_MemoryObjects.Length);

            m_GameOrder[i] = m_MemoryObjects[arrIndex].m_Index;
        }
    }

    private void ShuffleGameOrder()
    {
        m_GameOrderIndex = 0;
        //shuffle the order of the numbers in the array
        for (int i = 0; i < m_GameOrder.Length; i++)
        {
            int temp = m_GameOrder[i];
            int rand = Random.Range(0, m_GameOrder.Length - 1);
            m_GameOrder[i] = m_GameOrder[rand];
            m_GameOrder[rand] = temp;
        }

        //clear objects from being lit
        for (int i = 0; i < m_MemoryObjects.Length; i++)
        {
            SetLitObject(m_MemoryObjects[i], false);
        }

        //set starting memory object
        //SetLitObject(GetMemoryObject(0), true);
        m_OrderIsBeingShown = true;

        StartCoroutine(ShowFailure());
    }
    #endregion

    #region Updating Methods
    //called when memory object is hit
    private void OnObjectHit(int objectIndex)
    {
        if (m_OrderIsBeingShown) { return; } //if order is being shown, don't let player interact

        m_CurrentPlayerOrder.Add(objectIndex);
        SetLitObject(GetMemoryObject(m_GameOrderIndex), true);

        bool b = CheckIfCorrectOrder(objectIndex);

        //if player picked the correct number then try and go to next one
        if (b)
        {
            TryGotoNextObject();
        }

        else
        {
            //player screwed up and needs to restarts
            ResetGame();
        }
    }

    //if true then player currently has hit blocks in right order
    //this also assumes that previous hits were correct
    private bool CheckIfCorrectOrder(int objectIndex)
    {
        //get player recent hit index from most recent hit
        int playerRecentHit = m_CurrentPlayerOrder.Count - 1;

        //if player hit correct number in order
        if (m_CurrentPlayerOrder[playerRecentHit] == m_GameOrder[m_GameOrderIndex])
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    //try and move to next object in order
    private void TryGotoNextObject()
    {        

        //increase to the next object in the order
        m_GameOrderIndex++;

        //if player got through entire order
        if (m_GameOrderIndex >= m_GameOrder.Length)
        {
            WinGame();
            return;
        }

    }

    private void SetLitObject(MemoryObject mo, bool lightUp)
    {
        if (lightUp)
        {
            mo.gameObject.GetComponent<MeshRenderer>().material.color = mo.m_LitColor;
        }
        else
        {
            mo.gameObject.GetComponent<MeshRenderer>().material.color = mo.m_StartColor;
        }
    }

    private void WinGame()
    {
        Debug.Log("Player won the game");

        for (int i = 0; i < m_MemoryObjects.Length; i++)
        {
            SetLitObject(m_MemoryObjects[i], true);
            m_MemoryObjects[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    private void ResetGame()
    {
        //re-shuffle game order
        ShuffleGameOrder();

        //clear the player's previous order
        m_CurrentPlayerOrder.Clear();
    }

    #endregion

    #region Replay Memory Methods
    //called not in update
    //Done with coroutine

    private IEnumerator ShowFailure() //TODO put this in a resetGame function and maybe don't daisy chain if possible
    {
        m_OrderIsBeingShown = true;

        for (int i  = 0; i < m_MemoryObjects.Length; i++)
        {
            m_MemoryObjects[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        yield return new WaitForSeconds(2);

        StartCoroutine(PlayOrder());
    }

    private IEnumerator PlayOrder()
    {
        //if we have gone through whole order then bail out of function
        if (m_GameOrderIndex >=  m_GameOrder.Length)
        {
            m_GameOrderIndex = 0;

            StopCoroutine(PlayOrder());

            m_OrderIsBeingShown = false;

            yield break;
        }
        //player is able to see object
        MemoryObject currentMem = GetMemoryObject(m_GameOrderIndex);
        SetLitObject(currentMem, true);
        yield return new WaitForSeconds(m_TimeToSeeMemory);

        //player not able to see memory
        SetLitObject(currentMem, false);
        yield return new WaitForSeconds(m_TimeBetweenMemory);

        //increment to next object in order
        m_GameOrderIndex++;

        StartCoroutine(PlayOrder());
    }
    #endregion

    #region Helper Methods
    //gets memoryObject in the order from num
    private MemoryObject GetMemoryObject(int orderNum)
    {
        return m_MemoryObjects[m_GameOrder[orderNum]];
    }
    #endregion
}