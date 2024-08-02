using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//리지드바디가 없으면 안되므로 이 스크립트가 추가된 오브젝트는 리지드바디를 반드시 부착
[RequireComponent(typeof(Rigidbody))] 
public class PlayerMovement : MonoBehaviour
{
   //PlayerInput 스크립트에서 감지한 입력값을 사용하여 실제로 캐릭터를 이동시키고 회전시키는 스크립트

    public float moveSpeed = 5f;      // 플레이어 이동 속도
    public float rotateSpeed = 180f;  // 플레이어 회전 속도

    private Animator playerAnimator;  // 플레이어 애니메이터
    private PlayerInput playerInput;  // PlayerInput 스크립트 변수
    private Rigidbody rb;             // Rigidbody 컴포넌트 변수

    void Start()
    {
        //컴포넌트 가져오기
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        
    }

    void FixedUpdate()
    {
        // 게임오버 상태에서는 움직이지 않도록 (게임매니저가 구현되면 주석 해제)
        /*
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }
        */

        // 입력값을 기반으로 움직임과 회전을 처리
        Move();
        Rotate();
        // 입력값에 따라 애니메이터의 Move 파라미터 값을 변경
        playerAnimator.SetFloat("Move", playerInput.move);
    }

   private void Move()
    {
        // 입력값을 기반으로 이동할 거리 계산
        Vector3 moveDistance = transform.forward * playerInput.move * moveSpeed * Time.fixedDeltaTime;
        // Rigidbody를 사용하여 이동
        rb.MovePosition(rb.position + moveDistance);
    }

    private void Rotate()
    {
        // 입력값을 기반으로 회전 값 계산
        float turn = playerInput.rotate * rotateSpeed * Time.fixedDeltaTime;
        // Quaternion을 사용하여 회전
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // Rigidbody를 사용하여 회전
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}