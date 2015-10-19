#pragma strict
var rotSpeed : float = 2;

function FixedUpdate () {
	transform.Rotate(0, 0, rotSpeed);
}