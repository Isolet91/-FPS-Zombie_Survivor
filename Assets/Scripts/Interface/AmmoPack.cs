using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviourPun, IItem
{
    public int ammo = 30; //������ �Ѿ� ��

    [PunRPC]
    public void Use(GameObject target)
    {
       
        //���޹��� ���� ������Ʈ�κ��� PlayerShooter ������Ʈ ��������
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        //PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
        if(playerShooter != null && playerShooter.gun != null)
        {
            //���� ���� źȯ ���� ammo ��ŭ ���Ѵ�(��� Ŭ���̾�Ʈ���� ����
            //��� Ŭ���̾�Ʈ���� �������� AddAmmo() �޼��尡 ����ǵ��� �ڵ带 ����
            //��, ����� �ڵ�� ������ ��� ��ü�� ȣ��Ʈ������ �̷��������,
            //�������� ����Ͽ� ź���� �����ϴ� ȿ���� ��� Ŭ���̾�Ʈ���� �����ϰ� ����
            playerShooter.gun.photonView.RPC("AddAmmo", RpcTarget.All, ammo);
        }

        //��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
        PhotonNetwork.Destroy(gameObject);

        //target�� ź���� �߰��ϴ� ó��
        Debug.Log("ź���� �����ߴ� : " + ammo);
       
    }
}
