using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    public int ammo = 30; //충전할 총알 수

    public void Use(GameObject target)
    {
       
        //전달받은 게임 오브젝트로부터 PlayerShooter 컴포넌트 가져오기
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        //PlayerShooter 컴포넌트가 있으며, 총 오브젝트가 존재하면
        if(playerShooter != null && playerShooter.gun != null)
        {
            //총의 남은 탄환 수를 ammo 만큼 더한다
            playerShooter.gun.ammoRemain += ammo;
        }

        //사용되었으므로, 자신을 파괴
        Destroy(gameObject);

        //target에 탄알을 추가하는 처리
        Debug.Log("탄알이 증가했다 : " + ammo);
       
    }
}
