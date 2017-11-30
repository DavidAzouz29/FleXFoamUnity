using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleBlur : MonoBehaviour {

    [Range(0,10)]
    public float intensity;
    private Material material;

    private RenderTexture mainRenderTexture;


    // Creates a private material used to the effect
    void Awake() {
        material = new Material(Shader.Find("Hidden/WaterShadies"));

        //mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        //mainRenderTexture.Create();
        //
        //GetComponent<Camera>().targetTexture = mainRenderTexture;
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (intensity == 0) {
            Graphics.Blit(source, destination);
            return;
        }
        
    
        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);
    
        Graphics.Blit(destination, null as RenderTexture);
    }

    void OnPostRender() {
        
    }
}
