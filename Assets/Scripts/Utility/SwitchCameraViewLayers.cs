using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Summary:    Switches the camera layers to change what can be seen.
/// Developer:  Remus Jones
/// Date:       06/11/2017
/// </summary>
public class SwitchCameraViewLayers : MonoBehaviour {

    public LayerMask m_cameraOnLayers;
	// Use this for initialization
	void Start () {
        this.GetComponent<Camera>();
        GameObject.FindObjectOfType<FailStateManager>().OnGameEnd += SwitchOn;
	}
	
    /// <summary>
    /// Call when the game ends.
    /// </summary>
    public void SwitchOn()
    {
        Camera.main.cullingMask = m_cameraOnLayers;
    }
}
