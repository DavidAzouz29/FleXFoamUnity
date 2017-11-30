using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendHeat : MonoBehaviour {
    [SerializeField]
    float m_fireHeat = 600f;


	// Use this for initialization

    void OnParticleCollision(GameObject other)
    {

        BLEVE_Heating BH = other.transform.GetComponent<BLEVE_Heating>();
        if (BH != null)
        {
            BH.AddHeat(m_fireHeat);
        }

    }
}
