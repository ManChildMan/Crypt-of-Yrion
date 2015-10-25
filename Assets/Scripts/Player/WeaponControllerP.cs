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
        BossController bossController = col.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.health -= playerController.Attack;
        }
    }
}
