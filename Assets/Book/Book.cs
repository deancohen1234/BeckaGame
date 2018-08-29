using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

    //backPage is the page that (if the page is starting on the left), is the back side of the animated page

    [Header("Main Properties")]
    public Animator m_Animator;
    public Texture[] m_Pages;

    [Header("Paper Materials")]
    public Material m_FrontPageMat;
    public Material m_BackPageMat;
    public Material m_LeftPageMat;
    public Material m_RightPageMat;

    private int m_CurrentPageSet; //one page set is a set of two pages. ex: page 34 and 35 are one page set 17
	// Use this for initialization
	void Start ()
    {
        m_CurrentPageSet = 0;
        m_LeftPageMat.SetTexture("_MainTex", m_Pages[0]);
        m_RightPageMat.SetTexture("_MainTex", m_Pages[1]);
        m_BackPageMat.SetTexture("_MainTex", m_Pages[0]);
        m_FrontPageMat.SetTexture("_MainTex", m_Pages[0]); //front and left don't matter as at this point we are on the first page and can't go back
    }
	
	// Update is called once per frame
	void Update ()
    {
        //left to right
		if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsAnimationComplete())
            {
                m_CurrentPageSet -= 1;

                //so we don't go to far back in book where pages don't exist
                if (m_CurrentPageSet < 0)
                {
                    m_CurrentPageSet = 0;
                    return;
                }

                m_Animator.Play("FlippingPage", 0, 0); //restarts then plays animation

                ModifyPageMaterials(m_CurrentPageSet, true);
            }
            
        }

        //right to left
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (IsAnimationComplete())
            {
                m_CurrentPageSet += 1;

                int numPages = m_CurrentPageSet * 2;

                //if you are at end of the book
                if (numPages > m_Pages.Length - 1)
                {
                    m_CurrentPageSet--;
                    return;
                }

                m_Animator.Play("FlippingPageReverse", 0, 0); //restarts then plays animation

                ModifyPageMaterials(m_CurrentPageSet, false);
            }

        }
    }

    private bool IsAnimationComplete()
    {
        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.normalizedTime > 1 || stateInfo.normalizedTime < 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private void ModifyPageMaterials(int currentPageSetIndex, bool isLeftToRight)
    {
        //go backwards in book
        if (isLeftToRight)
        {

            int previousRightPage = ((currentPageSetIndex + 1) * 2) + 1; //get the previous right 
            int previousLeftPage = ((currentPageSetIndex + 1) * 2); //get the previous left 

            Debug.Log("Right page: " + previousRightPage);

            int newLeftPage = currentPageSetIndex * 2;
            int newRightPage = newLeftPage + 1;


            m_FrontPageMat.SetTexture("_MainTex", m_Pages[previousLeftPage]);
            m_BackPageMat.SetTexture("_MainTex", m_Pages[newRightPage]);

            m_RightPageMat.SetTexture("_MainTex", m_Pages[previousRightPage]);
            m_LeftPageMat.SetTexture("_MainTex", m_Pages[newLeftPage]);
        }

        //right to left
        //go forwards in book
        else
        {

            int previousRightPage = ((currentPageSetIndex - 1) * 2) + 1; //get the previous right 
            int previousLeftPage = ((currentPageSetIndex - 1) * 2); //get the previous left 

            int newLeftPage = currentPageSetIndex * 2;
            int newRightPage = newLeftPage + 1;


            m_FrontPageMat.SetTexture("_MainTex", m_Pages[newLeftPage]);
            m_BackPageMat.SetTexture("_MainTex", m_Pages[previousRightPage]);

            m_RightPageMat.SetTexture("_MainTex", m_Pages[newRightPage]);
            m_LeftPageMat.SetTexture("_MainTex", m_Pages[previousLeftPage]);
        }
    }
}
