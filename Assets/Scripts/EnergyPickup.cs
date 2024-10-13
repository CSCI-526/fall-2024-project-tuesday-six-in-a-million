using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
     public float energyAmount = 20f;  // 每次回复的能量量

    // 当玩家碰到该物体时触发
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的对象是否是玩家
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // 调用玩家的能量回复函数
            player.RechargeEnergy(energyAmount);

            // 物体被拾取后销毁
            Destroy(gameObject);
        }
    }
}
