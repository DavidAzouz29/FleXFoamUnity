using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class InputController : MonoBehaviour
{

    // handles the extinguisher; does the same as the old extinguisher script however, doesn't handle collisions.

    [SerializeField]
    float m_moistureAdd = 1f;
    [SerializeField]
    float m_heatReduction = 15f;
    [SerializeField]
    public ParticleSystem pr;
    [SerializeField]
    public ParticleSystem prSteam;
    [SerializeField]
    public AudioSource fireHose;
    [SerializeField]
    SteamVR_TrackedObject ctrl;

    private Statistics stats;
    private float waterSec;
    [SerializeField]
    bool isWaterOnDebug = true;
    [SerializeField]
    ParticleExtinguisher m_exting;

    [SerializeField]
    MeshRenderer m_blackScreen;
    SteamVR_Controller.Device input;
    //[SerializeField]
    //float triggerAxis;
    bool isXboxControllerUsed = false;
    public BranchAnim _branchAnim; //TODO: remove bc UDP?
    float branchFireAmount = 0;
    float flowSelect;
    float fogStream;


    [SerializeField]
    bool m_usingBranchControls = true;
    private void Start()
    {
        stats = FindObjectOfType<Statistics>();// ("StatisticsCanvas");
        FailStateManager.Instance.m_dimPanel = m_blackScreen;
        m_blackScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        if (_branchAnim == null)
            _branchAnim = GetComponentInChildren<BranchAnim>(); // This is broken - fix for final

        if (_branchAnim != null)
            isXboxControllerUsed = _branchAnim.GetIsXboxControllerUsed();
    }

    void Update()
    {
        //float triggerValue = 0;

        if (Input.GetKeyDown("e"))
        {
#if UNITY_EDITOR
            print("keyboard fire");
#endif
            FireBranch(true);
        }

        // Returning null; encapsulated until fixed - Remus & James.
        if (_branchAnim != null || m_usingBranchControls)
        {
            // Branch
            // If a debug xbox controller is used
            if (isXboxControllerUsed)
            {
                branchFireAmount = Input.GetAxis("BranchFire");
                flowSelect = Input.GetAxis("FlowSelect");
                fogStream = Input.GetAxis("FogStream");
            }
            if (branchFireAmount > 0.25f || Input.GetButtonDown("Fire1") || isWaterOnDebug)
            {
                FireBranch(true);
                //Debug.Log("Fire Branch Yes: " + _inputController.isActiveAndEnabled);
            }
            else if (branchFireAmount <= 0.25f || Input.GetButtonUp("Fire1") || !isWaterOnDebug)
            {
                FireBranch(false);
            }
            if (isXboxControllerUsed)
            {
                _branchAnim.SetBaleHandleRotX(branchFireAmount * 100f); //0-100 range
#if UNITY_EDITOR
                Debug.Log("Branch " + branchFireAmount);
#endif

                // Signed Normal. -1 to 1 range.
                // Flow 
                if (flowSelect != 0)
                {
                    _branchAnim.SetFlowSelectRotZSNormal(flowSelect);
#if UNITY_EDITOR
                    Debug.Log("Flow " + flowSelect);
#endif
                    //_inputController.SetWaterParticleEmissionRate(((flowSelect / 100) * 10) * branchFireAmount);
                }
                // Fog
                if (fogStream != 0)
                {
                    _branchAnim.SetFogStreamRotZSNormal(fogStream);
#if UNITY_EDITOR
                    Debug.Log("Fog " + fogStream);
#endif
                    //_inputController.SetWaterParticleAngle((fogStream / 360) * 15); // (fogStreamRotZ * -1 + 10) / deg); //TODO: FIX
                }
            }
        }
        else if (SteamVR_Controller.Input((int)ctrl.index) != null)
        {
            // Trigger TODO: test
            input = SteamVR_Controller.Input((int)ctrl.index);
            //triggerAxis = input.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
            //triggerValue = 1 - triggerAxis;
            if (input.GetTouch(Valve.VR.EVRButtonId.k_EButton_Axis1))
            {
                FireBranch(true);
                m_exting.Activate(true);
            }
            else
            {
                FireBranch(false);
                m_exting.Activate(false);
            }
            //_branchAnim.SetBaleHandleRotX(triggerValue * 100f); //0-100 range //TODO: remove bc UDP?
        }
    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!

    public void FireBranch(bool a_isInputFired)
    {

        if (a_isInputFired)
        {
            ////pr.enableEmission = true;
            //var axis = SteamVR_Controller.Input((int)ctrl.index).GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1);
            //prSteam = a_prSteam; //pr = a_extinguisher.pr;
            pr.Play(true);
            prSteam.Play(true);
            //fireHose = a_fireHose;
            if (!isXboxControllerUsed)
            {
                if (_branchAnim != null)
                    branchFireAmount = _branchAnim.GetBaleHandleRotX() / 100f;
                //flowSelect = _branchAnim.GetFlowSelectRotZ() / 100f;
                //fogStream = _branchAnim.GetFogStreamRotZ() / 100f;
            }
            // Volume based off Branch and Flow rate; branchFireAmount * flowSelect / 4f divide by 2 then 2 again or else it's too loud
            fireHose.volume = Mathf.Lerp(fireHose.volume, branchFireAmount / 2f, Time.deltaTime); 
            fireHose.pitch = Random.Range(0.99f, 1.01f);
            //adding 1 second of water time to stats 

            //waterSec = a_waterSec;
            waterSec += 1 * Time.deltaTime;
            if (waterSec >= 1)
            {
                waterSec = 0;
                //stats = a_stats;
                stats.waterTime += pr.emission.rateOverTime.constant / 100f; //TODO: reaffirm this is working as intended - FLUSH function will cause problems
            }
        }
        else
        {
            //prSteam = a_prSteam; 
            pr.Stop(true);

            prSteam.Stop(true);
            fireHose.volume -= 1 * Time.deltaTime;
        }
    }

    public void SetWaterOn(bool a_isWaterOn)
    {
        if (isWaterOnDebug != a_isWaterOn)
            isWaterOnDebug = a_isWaterOn;
    }

    public void SetWaterParticleEmissionRate(float a_emissionRate)
    {
        var psEmission = pr.GetComponent<ParticleSystem>().emission;
        psEmission.rateOverTime = 110f + a_emissionRate;
        // Collision ps
        psEmission = prSteam.GetComponent<ParticleSystem>().emission;
        psEmission.rateOverTime = a_emissionRate * 0.10f;
    }

    public void SetWaterParticleAngle(float a_angle)
    {
        var psShape = pr.GetComponent<ParticleSystem>().shape;
        psShape.angle = a_angle;
        // Collision ps
        psShape = prSteam.GetComponent<ParticleSystem>().shape;
        psShape.angle = a_angle;
    }

    public void HandleCollision(GameObject other)
    {

        // attempt to fetch the objects meshcollider if it has one
        FlamableObject flameObject = other.GetComponent<FlamableObject>();
        BLEVE_Heating bleveObject = other.GetComponent<BLEVE_Heating>();
        if (flameObject == null && bleveObject == null)
            return;
        else if (flameObject != null)
        {
            flameObject.StopCoroutine(flameObject.IsBeingExtinguished());
            flameObject.m_moisture += m_moistureAdd;
            flameObject.StartCoroutine(flameObject.IsBeingExtinguished());
#if UNITY_EDITOR
            Debug.Log("IsHitting");
#endif
        }
        else if (bleveObject != null)
        {
            bleveObject.ReduceHeat(m_heatReduction);
#if UNITY_EDITOR
            Debug.Log("is Calling");
#endif
        }
    }
}
