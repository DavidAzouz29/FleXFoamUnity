using UnityEngine;
using System.Collections;

public class SoftFlicker : MonoBehaviour
{
    public float minIntensity = 0.25f;
    public float maxIntensity = 0.5f;
    public float NoiseTime = 0.5f;
    private Light lt;
    float random;


    public float RageminIntensity = 0.25f;
    public float RagemaxIntensity = 0.5f;
    


    void Start()
    {
        lt = this.gameObject.GetComponent<Light>();
        random = Random.Range(0.0f, 65535.0f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(random, Time.time*NoiseTime);
        lt.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        lt.range = Mathf.Lerp(RageminIntensity, RagemaxIntensity, noise);
    }
}
