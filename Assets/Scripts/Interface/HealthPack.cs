using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ü���� ȸ���ϴ� ������
public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50; // ü���� ȸ���� ��ġ

    public void Use(GameObject target)
    {
        // ���޹��� ���� ������Ʈ�κ��� LivingEntity ������Ʈ �������� �õ�
        LivingEntity life = target.GetComponent<LivingEntity>();

        // LivingEntity������Ʈ�� �ִٸ�
        if (life != null)
        {
            // ü�� ȸ�� ����
            life.RestoreHealth(health);
        }

        // ���Ǿ����Ƿ�, ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
       PhotonNetwork.Destroy(gameObject);
    }
}