using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;

// �� ���� ������Ʈ�� �ֱ������� ����
public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;       // ������ �� AI

    public ZombieData[] zombieDatas; // ����� ���� �¾� �����͵�
    
    public Transform[] spawnPoints; // �� AI�� ��ȯ�� ��ġ�� (���� ��ġ�� ����� Ʈ������ ���� �迭)

    private List<Enemy> enemies = new List<Enemy>(); // ������ ������ ��� ����Ʈ

    //��� ���� ������ ������ ���̺� 1�� ����, ���̺� ���� Ŭ���� �ѹ��� �����Ǵ� ���� �� ����
    private int wave; // ���� ���̺� 
    private int enemyCount = 0; //���� ���� ��

    //���̺� ���� ����ȭ
    //���� ���� �� zombieCount�� ���� ���̺� wave ���� OnPhotonSerializeVie( ) �޼��带 �����Ͽ� ����ȭ

    //�ֱ������� �ڵ� ����Ǵ�, ����ȭ �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���� ������Ʈ��� ���� �κ��� �����
        if (stream.IsWriting)
        {
            //���� ���� ���� ��Ʈ��ũ�� ���� ������
            stream.SendNext(enemies.Count);
            //���� ���̺긦 ��Ʈ��ũ�� ���� ������
            stream.SendNext(wave);
        }
        else
        {
            //����Ʈ ������Ʈ��� �б� �κ��� �����
            //���� ���� ���� ��Ʈ��ũ�� ���� �ޱ�
            enemyCount = (int)stream.ReceiveNext();
            //���� ���̺긦 ��Ʈ��ũ�� ���� �ޱ�
            wave = (int)stream.ReceiveNext();
        }
    }

    //����ȭ�� ������ȭ
    //����ȭ - � ������Ʈ�� ������ �����ͷ� ��ȯ�ϴ� �͸�
    //������ȭ - ����Ʈ �����͸� �ٽ� ���� ������Ʈ�� ��ȯ�ϴ� ó��

    //PUN�� RPC�� ���� ������ �޼��忡 �Բ� ÷���� �� �ִ� �Է� Ÿ�Կ� ������ ����
    //RPC�� ���� �ٸ� Ŭ���̾�Ʈ�� ���� ������ ��ǥ���� Ÿ�����δ�
    //byte, bool, int, float, string, Vector3, Quaternion ��
    //�̵��� ����ȭ/������ȭ�� PUN�� ���� �ڵ����� �̷����
    //PhotonPeer.RegisterType( ) �޼��带 �����ϰ�, ���ϴ� Ÿ���� ����ϰ�,
    //��� �ش� Ÿ���� ����ȭ(Serialize, �ø��������) / ������ȭ (Deserialize, ��ø��������) ���� ���
    //PhotonPeer.RegisterType(Ÿ��, ��ȣ, ����ȭ �޼���, ������ȭ �޼���)

   
    //PhotonPeer.RegisterType()����
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor,
            ColorSerialization.DeserializeColor);
    }

    //SerializeColor( )
    // - SerializeColor( ) �޼���� ���� ������Ʈ�� color Ÿ������ �����ϰ�,
    //����Ʈ �迭 ������ byte[]�� ����ȭ
    // - ���ÿ� ����ȭ�� �������� ���̸� short(int���� �� ���� ������ ����) Ÿ������ ��ȯ

    //DeserializeColor( )
    //- DeserializeColor( ) �޼���� ����ȭ�� ����Ʈ �迭 �����͸� ���� Ÿ���� Color Ÿ������ ��ȯ


    private void Update()
    {
        // ȣ��Ʈ�� ���� ���� ������ �� ����
        // �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ���� �����϶��� �������� ����
            if (GameManager.instance != null && GameManager.instance.isGameover)
            {
                return;
            }

            // ���� ��� ����ģ ��� ���� ���� ����
            if (enemies.Count <= 0)
            {
                SpawnWave();
            }
        }
        // UI ����
        UpdateUI();
    }

    // ���̺� ������ UI�� ǥ��
    private void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ȣ��Ʈ�� ���� ������ �� ����Ʈ�� ���� ���� ���� ���� ǥ����
            UIManager.instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            //Ŭ���̾�Ʈ�� �� ����Ʈ�� ������ �� �����Ƿ�, ȣ��Ʈ�� ������ enemyCount�� ���� ���� ���� ǥ����
            UIManager.instance.UpdateWaveText(wave, enemyCount);
        }
    }

    // ���� ���̺꿡 ���� ���� ����
    private void SpawnWave()
    {
        // ���̺� 1 ����
        wave++;

        // ���� ���̺� * 1.5�� �ݿø� �� ���� ��ŭ ���� ����
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount ��ŭ ���� ����
        for (int i = 0; i < spawnCount; i++)
        {
            
            // �� ���� ó�� ����
            CreateEnemy();
        }
    }

    // ���� �����ϰ� ������ ������ ������ ����� �Ҵ�
    private void CreateEnemy()
    {
        // ����� ���� ������ �������� ����
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];

        //������ ��ġ�� �������� ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
            spawnPoint.position, spawnPoint.rotation);

        //������ ���� �¾��ϱ� ���� Enemy ������Ʈ�� ������
        Enemy enemy = createdEnemy.GetComponent<Enemy>();

        //Setup( ) �޼��带 RPC�� ���� ����
        //������ ���� �ɷ�ġ ����
        //ȣ��Ʈ�Ӹ� �ƴ϶� ��� Ŭ���̾�Ʈ���� ������ ���� ���� Setup �޼��带 ���� ����
        //���� �ڵ� enemy.Setup(enemyData);

        enemy.photonView.RPC("Setup", RpcTarget.All, zombieData.health, 
                             zombieData.damage, zombieData.speed, zombieData.skinColor);


        // ������ ���� ����Ʈ�� �߰�
        enemies.Add(enemy);

        // ���� onDeath �̺�Ʈ�� �͸� �޼��� ���
        // ����� ���� ����Ʈ���� ����
        enemy.onDeath += () => enemies.Remove(enemy);
        // ����� ���� 10 �� �ڿ� �ı�
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10f));
        // �� ����� ���� ���
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }

    //DestroyAfter( ) �ڷ�ƾ �޼��� �߰�
    //������ Network.Destroy()�� ���� �ı��� �������� �����Ƿ� ���� �ı��� ���� ������
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        //delay��ŭ ����
        yield return new WaitForSeconds(delay);
        //target�� ���� �ı����� �ʾҴٸ�
        if(target != null)
        {
            //target�� ��� ��Ʈ��ũ �󿡼� �ı�
            PhotonNetwork.Destroy(target);
        }
    }
}

