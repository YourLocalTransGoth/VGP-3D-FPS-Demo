using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodePrac : MonoBehaviour
{

        public int input = 5;


    // Start is called before the first frame update
    void Start()
    {
        solution();
    }

    // Update is called once per frame
    void solution()
    {
        int answer = input * 60;

        Debug.Log(input + " minute is " + answer + " seconds"); 
    }
}
