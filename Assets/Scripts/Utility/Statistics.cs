using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Statistics : MonoBehaviour 
{
	public float timeTaken;
	public float waterUsage;
	public float waterTime;
	private int timeMinutes;
	public GameObject panelFail;

	public Text timeTakenText;
	public Text waterUsageText;
    public Text stateText;

	private bool gameOver = false;

    [SerializeField]
    private FailStateManager fsm;

    private void Awake()
    {
        fsm = FailStateManager.Instance;
    }

    void Update () 
	{
        
		waterUsage = waterTime * 0.45f;

		timeTaken += 1 * Time.deltaTime;

		//Converting Seconds to Minutes
		if (timeTaken >= 60) 
		{			
			timeMinutes += 1;
			timeTaken = 0;
		}

		if (gameOver == false) {
			//Setting time taken text values
			timeTakenText.text = "<Size=55>Time Taken:</size> <b>" + timeMinutes.ToString () + "m " + timeTaken.ToString ("f0") + "s</b>";
			//Setting water usage values
			waterUsageText.text = "<Size=55>Water Usage:</size> <b>" + waterUsage.ToString ("f1") + "(L) </b>";


		}
		string str = "";
        if (fsm != null)
        {
            switch(fsm.m_currentState)
            {
                case GameState.FAILED:
                    {
                        str = "Not Yet Competent";
						panelFail.SetActive (true);
						gameOver = true;
                        break;
                    }
                case GameState.RUNNING:
                    {
						gameOver = false;
                        str = "RUNNING";
                        break;
                    }
                case GameState.SUCCESSFUL:
                    {
						gameOver = true;
                        str = "Competent";
                        break;
                    }


            }

        }
		//Game State:
        stateText.text = "<Size=55></size> <b> " + str + "</b>";

    }
}
