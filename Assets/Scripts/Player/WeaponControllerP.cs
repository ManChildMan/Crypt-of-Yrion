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
        
        SkeletonController skelController = col.GetComponent<SkeletonController>();
        if (skelController != null)
        {
            skelController.CurrentHealth -= playerController.Attack / 10;
        }

        ZombieController zombController = col.GetComponent<ZombieController>();
        if (zombController != null)
        {
            zombController.CurrentHealth -= playerController.Attack / 10;
        }

        BossController bossController = col.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.health -= playerController.Attack / 10;
        }
    }
}
