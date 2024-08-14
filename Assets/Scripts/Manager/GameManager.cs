using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
// ������ ���� ���� ����, ���� UI�� �����ϴ� ���� �Ŵ���
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static GameManager m_instance; // �̱����� �Ҵ�� static ����

    private int score = 0; // ���� ���� ����
    public bool isGameover { get; private set; } // ���� ���� ����

    public GameObject playerPrefab; //������ �÷��̾� ĳ���� ������

    //IPunObservable ���, OnPhotonSerializeView ����
    //IPunObservable �������̽��� ����ϰ�  OnPhotonSerializeView() �޼��带 �����Ͽ�
    //���ÿ��� ����Ʈ���� ���� ����ȭ�� �����ϸ�
    //ȣ��Ʈ���� ���ŵ� ������ �ٸ� Ŭ���̾�Ʈ���� �ڵ� �ݿ�


    //�ֱ������� �ڵ� ����Ǵ�, ����ȭ �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���� ������Ʈ���, ���� �κ��� �����
        if (stream.IsWriting)
        {
            //��Ʈ��ũ�� ���� score ���� ������
            stream.SendNext(score);
        }
        else
        {
            //����Ʈ ������Ʈ��� �б� �κ��� �����

            //��Ʈ��ũ�� ���� score �� �ޱ�
            score = (int)stream.ReceiveNext();
            //����ȭ�Ͽ� ���� ������ UI�� ǥ��
            UIManager.instance.UpdateScoreText(score);
        }



       
    }
    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    //Start()���� ���� �÷��̾� ĳ���� ����
    //PhotonNewwork.Instantiate() �޼��带 ������ �ڽ��� ���� �÷��̾� ĳ���͸� ��Ʈ��ũ�󿡼� ����
    //��, �ڽ��� ���忡���� ����, Ÿ���� ���忡���� ����Ʈ�� �÷��̾� ĳ���Ͱ� ����
    //GameManager ��ũ��Ʈ�� Start() �޼���� �� ���� PhotonNetwork.Instantiate()�� ������ Ŭ���̾�Ʈ���� ���� ����
    
    //���� ���۰� ���ÿ� �÷��̾ �� ���� ������Ʈ�� ����
    private void Start()
    {
        // ������ ���� ��ġ ����
        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        // ��ġ y���� 0���� ����
        randomSpawnPos.y = 0f;

        // ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ���� ����
        // ��, �ش� ���� ������Ʈ�� �ֵ�����, ���� �޼��带 ���� ������ Ŭ���̾�Ʈ���� ����
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }


    // ������ �߰��ϰ� UI ����
    public void AddScore(int newScore)
    {
        // ���� ������ �ƴ� ���¿����� ���� ���� ����
        if (!isGameover)
        {
            // ���� �߰�
            score += newScore;
            // ���� UI �ؽ�Ʈ ����
            UIManager.instance.UpdateScoreText(score);
        }
    }

    // ���� ���� ó��
    public void EndGame()
    {
        // ���� ���� ���¸� ������ ����
        isGameover = true;
        // ���� ���� UI�� Ȱ��ȭ
        UIManager.instance.SetActiveGameoverUI(true);
    }

    //Ű���� �Է��� �����ϰ� ���� ������ ��
    //GameManager ��ũ��Ʈ�� ���� �߰��� Update() �޼��忡����
    //Ű������ ESC Ű (KeyCode.Escape)�� ������ �� ��Ʈ��ũ �� �����⸦ ����
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    //OnLeftRoom() �޼���� ���� �÷��̾ ���� ���� ���� ���� �� �ڵ� ����
    //SceneManager.LoadScene("Lobby"); �� ���� ���� Ŭ���̾�Ʈ�� ���� Lobby������ ����ǰ�,
    //�ٸ� Ŭ���̾�Ʈ�� ������ �뿡 ���ӵ� ����

    //���� ���� �� �ڵ� ����Ǵ� �޼���
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}