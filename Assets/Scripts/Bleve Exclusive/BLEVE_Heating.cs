using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLEVE_Heating : MonoBehaviour {


    [Header("Liquid Settings")]
    [SerializeField]
    [Tooltip("1 mass = 1kg or 1litre")]
    private float m_liquidMass = 1000f;
    [SerializeField]
    [Tooltip("The Temperature in Celsius")]
    private float m_liquidTemperature = 0f;
    [SerializeField]
    [Tooltip("The boiling point of the liquid in the container")]
    private float m_boilingPoint = 95f;
    [SerializeField]
    private float m_idleTemp = 20f;

    [Header("Pressure Settings")]
    [SerializeField]
    [Tooltip("The pressure is based around the mass, 1 pressure is equivalent to 1kg of pressure.")]
    private float m_pressure = 0f;
    [SerializeField]
    [Tooltip("how often the valve will open to release gas.")]
    private float m_pressureRelease = 20f;
    [SerializeField]
    [Tooltip("Multiplier for the pressure gain amount")]
    private float m_pressureGainAmount = 1f;
    [SerializeField]
    [Tooltip("When the object will reach an explosive amount")]
    private float m_explosivePressure = 80f;
    // used to store the original gas.
    private float m_pressureReleaseOriginal = 0f;

    [Header("Valve Settings")]
    [SerializeField]
    [Tooltip("How many times the valve will open")]
    private int m_valveReleaseAmount = 3;
    [SerializeField]
    [Tooltip("How much pressure will be removed per valve opening")]
    private float m_lossPerRelease = 10f;

    [SerializeField]
    [Tooltip("Valve particles to play on valve release")]
    List<ParticleSystem> m_valveParticles = new List<ParticleSystem>();
    [SerializeField]
    [Tooltip("Particles to play when the bleve has reached the exposion pressure")]
    List<ParticleSystem> m_Explosion = new List<ParticleSystem>();

    [Header("Utility Settings")]
    [Tooltip("How fast the tank coolsdown on extinguisher hit")]
    [SerializeField]
    float m_coolDownMultiplier = 2f;
    [SerializeField]
    [Tooltip("Specific heat for the liquid. (joules/gram Celcius)")]
    private float m_specificHeatJGC = 2.22f;
    // joule to celcius 'rough' amount.
    private float m_jouleToC = 1600f;

    [Header("Debug Settings")]
    [SerializeField]
    [Tooltip("Instantly explode the tanker")]
    private bool m_forceExplode = false;
    [SerializeField]
    [Tooltip("Allows visualization of the temp through a color")]

	public AudioSource valveReleaseAudio;
	public AudioClip releaseAudio;
	public AudioClip explosion;
    public bool m_debugMode = true;
	[SerializeField]
	float m_darkSpeed = 0.001f;
	[SerializeField]
	float m_minDarkness = 0.2f;


    // a check bool to make sure that the object isn't dead before playing the next loop of the particle.
    private bool m_hasExploded = false;

    // Use this for initialization
    void Start()
    {
        m_liquidTemperature = m_idleTemp;
        m_pressureReleaseOriginal = m_pressureRelease;
    }

    // Update is called once per frame
    void Update()
    {
		
        // removable - only for debug purposes
        if (m_debugMode)
            DebugDraw();
        // removable - only for debug purposes
        if (m_forceExplode)
        {
            m_pressure += m_explosivePressure;
            m_liquidTemperature += m_boilingPoint;
        }
        // check if the temp is below its idle temp and reset it.
        if (m_liquidTemperature <= m_idleTemp)
            m_liquidTemperature = m_idleTemp;



        // Check whether the liquid temp is greater than the boiling point.
        // once this happens we are starting to produce gases. // NOTE: usual cases there would already be gases being produced but for simplicity we are just creating gases when the boiling point is reached
        // when the pressure is higher than the valve release point the valve will open and release some of the gasses.
        // this will happen depending on the int set in the inspector "m_valveReleaseAmount"
        // everytime the gas is released a var "m_lossPerRelease" will be deducted from the total pressure that was accumulated
        // once the gas has been released we will play the particle system.
        // if the pressure is greater than the explosive pressure we will set the particles to play
        // after the object has exploded ??? possibly replace mesh with a destroyed one.
        if (m_liquidTemperature > m_boilingPoint)
        {
            float gasRelease = (m_pressureGainAmount * Time.deltaTime) * (m_liquidTemperature / m_boilingPoint);
            m_pressure += gasRelease;
            m_liquidMass -= gasRelease;
            // start producing gasses
            if (m_pressure > m_pressureRelease && m_valveReleaseAmount > 0)
            {
                foreach(var PS in m_valveParticles)
                {
                    PS.Play();
					valveReleaseAudio.PlayOneShot (releaseAudio,0.7f);
                }
                m_valveReleaseAmount--;
                m_pressureRelease += m_pressureReleaseOriginal;
                m_pressure -= m_lossPerRelease;
            }
            if (m_pressure >= m_explosivePressure && !m_hasExploded)
            {
				valveReleaseAudio.PlayOneShot (explosion,0.7f);
                Invoke("PlayExplosion", 0.1f);
                m_hasExploded = true;
            }
        }
    }
    /// <summary>
    /// Formula used to calculate heat needed to reach this temp (Q = mcΔT)
    /// </summary>
    /// <param name="a_mass"></param>
    /// <param name="a_heat"></param>
    /// <param name="a_specificHeat"></param>
    /// <returns></returns>
    float EnergyNeededToReachTemp(float a_mass, float a_specificHeat, float a_desiredHeat, float a_currentHeat)
    {
        // return the amount of energy needed to reach this temperature
        return ((a_mass) * (a_specificHeat)) * (a_desiredHeat - a_currentHeat);
    }
    /// <summary>
    /// Heat will be added depending on the mass of this object. (Q = cm∆T)
    /// </summary>
    /// <param name="a_heat"></param>
    public void AddHeat(float a_heat)
    {
        // calculate temp to joules
        float heatinJoules = a_heat * m_jouleToC;

        // add a rough approximate of how much heat will be applied to the bleve object per frame.
        float heatAdded = (heatinJoules / (m_specificHeatJGC * m_liquidMass) * Time.deltaTime);//m_specificHeatJGC * m_liquidMass;
        m_liquidTemperature += (heatAdded / m_jouleToC);
    }
    /// <summary>
    /// Used only for debugging purposes, this will set the overlayed texture color to red depending on its temp.
    /// </summary>
    private void DebugDraw()
    {
        Material mat = GetComponent<Renderer>().material;
		if (mat.color.r > m_minDarkness)
			mat.color = new Vector4 (mat.color.r - m_darkSpeed,mat.color.g -  m_darkSpeed, mat.color.b - m_darkSpeed,0);

    }
    /// <summary>
    /// Will play the list of particles attached to this object
    /// </summary>
    private void PlayExplosion()
    {
        foreach(var PS in m_Explosion)
        {
            PS.Play();
        }
        FindObjectOfType<FailStateManager>().SwitchGameState(GameState.FAILED);
    }
    /// <summary>
    /// Temporary solution - Reduces Heat of the object
    /// </summary>
    /// <param name="a_waterTemp"></param>
    public void ReduceHeat(float a_waterTemp)
    {
        // lets just use this for a placeholder for now until a more permanant solution is discussed.
        // calculate temp to joules
        float heatinJoules = -(a_waterTemp * m_jouleToC);

        // add a rough approximate of how much heat will be applied to the bleve object per frame.
        float heatReduced = ((heatinJoules / (m_specificHeatJGC * m_liquidMass) * Time.deltaTime)  *m_coolDownMultiplier);//m_specificHeatJGC * m_liquidMass;
        m_liquidTemperature += ((heatReduced / m_jouleToC));
    }
}
