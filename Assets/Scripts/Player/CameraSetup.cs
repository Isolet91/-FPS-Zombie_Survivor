using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  //시네머신 관련 코드
using Photon.Pun;   //PUN 관련 코드

// 시네머신 카메라가 로컬 플레이어만 추적하도록 설정
public class CameraSetup : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        //만약 자신이 로컬 플레이어라면
        if (photonView.IsMine)
        {
            //씬에 있는 시네머신 가상 카메라를 찾고
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            //가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
