///<summary>
/// Name: BranchAnim.cs
/// Author: Sameer Deshpande
/// Date Created: ??/10/17
/// Date Modified: 16/10/17
/// --------------------------------------------------
/// Brief: UDP Receive Branch data + Change flow rate combined + emission rate & emission angle
/// viewed: 
/// 
/// --------------------------------------------------
/// Edits:
/// - Script Created - Sameer Deshpande 16/10/17
/// - Branch Anim - David Azouz 17/10/17
/// 
/// TODO:
/// 
/// </summary>

using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using System.Windows;
//using System.Windows.Threading;


public class ChangeWaterPropeties : MonoBehaviour
{

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // public
    //public string IP = "127.0.0.1"; //default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!

    public ParticleSystem[] psEffectsArray; // Holds water ps
    //[SerializeField] private ParticleSystem psEffectCurr;
    //[SerializeField] private ParticleSystem psSteamEffectCurr; // Used for collisions
    public static ParticleSystem ps;
    public static ParticleSystem psCol; // Used for collisions
    //float angle = 5f;
    //float variation = 5f;
    //int rate = 1000;
    [SerializeField] float Angle;
    [SerializeField] float Flowrate;
    [SerializeField] float FlowRate_Comb;
    float prevFlowRateComb;
    float prevfogStream;
    float flowSelectSNormal; // Signed Normal between -1 and 1
    float fogStreamSNormal;  // Signed Normal between -1 and 1
    float fFlushSetting = 44f; // If over this number, we're on the flush setting
    public static ParticleSystem.ShapeModule _shapeModule;
    public static ParticleSystem.ShapeModule _shapeModuleCol;
    public static ParticleSystem.EmissionModule _emissionModule;
    public static ParticleSystem.EmissionModule _emissionModuleCol;
    bool isTrackerOnBranch = true; // is tracker physically attached to branch irl.
    //bool isFreezeRot = false;
    static private bool isProtek;
    public BranchAnim[] _branchesAnim;
    private BranchAnim _branchAnim;
	public enum E_BRANCH_STATE
    {
        E_BRANCH_STATE_PROTEK_TRACKER,
        E_BRANCH_STATE_PROTEK_VIVE,
        E_BRANCH_STATE_GFORCE_TRACKER,
        E_BRANCH_STATE_GFORCE_VIVE,
        E_BRANCH_STATE_COUNT,
    };
    [SerializeField]
    private E_BRANCH_STATE m_eCurrState = E_BRANCH_STATE.E_BRANCH_STATE_PROTEK_TRACKER;

    bool isXboxController = false;
    // start from shell
    private static void Main()
    {
        ChangeWaterPropeties receiveObj = new ChangeWaterPropeties();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    
    // start from unity3d
    public void Start()
    {
        // Turns of PS based on controller type
        switch (m_eCurrState)
        {
            case E_BRANCH_STATE.E_BRANCH_STATE_PROTEK_TRACKER:
            case E_BRANCH_STATE.E_BRANCH_STATE_GFORCE_TRACKER:
                {
                    psCol = psEffectsArray[0];
                    ps = psEffectsArray[1];
                    if(m_eCurrState == E_BRANCH_STATE.E_BRANCH_STATE_GFORCE_TRACKER)
                        isTrackerOnBranch = true;
                    else
                        isTrackerOnBranch = false;
                    //isFreezeRot = false;
                    break;
                }
            case E_BRANCH_STATE.E_BRANCH_STATE_PROTEK_VIVE:
            case E_BRANCH_STATE.E_BRANCH_STATE_GFORCE_VIVE:
                {
                    psCol = psEffectsArray[2];
                    ps = psEffectsArray[3];
                    isTrackerOnBranch = false;
                    //isFreezeRot = false;
                    break;
                }
            case E_BRANCH_STATE.E_BRANCH_STATE_COUNT:
                break;
            default:
                break;
        }
        switch (m_eCurrState)
        {
            case E_BRANCH_STATE.E_BRANCH_STATE_PROTEK_TRACKER:
            case E_BRANCH_STATE.E_BRANCH_STATE_PROTEK_VIVE:
                isProtek = true;
                break;
            case E_BRANCH_STATE.E_BRANCH_STATE_GFORCE_TRACKER:
            case E_BRANCH_STATE.E_BRANCH_STATE_GFORCE_VIVE:
                isProtek = false;
                break;
            case E_BRANCH_STATE.E_BRANCH_STATE_COUNT:
                break;
            default:
                break;
        }

        // Water
        //ps = psEffectCurr;//.GetComponent<ParticleSystem>();
        _shapeModule = ps.shape;
        _emissionModule = ps.emission;
        Angle = _shapeModule.angle; //TODO: ??
        // Water Collision ps
        //psCol = psSteamEffectCurr;
        _shapeModuleCol = psCol.shape;
        _emissionModuleCol = psCol.emission;
        Angle = _shapeModuleCol.angle; //TODO: ??

        prevFlowRateComb = -1;
        prevfogStream = -1;
        flowSelectSNormal = 0;
        fogStreamSNormal = 0;
        init();
        if (_branchAnim == null)
            _branchAnim = _branchesAnim[(int)m_eCurrState];
        //turn off all other Branches that are not being used
        for (int i = 0; i < _branchesAnim.Length; i++)
        {
            if(i != (int)m_eCurrState)
                _branchesAnim[i].gameObject.SetActive(false);
        }
        this.StartCoroutine(this.PerformChanges());
        isXboxController = _branchAnim.GetIsXboxControllerUsed();
    }

    // init
    private void init()
    {

        //print("UDPSend.init()");

        // define port
        port = 11000;//8051;

        // status
        //print("Sending to "+ IP + " : " + port);
        //print("Test-Sending to this Port: nc -u " + IP + "  " + port + "");

        ThreadAlive = true;

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }
    bool ThreadAlive = false;

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (ThreadAlive)
        {
            try
            {
                string text = "";
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
                byte[] data = client.Receive(ref anyIP);

                text = Encoding.UTF8.GetString(data);
                Change_Angle_FlowRate(perform_string_operations(text));
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    public System.Collections.IEnumerator PerformChanges()
    {
        // Handles Rotations 1-100 Range
        _branchAnim.FlowAnimUpdate(FlowRate_Comb);
        _branchAnim.FogAnimUpdate(Angle);

        //if (isTrackerOnBranch && isFreezeRot)
        //{
        //    SteamVR_TrackedObject.SetIsFreezeTrackerRot(true);
        //    isFreezeRot = false;
        //}
        _shapeModule.angle = (Angle / 170f) * 15f;// Angle / 170f * -1 + 15f; //Angle;
        var emission = ps.emission;
        emission.rateOverTime = 110f + Mathf.Min(FlowRate_Comb, fFlushSetting) * 3f * (Flowrate * 0.10f); //FlowRate_Comb;
        
        // Collision
        _shapeModuleCol.angle = _shapeModule.angle;
        var emissionCol = psCol.emission;
        // We reduce the number of collision particles for efficiency
        // then double the "damage".
        emissionCol.rateOverTime = emission.rateOverTime.constant * 0.10f;

        yield return null;
        this.StartCoroutine(this.PerformChanges());
    }

    //void Update(){}

    public System.Collections.Generic.List<float> perform_string_operations(string receivedString)
    {
        System.Collections.Generic.List<float> return_List = new System.Collections.Generic.List<float>();
        string[] items = receivedString.Split(',');

        foreach (string item in items)
        {
            float i = 0;
            float.TryParse(item, out i);
            return_List.Add(i);
        }
        return return_List;
    }

    public void Change_Angle_FlowRate(System.Collections.Generic.List<float> ChangeValues)
    {
        /* Old values for referrence
        Flowrate = float.Parse((ChangeValues[0] * 10).ToString());
        FlowRate_Comb = float.Parse((((ChangeValues[1] / 100) * ChangeValues[0]) * 10).ToString());
        Angle = float.Parse(((ChangeValues[2] * -1) + 100).ToString());*/

        //TODO: make more efficient?
        // The way Flowrate operates is different to FlowRate_Comb and Angle
        Flowrate = float.Parse(ChangeValues[0].ToString());
        _branchAnim.SetBaleHandleRotX(Flowrate); //0-100

        FlowRate_Comb = float.Parse(ChangeValues[1].ToString());
        Angle = float.Parse(ChangeValues[2].ToString());

        // Turn water on/ off for Vive/ UDP
        if (!isXboxController)
        {
            if (Flowrate >= 0.1f)
            {
                _branchAnim._inputController.SetWaterOn(true);
            }
            else
            {
                _branchAnim._inputController.SetWaterOn(false);
            }

            if (Angle != prevfogStream && isTrackerOnBranch)
            {
                //TODO: SteamVR_TrackedObject.SetIsFreezeTrackerRot(true);
                //isFreezeRot = true;
                prevfogStream = Angle;
            }
        }
        // Xbox debug controller
        else
        {
            // Flowrate/ Gate/ Branch
            if (Flowrate < 0.25f)
            {
                _branchAnim._inputController.SetWaterOn(false);
            }
            else
            {
                _branchAnim._inputController.SetWaterOn(true);
            }
        
            // FlowRate_Comb/ Flow Rate/ Flow Select
            if (FlowRate_Comb != prevFlowRateComb)
            {
                if (FlowRate_Comb > prevFlowRateComb)
                    flowSelectSNormal++;
                if (FlowRate_Comb < prevFlowRateComb)
                    flowSelectSNormal--;

                _branchAnim.SetFlowSelectRotZSNormal(flowSelectSNormal); //-1-1 //FlowRate_Comb / 10?
                _branchAnim.SetFlowSelectRotZCurr(FlowRate_Comb); //0-1 //FlowRate_Comb / 10?
                flowSelectSNormal = 0;
                prevFlowRateComb = FlowRate_Comb;
            }

            // Angle/ Fog Pattern/ Fog Stream
            if (Angle != prevfogStream)
            {
                if (Angle > prevfogStream)
                    fogStreamSNormal++;
                if (Angle < prevfogStream)
                    fogStreamSNormal--;
                _branchAnim.SetFogStreamRotZSNormal(fogStreamSNormal); //Normalised value to Lerp in BranchAnim
                _branchAnim.SetFogStreamRotZCurr(Angle);
                fogStreamSNormal = 0;
                prevfogStream = Angle;
            }
            //_branchAnim.SetIsToUpdate(true); //TODO: Make Coroutine?
        }
    }

    static public bool GetBranchBrand() { return (isProtek) ? isProtek : false; }// isGForce;

    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
    //TOOD: needed?
    private void OnDisable()
    {
        OnApplicationQuit();
    }
    //TOOD: needed?
    private void OnDestroy()
    {
        OnApplicationQuit();
    }

    void OnApplicationQuit()
    {
        ThreadAlive = false;
        if (client != null)
        {
            client.Close();
        }
        receiveThread.Abort();
        //print("Abort completed");
    }
}
