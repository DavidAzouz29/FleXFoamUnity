#pragma strict

private var initialValue : float;//Initial light intensity value
private var initialPosition : Vector3;//Initial transform position
private var initialScale : Vector3;
private var initialTime : float;//Initial time offset allows each instance of the fires to appear to flicker differently 
private var lightRef : Light;

var amount : float = 0.01;//Amount to ajust intensity of light
var speed : float = 8;//speed at which to ajust intensity

var adjustLocation : boolean;//do we want to randomly offset this position? 
var locationAdjustAmount : float = 1;

var adjustScale : boolean = false; 
var scaleAdjustAmount : float = 1;
var scaleObject : Transform;//Incase the scale needs to be applied to a different transform


function Start () {
	initialTime = Random.value*100;//Get random offset

	if(GetComponent(Light)){//find light
		lightRef = GetComponent(Light);
		initialValue = lightRef.intensity;
	}

	if(scaleObject == false){
		scaleObject = transform;
	}

	initialPosition = transform.position;
	initialScale = scaleObject.localScale;
}

function Update () {
	var intensityNoise = Mathf.PerlinNoise(Time.time*speed, initialTime);
	if(lightRef){//use perlin noise to ajust intensity
		lightRef.intensity = initialValue + intensityNoise*amount;
	}

	if(adjustLocation){//use perlin noise to ajust position
		var offset : Vector3 = Vector3(
								Mathf.PerlinNoise(Time.time*speed, initialTime + 5) - 0.5,
								intensityNoise - 0.5,//reuse intensity noise for y offset
								Mathf.PerlinNoise(Time.time*speed, initialTime + 10) - 0.5);

		transform.position = initialPosition + offset * locationAdjustAmount * 2;
	}

	if(adjustScale){//use perlin noise to ajust scale
		scaleObject.localScale = initialScale * ((intensityNoise-0.5)*scaleAdjustAmount + 1);
	}
}