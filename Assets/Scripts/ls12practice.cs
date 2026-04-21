using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L12practiceCoding : MonoBehaviour
{
    int[] playerScores = { 450, 1200, 340, 890, 2100, 150, 780, 1050 };
    // Start is called before the first frame update
    void Start()
    {
       
        Debug.Log("First" + playerScores[0]);

        FindMinorMax();

    }

    void FindMinorMax()
    { 
    
    int mvpScore = playerScores[0];
    int needsPracticeScore = playerScores[0];

    for (int i = 0; i < playerScores.Length; i++)
    {
        if (playerScores[i] > mvpScore)
        {
            mvpScore = playerScores[i];
        }
        if (playerScores[i] < needsPracticeScore)
        {
            needsPracticeScore = playerScores[i];
        }
    
    }

    Debug.Log("MVP Score: " + mvpScore);
    Debug.Log("Needs Practice Score: " + needsPracticeScore);

    }
}