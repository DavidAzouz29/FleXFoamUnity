using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExtinguisher : MonoBehaviour {

    public CapsuleCollider m_capsule;
    public Transform m_startPos;

    public float height;

    public bool debug = true;
	// Use this for initialization
	void Start () {
        if (!debug)
            m_capsule.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        m_capsule.height = height;
        UpdatePosition();
       
    }


    public void Activate(bool isActive)
    {
        if (isActive)
        {
            m_capsule.enabled = true;
        }
        else
        {
            m_capsule.enabled = false;
        }
    }

    public void ResizeLength(float flowAmount)
    {
        m_capsule.height = flowAmount;
    }
    public void ResizeWidth(float spread)
    {
        m_capsule.radius = spread;
    }

    void UpdatePosition()
    {
        // check how far behind the m_startpos the position is
        // move forward from that offset
        float zoffset = 0f + m_capsule.height / 2;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, zoffset);

        //m_pos = m_capsule.bounds.extents;
    }
}
