using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class SwitchToggle : MonoBehaviour {

    [SerializeField]
    private bool mouseControl = true;
    private Material switchMaterial = null;
    [SerializeField]
    private bool buttonOn = false;
    [SerializeField]
    private Color32 toggledEmissiveColor = Color.gray;
    [SerializeField]
    private bool toggleEmissiveMap = false;
    private Texture emmissiveMap = null;

    private Color startColor;


	// Use this for initialization
	void Start () {
        switchMaterial = GetComponent<Renderer>().material;
        startColor = switchMaterial.GetColor("_EmissionColor");
        emmissiveMap = switchMaterial.GetTexture("_EmissionMap");  
        if (!buttonOn)
        {
            if (toggleEmissiveMap)
            {
                switchMaterial.SetTexture("_EmissionMap", null);
            }
            else
            {
                switchMaterial.SetColor("_EmissionColor", startColor);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (mouseControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RayCast();
            }
        }
    }

    private void RayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 500) == true)
        {
            //Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.transform.position.z);
            // check for hits
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.transform.gameObject == gameObject)
            {
                if (buttonOn)
                {
                    if(toggleEmissiveMap)
                    {
                        switchMaterial.SetTexture("_EmissionMap", emmissiveMap);
                    }
                    else
                    {
                        switchMaterial.SetColor("_EmissionColor", toggledEmissiveColor);
                    }
                    buttonOn = false;
                }
                else
                {
                    if (toggleEmissiveMap)
                    {
                        switchMaterial.SetTexture("_EmissionMap", null);
                    }
                    else
                    {
                        switchMaterial.SetColor("_EmissionColor", startColor);
                    }
                    buttonOn = true;
                }
            }
        }
    }
}
