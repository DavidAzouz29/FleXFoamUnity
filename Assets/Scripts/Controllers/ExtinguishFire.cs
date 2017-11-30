///<summary>
/// Name: ExtinguishFire.cs
/// Author: David Azouz
/// Date Created: 29/11/17
/// Date Modified: 29/11/17
/// --------------------------------------------------
/// Brief:
/// viewed: 
/// 
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 29/11/17
/// 
/// TODO:
/// Turn off burst in SimpleEmbers
/// </summary>

using UnityEngine;

public class ExtinguishFire : MonoBehaviour
{
    public Transform[] t_props;
    [SerializeField]
    private string sTag = "Prop";
    // used to only handle collision now.

    float fEmissionRate = 64f;
    [SerializeField]
    ParticleSystem[,] psSystems;
    const int psEffects = 5;

    private void Start()
    {
        psSystems = new ParticleSystem[t_props.Length, psEffects];
        for (int i = 0; i < t_props.Length; i++)
        {
            // Get the five fire particle effects.
            ParticleSystem[] psFire = t_props[i].GetComponentsInChildren<ParticleSystem>();
            // Set 1-5 to the second of the 2D array element.
            for (int j = 0; j < psFire.Length; j++)
            {
                psSystems[i, j] = psFire[j];
            }
        }
    }


    // needs to be on the particle.
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(sTag))
        {
            for (int i = 0; i < t_props.Length; i++)
            {
                // Use the Prop name (and number) to define which prop we want to Extinguish e.g. Bench (1)
                if (other.name.Contains(i.ToString()) && psSystems != null)
                {
                    for (int j = 0; j < psEffects; j++)
                    {
                        ParticleSystem psFire = psSystems[i, j];
                        var psEmission = psFire.emission;
                        psEmission.rateOverTime = psEmission.rateOverTime.constant / fEmissionRate;
                    }
                }
            }
        }
    }

}
