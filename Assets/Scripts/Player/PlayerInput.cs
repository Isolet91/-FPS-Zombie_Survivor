using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    //�÷��̾� ĳ���͸� �����ϱ� ���� ����� �Է��� ����
    //������ �Է°��� �ٸ� ������Ʈ���� ����Ҽ� �ֵ��� ����

    public string moveAxisname = "Vertical";     // �յ� �������� ���� �Է��� �̸�
    public string rotateAxisname = "Horizontal"; // �¿�ȸ���� ���� �Է��� �̸�
    public string fireButtonname = "Fire1";        // �߻縦 ���� �Է� ��ư �̸�
    public string reloadButtonname = "Reload";     // �������� ���� �Է� ��ư �̸�
   
    //�� �Ҵ��� ���ο����� ����
    public float move { get; private set; }   //������ ������ �Է°�
    public float rotate { get; private set; } //������ ȸ�� �Է°�
    public bool fire { get; private set; }    //������ �߻� �Է°�
    public bool reload { get; private set; }  //������ ������ �Է°�

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���� �÷��̾ �ƴ� ��� �Է��� ���� ����
        if (!photonView.IsMine)
        {
            return;
        }

       
        //���ӿ��� ���¿����� ����� �Է��� �������� �ʴ´�
      if(GameManager.instance != null&& GameManager.instance.isGameover)
        {
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;
            return;
        }
      


        //move�� ���� �Է� ����
        move = Input.GetAxis(moveAxisname);
        //rotate
        rotate = Input.GetAxis(rotateAxisname);
        //fire
        fire = Input.GetButton(fireButtonname);
        //reload
        reload = Input.GetButtonDown(reloadButtonname);
    }
}
