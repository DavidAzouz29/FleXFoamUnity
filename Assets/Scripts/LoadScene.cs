using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            SceneManager.LoadScene(1);

    }
}
