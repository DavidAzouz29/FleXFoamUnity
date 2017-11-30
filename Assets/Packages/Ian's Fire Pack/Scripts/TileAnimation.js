#pragma strict

var xFrames : int;//horizontal frames
var yFrames : int;//virtical frames
var speed : float;//How many frames per second?

var billboard = true;//do we want to billboard this object?

var mainCamera : Camera;

private var frame : int = 0;//current frame of animation
private var rendererReference : Renderer;
private var randomStart : int;//to offset each instance of this animation

function Awake () {
	rendererReference = GetComponent(Renderer);
	rendererReference.materials[0].mainTextureScale = Vector2(1.0/xFrames, 1.0/yFrames);
	if(billboard){	
		if(!mainCamera){
			mainCamera = Camera.main;
		}
	}
	
	randomStart = Random.value*xFrames*yFrames;

}

function Update () {
	frame = Mathf.Repeat(Mathf.FloorToInt(Time.time*speed) + randomStart, xFrames*yFrames);

	var xOffset = frame % xFrames; 
	var yOffset = frame / xFrames;

	rendererReference.materials[0].mainTextureOffset = Vector2(xOffset/(xFrames*1.0), 1 - (yOffset+1)/(yFrames*1.0));


	if(billboard){
		transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
