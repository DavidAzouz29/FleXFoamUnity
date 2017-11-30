using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Extinguisher : MonoBehaviour
{

    // used to only handle collision now.

    InputController iCtrl;

    private void Start()
    {
        
        iCtrl = transform.root.GetComponent<InputController>();
    }


    // needs to be on the particle.
    void OnParticleCollision(GameObject other)
    {
        iCtrl.HandleCollision(other);
    }

}
