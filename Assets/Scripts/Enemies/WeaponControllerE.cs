using UnityEngine;
using System.Collections;

public class WeaponControllerE : MonoBehaviour {

    public int Damage;

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().CurrentHealth -= Damage;
        }
    }
}
