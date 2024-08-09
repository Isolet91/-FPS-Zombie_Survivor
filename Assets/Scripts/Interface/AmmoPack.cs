using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    public int ammo = 30; //������ �Ѿ� ��

    public void Use(GameObject target)
    {
       
        //���޹��� ���� ������Ʈ�κ��� PlayerShooter ������Ʈ ��������
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        //PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
        if(playerShooter != null && playerShooter.gun != null)
        {
            //���� ���� źȯ ���� ammo ��ŭ ���Ѵ�
            playerShooter.gun.ammoRemain += ammo;
        }

        //���Ǿ����Ƿ�, �ڽ��� �ı�
        Destroy(gameObject);

        //target�� ź���� �߰��ϴ� ó��
        Debug.Log("ź���� �����ߴ� : " + ammo);
       
    }
}
