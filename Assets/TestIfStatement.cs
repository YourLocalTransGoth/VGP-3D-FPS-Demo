using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIfStatement : MonoBehaviour
{



    public int[] array;
    public int evenCount = 0;
    public int oddCount = 0;

    void PrintsHowManyOddandEven(int num1)
    {
        Debug.Log(array[0]);

        
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] % 2 == 0)
            {
                Debug.Log(array[i] + " is even");
                evenCount++;
            }
            else
            {
                Debug.Log(array[i] + " is odd");
                oddCount++;
            }
        }
        Debug.Log("Total Evens: " + evenCount);
        Debug.Log("Total Odds: " + oddCount);
    }




    public static int factorial(int num2)
    {
        int ans = 1;
        for (int i = 1; i <= num2; i++)
        {
        ans += i;
        }
        return ans;
    
    }
    
    public void fact()
    {
        int n = num;
        Debug.Log(factorial(n));
    }


    public string legalDrinkingAge()
    { 
        if (num >= 19)
        {
            return "yes";
        }
        else
        {
            return "no";
        }
    }


    public int num;

    int CalculateAge()
    {
        
        int CalPlus5 = num + 5;
        return CalPlus5;
    }

    void Start()
    {
        int finalAge = CalculateAge();
        Debug.Log("Age is: "+ num + "Drinking: " + legalDrinkingAge());
        Debug.Log("Factorial: " + factorial(num));
        PrintsHowManyOddandEven(num);
        Debug.Log("Number of evens: " + evenCount);
        Debug.Log("Number of odds: " + oddCount);
    } 


    void printAge()
    {
        num = 5;
        Debug.Log("Hello World" + num);
    }


    void Update()
    {

    }
}
