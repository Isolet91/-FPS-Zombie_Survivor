using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//������ٵ� ������ �ȵǹǷ� �� ��ũ��Ʈ�� �߰��� ������Ʈ�� ������ٵ� �ݵ�� ����
[RequireComponent(typeof(Rigidbody))] 
public class PlayerMovement : MonoBehaviourPun
{
   //PlayerInput ��ũ��Ʈ���� ������ �Է°��� ����Ͽ� ������ ĳ���͸� �̵���Ű�� ȸ����Ű�� ��ũ��Ʈ

    public float moveSpeed = 5f;      // �÷��̾� �̵� �ӵ�
    public float rotateSpeed = 180f;  // �÷��̾� ȸ�� �ӵ�

    private Animator playerAnimator;  // �÷��̾� �ִϸ�����
    private PlayerInput playerInput;  // PlayerInput ��ũ��Ʈ ����
    private Rigidbody rb;             // Rigidbody ������Ʈ ����

    void Start()
    {
        //������Ʈ ��������
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        
    }

    void FixedUpdate()
    {
        //���� �÷��̾ ���� ��ġ�� ȸ���� ���� ����
        if (!photonView.IsMine)
        {
            return;
        }
        
     
        // �Է°��� ������� �����Ӱ� ȸ���� ó��
        Move();
        Rotate();
        // �Է°��� ���� �ִϸ������� Move �Ķ���� ���� ����
        playerAnimator.SetFloat("Move", playerInput.move);
    }

   private void Move()
    {
        // �Է°��� ������� �̵��� �Ÿ� ���
        Vector3 moveDistance = transform.forward * playerInput.move * moveSpeed * Time.fixedDeltaTime;
        // Rigidbody�� ����Ͽ� �̵�
        rb.MovePosition(rb.position + moveDistance);
    }

    private void Rotate()
    {
        // �Է°��� ������� ȸ�� �� ���
        float turn = playerInput.rotate * rotateSpeed * Time.fixedDeltaTime;
        // Quaternion�� ����Ͽ� ȸ��
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // Rigidbody�� ����Ͽ� ȸ��
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}