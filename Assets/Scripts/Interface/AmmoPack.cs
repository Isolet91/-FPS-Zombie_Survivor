using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviourPun, IItem
{
    public int ammo = 30; //충전할 총알 수

    [PunRPC]
    public void Use(GameObject target)
    {
       
        //전달받은 게임 오브젝트로부터 PlayerShooter 컴포넌트 가져오기
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        //PlayerShooter 컴포넌트가 있으며, 총 오브젝트가 존재하면
        if(playerShooter != null && playerShooter.gun != null)
        {
            //총의 남은 탄환 수를 ammo 만큼 더한다(모든 클라이언트에서 실행
            //모든 클라이언트에서 원격으로 AddAmmo() 메서드가 실행되도록 코드를 변경
            //즉, 변경된 코드는 아이템 사용 자체는 호스트에서만 이루어지지만,
            //아이템을 사용하여 탄알이 증가하는 효과는 모든 클라이언트에서 동일하게 적용
            playerShooter.gun.photonView.RPC("AddAmmo", RpcTarget.All, ammo);
        }

        //모든 클라이언트에서의 자신을 파괴
        PhotonNetwork.Destroy(gameObject);

        //target에 탄알을 추가하는 처리
        Debug.Log("탄알이 증가했다 : " + ammo);
       
    }
}
