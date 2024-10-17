using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaleUIActions : MonoBehaviour
{
    void Update()
    {
        bool isQPressed = Input.GetKeyDown(KeyCode.Q);
        if (isQPressed) { 
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}
