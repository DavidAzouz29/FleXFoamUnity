using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFilter : MonoBehaviour {

    public Material PostProcessMaterial;

    public Camera BackgroundCamera;
    public Camera MainCamera;

    public RenderTexture mainRenderTexture;

    [Range(0, 10)]
    public float intensity;

    // Use this for initialization
    void Start () {
        PostProcessMaterial = new Material(Shader.Find("Hidden/WaterShadies"));
        PostProcessMaterial.SetFloat("_bwBlend", 3.0f);
        //PostProcessMaterial = new Material(Shader.Find("Unlit/Texture"));

        mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        mainRenderTexture.Create();

        BackgroundCamera.targetTexture = mainRenderTexture;
        MainCamera.targetTexture = mainRenderTexture;
    }
	
	// Update is called once per frame
	void OnPostRender () {
        PostProcessMaterial.SetFloat("_bwBlend", intensity);

        Graphics.Blit(mainRenderTexture, RenderTexture.active, PostProcessMaterial);
        //Graphics.Blit(mainRenderTexture, RenderTexture.active);
    }

}
