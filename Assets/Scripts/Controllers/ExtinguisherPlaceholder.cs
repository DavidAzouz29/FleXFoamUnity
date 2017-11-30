using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherPlaceholder : MonoBehaviour {

    [SerializeField]
    float m_waterTemp = 20f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButton(0))
        {
            
            BLEVE_Heating objectHit = hit.transform.GetComponent<BLEVE_Heating>();
            if (objectHit != null)
            {
                objectHit.ReduceHeat(m_waterTemp);
                
            }
        }
    }
}
