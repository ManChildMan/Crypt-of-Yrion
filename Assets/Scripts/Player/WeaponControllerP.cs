using UnityEngine;
using System.Collections;

public class WeaponControllerP : MonoBehaviour {

    PlayerController playerController;

    void Start()
    {
        playerController = transform.root.GetComponentInParent<PlayerController>();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<SkeletonController>().CurrentHealth -= playerController.Attack / 10;
            return;
        }
        BossController bossController = col.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.health -= playerController.Attack / 10;
        }
    }
}
