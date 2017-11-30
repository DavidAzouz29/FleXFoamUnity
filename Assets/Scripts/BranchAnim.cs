///<summary>
/// Name: BranchAnim.cs
/// Author: David Azouz
/// Date Created: 14/10/17
/// Date Modified: 14/10/17
/// --------------------------------------------------
/// Brief:
/// viewed: 
/// https://twitter.com/DanielJMoran/status/727535792571064321
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 14/10/17
/// - Branch Anim based off UDP - David Azouz 17/10/17
/// - Water properties adjusted - David Azouz 18/10/17
/// 
/// TODO:
/// find perfect angle range
/// play anim based on UDP code
/// position player hand at right pos
/// </summary>

using UnityEngine;

//[ExecuteInEditMode]
public class BranchAnim : MonoBehaviour
{
    //[SerializeField]
    public InputController _inputController;
    public AnimationCurve TweenEase;

    [Range(0f, 100f)]
    public float baleHandleRotX;
    private float prevBaleHandleRotX;
    public Transform baleHandle;

    [Range(0f, 170f)] //TODO: find perfect angle range
    public float flowSelectRotZ;
    public Transform flowSelect;

    [Range(-55f, 360f)] //TODO: find perfect angle range
    public float fogStreamRotZ;
    public Transform fogStream;

    [Range(0f, 360f)]
    public float turbineRotZ;
    public Transform turbine;
    public float turbineRotSpeed = 360f;

    public float numOfSpokesBale = 10f; //TODO: is this right? Close enough
    public float numOfSpokesFlow = 14f; //small
    public float numOfSpokesFog = 13f;  //big
    const float deg = 360f;             // Total rotation (degrees)
    const float degRange = 170f;        // Total rotation (degrees) that spoke will turn
    const float degMulti = 36f;         // Deg to * to get from 0-1 to 0-360
    const float degMultiRange = 17f;    // Deg to * to get from 0-1 to 0-170
    public float slerpSpeed = 10f;
    //private bool isToUpdate = false;

    private float flowSelectRotZSNormal = 0; // Signed Normal. -1 to 1 range.
    private float fogStreamRotZSNormal = 0; // Signed Normal. -1 to 1 range.
    private float flowSelectRotZCurr = 0; // Current flow
    private float fogStreamRotZCurr = 0; // Current fog
    float[] flowRateCombUDPProtek = new float[] { 1f, 12f, 24f, 35f, 44f, 100f};
    float[] flowRateCombPos = new float[] { 332.2f, 310.3f, 285.2f, 266f, 237f, 212f}; // 0f, 22.5f, 45f, 76f, 82f, 173f
    float fTimer = 0f;
    private bool isXboxControllerUsed;
    bool isProtek = false;

    // Use this for initialization
    void Awake()
    {
        baleHandleRotX = 0f;
        flowSelectRotZ = 0f;
        fogStreamRotZ = 0f;
        turbineRotZ = 360f; //deg?

        prevBaleHandleRotX = baleHandleRotX;

        flowSelectRotZSNormal = 0;
        fogStreamRotZSNormal = 0;
        flowSelectRotZCurr = 0; //+= numOfSpokesFlow;
        fogStreamRotZCurr = 0;  //+= numOfSpokesFog;

        //Recode: use one or the other
        //isControllerUsed = GetComponent<BranchControllerDebug>().isActiveAndEnabled;
        isXboxControllerUsed = false;
    }

    private void Start()
    {
        //if (_inputController == null)
        //    _inputController = transform.GetComponent<BranchControllerDebug>()._inputController;
        if (_inputController == null)
            _inputController = transform.root.GetComponent<InputController>(); //GetComponentInParent?
        isProtek = ChangeWaterPropeties.GetBranchBrand();
    }

    // Update is called once per frame
    void Update()
    {
        #region Branch Rotations
        fTimer += Time.deltaTime;
        turbineRotZ -= turbineRotSpeed * Time.deltaTime;
        if (turbineRotZ <= 0f)
        {
            turbineRotZ = 360f; //deg?
        }
        // Branch Articulation
        // Bale Handle
        if (baleHandleRotX != prevBaleHandleRotX)
        {
            //baleHandleRotX = Mathf.Lerp(0, 100, baleHandleRotX);
            // TODO: position hand to bale... after isToUpdate performs?
            //isToUpdate = true;
            //_inputController.FireBranch(true);
        }

        // Recode: As UDP will take care of this?
        /*if (isToUpdate)
        {
            FlowAnimUpdate();
            FogAnimUpdate();
        } */

        // Turbine auto rotates
        turbine.localRotation = Quaternion.AngleAxis(turbineRotZ, Vector3.up);
        #endregion

        // TODO: recode?
        //FlowAnimUpdate(flowSelectRotZCurr);
        //FogAnimUpdate(fogStreamRotZCurr);

        float deltaTime = Time.deltaTime * slerpSpeed;
        baleHandle.localRotation = Quaternion.Slerp(
            baleHandle.localRotation,
            Quaternion.AngleAxis(baleHandleRotX, Vector3.right),
            TweenEase.Evaluate(deltaTime));
        flowSelect.localRotation = Quaternion.Slerp(
            flowSelect.localRotation,
            Quaternion.AngleAxis(flowSelectRotZ, Vector3.forward),
            TweenEase.Evaluate(deltaTime));
        fogStream.localRotation = Quaternion.Slerp(
            fogStream.localRotation,
            Quaternion.AngleAxis(fogStreamRotZ, Vector3.forward),
            TweenEase.Evaluate(deltaTime));
        //// Water Particle Count
        //if (isXboxControllerUsed)
        //{
        //    _inputController.SetWaterParticleEmissionRate((flowSelectRotZ / 10) * baleHandleRotX);
        //    _inputController.SetWaterParticleAngle(fogStreamRotZ / 10); // (fogStreamRotZ * -1 + 10) / deg); //TODO: FIX
        //}
        //if (fTimer >= 1) //TODO: fix hack?
        //{
        //    fTimer = 0;
        //    //isToUpdate = false;
        //    //prevBaleHandleRotX = baleHandleRotX;
        //}
    }

    #region Branch Rot
    bool Approximatly(float inputA, float inputB)
    {
        return Mathf.Abs(inputA - inputB) < 5f;
    }
    bool Approximatly(float inputA, float inputB, float a_tollerance)
    {
        return Mathf.Abs(inputA - inputB) < a_tollerance;
    }

    //TODO: Make Coroutine?
    // Protek Values to set will be 110, 230, 360, 470, 570, Flush
    //1f    , 12f   , 24f   , 35f   , 44f   , 100f 
    //332.2f, 310.3f, 285.2f, 266f  , 237f  , 212f // 0f, 22.5f, 45f, 76f, 82f, 173f
    public void FlowAnimUpdate(float a_flowRateComb)
    {
        // Flow Rate Comb Vive/ UDP Controller //TODO: confirm numbers are correct
        // Numbers from UDP aren't precise so hard setting it should solve this.
        #region UDP Set Rot
        // This is for Protek
        //ChangeWaterPropeties.E_BRANCH_STATE.E_BRANCH_STATE_PROTEK
        //flowSelectRotZ = Mathf.Lerp(332.2f, 212f, a_flowRateComb / 100f);
        for (int i = 0; i < flowRateCombUDPProtek.Length; ++i)
        {
            if (Approximatly(a_flowRateComb, flowRateCombUDPProtek[i], 6f))
            {
                flowSelectRotZ = flowRateCombPos[i];
				break; // No need to check the others if we found our match.
            }
            // Flush
            else if (a_flowRateComb >= flowRateCombUDPProtek[flowRateCombUDPProtek.Length - 2]) // Get's value 44f // 211f
            {
                flowSelectRotZ = flowRateCombPos[flowRateCombPos.Length - 1]; // 212f; //173f;
            }
        }
        #endregion

        if (isXboxControllerUsed)
        {
            // Flow
            if (flowSelectRotZSNormal != 0)
            {
                if (flowSelectRotZSNormal > 0)
                {
                    flowSelectRotZCurr += numOfSpokesFlow;
                    //TODO: hand increase anim

                }
                else if (flowSelectRotZSNormal < 0)
                {
                    flowSelectRotZCurr -= numOfSpokesFlow;
                    //TODO: hand decrease anim

                }
                flowSelectRotZSNormal = 0;
                // TODO: lock tracker offset if rotating, and move controller check to within.
                //isToUpdate = true;
            }
            flowSelectRotZ = Mathf.Clamp(flowSelectRotZCurr, 0, degRange); //
        }
        // Vive/ UDP
        //else
        //    flowSelectRotZ = flowSelectRotZCurr / 10;
        //_inputController.SetWaterParticleEmissionRate((flowSelectRotZ / 10) * 100 * baleHandleRotX); //TODO: simplify?
        //(flowSelectRotZ * baleHandleRotX) / 2);
        flowSelectRotZCurr = flowSelectRotZ;
    }

    //TODO: Make Coroutine?
    public void FogAnimUpdate(float a_angle)
    {
        // Set Anim Vive/ UDP
        // Numbers from UDP aren't precise so hard setting it should solve this.
        // Solid Stream
        #region UDP Set Rot
        // Protek
        if (isProtek)
            fogStreamRotZ = Mathf.Lerp(342f, 34.7f, a_angle / 100f); //83f, 0f
        // GForce
        else
            fogStreamRotZ = Mathf.Lerp(-34.7f, 315f, a_angle / 100f);//342

        /*if (Approximatly(a_angle, 100f))
        {
            fogStreamRotZ = 0f;
        }
        // Mid
        else if (Approximatly(a_angle, 77f, 10f))
        {
            fogStreamRotZ = 18f;
        }
        // Full fog
        else //if (Approximatly(a_angle, 17f))
        {
            fogStreamRotZ = 83f;
        }*/
        #endregion

        if (isXboxControllerUsed)
        {
            // Fog TODO: add code to prevent from going over 360d? Or will UDP/ irl branch handle this?
            if (fogStreamRotZSNormal != 0)
            {
                if (fogStreamRotZSNormal > 0)
                {
                    fogStreamRotZCurr += numOfSpokesFog;
                    //TODO: hand increase anim

                }
                else if (fogStreamRotZSNormal < 0)
                {
                    fogStreamRotZCurr -= numOfSpokesFog;
                    //TODO: hand decrease anim

                }
                fogStreamRotZSNormal = 0;
                //isToUpdate = true; //
            }
            fogStreamRotZ = Mathf.Clamp(fogStreamRotZCurr, 0, degRange); //
        }
        // Vive/ UDP
        //else
        //    fogStreamRotZ = fogStreamRotZCurr;
        //_inputController.SetWaterParticleAngle(fogStreamRotZ / degRange * -1 + 15f);// / deg) * 15); // (fogStreamRotZ * -1 + 10) / deg); //TODO: FIX
        fogStreamRotZCurr = fogStreamRotZ;
    }
    #endregion

    // Getters
    public bool GetIsXboxControllerUsed() { return isXboxControllerUsed; }
    public float GetBaleHandleRotX() { return baleHandleRotX; }
    public float GetFlowSelectRotZ() { return flowSelectRotZ; }
    public float GetFogStreamRotZ() { return fogStreamRotZ; }

    // Setters
    //public void SetIsToUpdate(bool a_isToUpdate) { isToUpdate = a_isToUpdate; }
    public void SetBaleHandleRotX(float a_baleHandleRotX) { baleHandleRotX = a_baleHandleRotX; }
    public void SetFlowSelectRotZ(float a_flowSelectRotZ) { flowSelectRotZ = a_flowSelectRotZ; }
    public void SetFlowSelectRotZSNormal(float a_flowSelectRotZClamp) { flowSelectRotZSNormal = a_flowSelectRotZClamp; }
    public void SetFlowSelectRotZCurr(float a_flowSelectRotZCurr) { flowSelectRotZCurr = a_flowSelectRotZCurr; }
    public void SetFogStreamRotZ(float a_fogStreamRotZ) { fogStreamRotZ = a_fogStreamRotZ; }
    public void SetFogStreamRotZSNormal(float a_fogStreamRotZClamp) { fogStreamRotZSNormal = a_fogStreamRotZClamp; }
    public void SetFogStreamRotZCurr(float a_fogStreamRotZCurr) { fogStreamRotZCurr = a_fogStreamRotZCurr; }
}
