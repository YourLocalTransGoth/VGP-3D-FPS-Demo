using UnityEngine;

public class practiceL10 : MonoBehaviour
{
    private void Start()
    {
        string text = " racecar";
        string text2 = "canada";
        string text3 = "12321";
        bool isPalindrome = IsPalindrome(text);
        Debug.Log(text + " is palindrome: " + isPalindrome);

        isPalindrome = IsPalindrome(text2);
        Debug.Log(text2 + " is palindrome: " + isPalindrome);

        isPalindrome = IsPalindrome(text3);
        Debug.Log(text3 + " is palindrome: " + isPalindrome);
    }

    private bool IsPalindrome(string s)
    {
        string cleaned = "";

        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsWhiteSpace(s[i]))
            {
                cleaned += char.ToLowerInvariant(s[i]);
            }
        }

        int left = 0;
        int right = cleaned.Length - 1;

        while (left < right)
        {
            if (cleaned[left] != cleaned[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }
}