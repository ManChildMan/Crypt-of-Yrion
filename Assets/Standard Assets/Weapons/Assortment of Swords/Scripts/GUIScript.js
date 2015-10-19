#pragma strict
var weapons : GameObject[];

function NextWeapon() {
	if(weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
}

function PreviousWeapon() {
	if(weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[0].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[1].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[2].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
	else if(weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled == true) {
		weapons[4].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = false;
		weapons[3].GetComponent.<MeshRenderer>().GetComponent.<Renderer>().enabled = true;
	}
}