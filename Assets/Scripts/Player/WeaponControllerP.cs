using UnityEngine;
using System.Collections;

public class WeaponControllerP : MonoBehaviour {

    private int Damage;

    public void OnTriggerEnter(Collider col)
    {
        Damage = gameObject.transform.root.GetComponentInParent<PlayerController>().GiveDamage();

        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyController>().TakeDamage(Damage);
        }
        else
        {
            return;
        }

    }
}
