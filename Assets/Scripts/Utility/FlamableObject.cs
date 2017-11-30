using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamableObject : MonoBehaviour {

    // this will cause the fire to either be on, dwindling (will be added) or off.
    public float m_moisture;
    // this will represent if the fire is currently active.
    public bool m_isFire;
    // max moisture until the fire can no longer sustain.
    public float m_maxMoisture;
    public float m_deductionAmount = 1f;
    float m_originalRate;

    [SerializeField]
    float m_gasRemaining = 300f;
    [SerializeField]
    float m_gasBurnPerSecond = 1f;

    private bool m_extinguished = false;
    [SerializeField]
    private float m_endTime = 4f;

    // a ref to the emmiter on this object
    [SerializeField]
    ParticleSystem m_emmiter;
    [SerializeField]
    ParticleSystem m_gasEmmiter;

    public bool m_hasGas;

    void DecreaseFireRate()
    {
        ParticleSystem.EmissionModule emisionModule = m_emmiter.emission;

        if (emisionModule.rateOverTime.constant > 0)
            emisionModule.rateOverTime = (emisionModule.rateOverTime.constantMax - ( m_originalRate ) * Time.deltaTime);

        
    }
    // Use this for initialization
    void Start() {

        if (m_emmiter != null)
        {
            StartCoroutine(IsFireLoop());
            m_originalRate = m_emmiter.emission.rateOverTime.constant;
        }
        else
            Debug.LogError("Emitter is Missing", this.gameObject);


    }

    // Update is called once per frame
    void Update()
    {
        m_hasGas = m_gasRemaining > 0;

        if (m_moisture > 0f && m_isFire)
        {

            m_moisture -= m_deductionAmount * Time.deltaTime;
        }
        else if (m_moisture < 0f)
            m_moisture = 0f;


        if (m_gasRemaining > 0)
        {
            m_gasRemaining -= m_gasBurnPerSecond * Time.deltaTime;
            if (!m_isFire && !m_extinguished)
            {
                // wait on reignite
                m_extinguished = true;
                if (m_gasEmmiter != null)
                    m_gasEmmiter.Play();
                Invoke("SendFailState", m_endTime);
            }

            if (!m_isFire && m_extinguished)
                DecreaseFireRate();
        }
        if (!m_hasGas && !m_extinguished)
        {
            m_extinguished = true;
            Invoke("SendSuccessState", m_endTime);
        }
    }
    void SendSuccessState()
    {
        FailStateManager.Instance.SwitchGameState(GameState.SUCCESSFUL);
    }

    void SendFailState()
    {
        FailStateManager.Instance.SwitchGameState(GameState.FAILED);
    }
    /// <summary>
    /// This will handle the emitting display that this object outputs.
    /// </summary>
    /// <returns></returns>
    IEnumerator IsFireLoop()
    {
        while (true)
        {
            if (m_moisture > m_maxMoisture || m_gasRemaining <= 0f)
                m_isFire = false;
            if (m_isFire && !m_emmiter.isPlaying)
                m_emmiter.Play();
            else if (!m_isFire && !m_hasGas)
                m_emmiter.Stop();


            yield return null;
        }
    }
    // TODO: Make this work.
    public IEnumerator IsBeingExtinguished()
    {
     //   ParticleSystem.EmissionModule emisionModule = m_emmiter.emission;
     //   //
     //   if (!m_isFire && m_extinguished)
     //       yield return null;
     //
     //   emisionModule.rateOverTime = m_originalRate / 5;
        yield return new WaitForSeconds(5f);

      //  if (!m_isFire && m_extinguished)
      //      yield return null;
      //  emisionModule.rateOverTime = m_originalRate;
    }
}
