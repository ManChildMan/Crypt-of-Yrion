using UnityEngine;
using System.Collections;

public class WeaponControllerE : MonoBehaviour {

    public int Damage;

    public void OnTriggerEnter(Collider col)
    {
       // Damage = gameObject.transform.root.GetComponent<EnemyController>().GiveDamage();

        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().CurrentHealth -= Damage;
        }
        else
        {
            return;
        }
    }

}
