///<summary>
///
/// Added Singleton - David Azouz 30/11/17
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum GameState
{
    RUNNING,
    FAILED,
    SUCCESSFUL
}

public class FailStateManager : MonoBehaviour
{
    public delegate void OnMenuDisplay();
    public OnMenuDisplay OnGameEnd;



    [SerializeField]
    public GameState m_currentState = GameState.RUNNING;
    [SerializeField]
    public MeshRenderer m_dimPanel;
    [SerializeField]
    private float m_dimSpeed = 0.3f;
    [SerializeField]
    Canvas m_statScreen;

    // FailStateManager Singleton 
    private static FailStateManager _instance;
    public static FailStateManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            // If we're null
            FailStateManager failStateManager = FindObjectOfType<FailStateManager>();
            if (failStateManager != null)
            {
                _instance = failStateManager;
            }
            return _instance;
        }
    }

    // Use this for initialization
    void Start()
    {
       m_statScreen.enabled = false;
    }

    public void SwitchGameState(GameState a_gameState)
    {
        if (m_currentState != GameState.SUCCESSFUL && m_currentState != GameState.FAILED)
            m_currentState = a_gameState;

        if (m_currentState == GameState.FAILED || m_currentState == GameState.SUCCESSFUL)
        {
            StartCoroutine(DimScreen());
            StartCoroutine(RestartScreen());
        }

    }
    IEnumerator DimScreen()
    {
        while (m_currentState == GameState.FAILED || m_currentState == GameState.SUCCESSFUL)
        {
            if (m_dimPanel != null)
                m_dimPanel.material.SetFloat("_Opac", m_dimPanel.material.GetFloat("_Opac") + (m_dimSpeed * Time.deltaTime));
            yield return null;
        }
        yield return null;
    }
    IEnumerator RestartScreen()
    {
        while (m_currentState == GameState.FAILED || m_currentState == GameState.SUCCESSFUL)
        {
            m_statScreen.enabled = true;

            if (m_dimPanel != null)
            {
                if (m_dimPanel.material.GetFloat("_Opac") >= 1f)
                {
                    if (OnGameEnd != null)
                        OnGameEnd();
                    StopCoroutine(RestartScreen());
                }
            }
            yield return null;
        }

        yield return null;
    }
}

