using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;      // ����Ƽ�� ���� ������Ʈ
using Photon.Realtime; // ���� ���� ���� ���̺귯��
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";  // ���� ����

    public Text connectionInfoText; // ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Button joinButton;          // �� ���� ��ư
    
    //���� ����� ���ÿ� ������ ���� ���� �õ�
    void Start()
    {
        // ���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        // ������ ������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        //�� ���� ��ư�� ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        //������ �õ� ������ �ؽ�Ʈ�� ǥ��
        connectionInfoText.text = "������ ������ ���� ��....";

    }

    //������ ���� ���� �� �ڵ� ����
    public override void OnConnectedToMaster()
    {
        // �� ���� ��ư�� Ȱ��ȭ
        joinButton.interactable = true;
        // ���� ���� ǥ��
        connectionInfoText.text = "�¶��� : ������ ������ ������ �Ϸ�Ǿ����ϴ�";
    }

    // ������ ������ ���� ���� �� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        //�� ���� ��ư�� ��Ȱ��ȭ
        joinButton.interactable = false;
        //���� ���� ǥ��
        connectionInfoText.text = "�������� : ������ �������� ������ �����߽��ϴ�.\n���� ��õ� ��...";
        //������ �������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // �� ���� �õ�
    public void Connect()
    {
        // �ߺ� ���� �õ��� ���� ����, ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        // ������ ������ ���� ���̶��..
        if (PhotonNetwork.IsConnected)
        {
            //�� ���� ����
            connectionInfoText.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //������ ������ ���� ���� �ƴ϶�� , ������ ������ ���� �õ�
            connectionInfoText.text = "�������� : ������ �������� ������ �����߽��ϴ�.\n���� ��õ� ��...";
            //������ �������� ������ �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (�� ���� ����)���� �� ������ ������ ��� �ڵ� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //���� ���� ǥ��
        connectionInfoText.text = "�� ���� ����, ���ο� ���� �����մϴ�...";
        //�ִ� 4���� ���� ������ �� ���� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    // �뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        //���� ���� ǥ��
        connectionInfoText.text = "�� ���� ����";
        //��� �� �����ڵ��� Main ���� �ε��ϰ� ��
        PhotonNetwork.LoadLevel("Main");
    }
}
