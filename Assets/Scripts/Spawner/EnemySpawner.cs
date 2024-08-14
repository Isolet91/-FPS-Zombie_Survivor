using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public Enemy enemyPrefab;       // 생성할 적 AI

    public ZombieData[] zombieDatas; // 사용할 좀비 셋업 데이터들
    
    public Transform[] spawnPoints; // 적 AI를 소환할 위치들 (생성 위치를 사용한 트랜스폼 저장 배열)

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트

    //모든 좀비 제거할 때마다 웨이브 1씩 증가, 웨이브 값이 클수록 한번에 생성되는 좀비 수 증가
    private int wave; // 현재 웨이브 
    private int enemyCount = 0; //남은 좀비 수

    //웨이브 정보 동기화
    //남은 좀비 수 zombieCount와 현재 웨이브 wave 값은 OnPhotonSerializeVie( ) 메서드를 구현하여 동기화

    //주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            //남은 좀비 수를 네트워크를 통해 보내기
            stream.SendNext(enemies.Count);
            //현재 웨이브를 네트워크를 통해 보내기
            stream.SendNext(wave);
        }
        else
        {
            //리모트 오브젝트라면 읽기 부분이 실행됨
            //남은 좀비 수를 네트워크를 통해 받기
            enemyCount = (int)stream.ReceiveNext();
            //현재 웨이브를 네트워크를 통해 받기
            wave = (int)stream.ReceiveNext();
        }
    }

    //직렬화와 역직렬화
    //직렬화 - 어떤 오브젝트를 바이터 데이터로 변환하는 터리
    //역직렬화 - 바이트 데이터를 다시 원본 오브젝트로 변환하는 처리

    //PUN은 RPC로 원격 실행할 메서드에 함께 첨부할 수 있는 입력 타입에 제약이 없음
    //RPC를 통해 다른 클라이언트로 전송 가능한 대표적인 타입으로는
    //byte, bool, int, float, string, Vector3, Quaternion 등
    //이들은 직렬화/역직렬화가 PUN에 의해 자동으로 이루어짐
    //PhotonPeer.RegisterType( ) 메서드를 실행하고, 원하는 타입을 명시하고,
    //어떻게 해당 타입을 직렬화(Serialize, 시리얼라이즈) / 역직렬화 (Deserialize, 디시리얼라이즈) 할지 명시
    //PhotonPeer.RegisterType(타입, 번호, 직렬화 메서드, 역직렬화 메서드)

   
    //PhotonPeer.RegisterType()실행
    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor,
            ColorSerialization.DeserializeColor);
    }

    //SerializeColor( )
    // - SerializeColor( ) 메서드는 들어온 오브젝트는 color 타입으로 가정하고,
    //바이트 배열 데이터 byte[]로 직렬화
    // - 동시에 직렬화된 데이터의 길이를 short(int보다 더 적은 범위의 정수) 타입으로 반환

    //DeserializeColor( )
    //- DeserializeColor( ) 메서드는 직렬화된 바이트 배열 데이터를 본래 타입인 Color 타입으로 변환


    private void Update()
    {
        // 호스트만 적을 직접 생성할 수 있음
        // 다른 클라이언트들은 호스트가 생성한 적을 동기화를 통해 받아옴
        if (PhotonNetwork.IsMasterClient)
        {
            // 게임 오버 상태일때는 생성하지 않음
            if (GameManager.instance != null && GameManager.instance.isGameover)
            {
                return;
            }

            // 적을 모두 물리친 경우 다음 스폰 실행
            if (enemies.Count <= 0)
            {
                SpawnWave();
            }
        }
        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 호스트는 직접 갱신한 적 리스트를 통해 남은 적의 수를 표시함
            UIManager.instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            //클라이언트는 적 리스트를 갱신할 수 없으므로, 호스트가 보내준 enemyCount를 통의 적의 수를 표시함
            UIManager.instance.UpdateWaveText(wave, enemyCount);
        }
    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave()
    {
        // 웨이브 1 증가
        wave++;

        // 현재 웨이브 * 1.5에 반올림 한 개수 만큼 적을 생성
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount 만큼 적을 생성
        for (int i = 0; i < spawnCount; i++)
        {
            
            // 적 생성 처리 실행
            CreateEnemy();
        }
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy()
    {
        // 사용할 좀비 데이터 랜덤으로 결정
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];

        //생성할 위치를 랜덤으로 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
            spawnPoint.position, spawnPoint.rotation);

        //생성할 적을 셋업하기 위해 Enemy 컴포넌트를 가져옴
        Enemy enemy = createdEnemy.GetComponent<Enemy>();

        //Setup( ) 메서드를 RPC로 원격 실행
        //생성한 적의 능력치 설정
        //호스트뿐만 아니라 모든 클라이언트에서 생성된 좀비에 대해 Setup 메서드를 원격 실행
        //기존 코드 enemy.Setup(enemyData);

        enemy.photonView.RPC("Setup", RpcTarget.All, zombieData.health, 
                             zombieData.damage, zombieData.speed, zombieData.skinColor);


        // 생성된 적을 리스트에 추가
        enemies.Add(enemy);

        // 적의 onDeath 이벤트에 익명 메서드 등록
        // 사망한 적을 리스트에서 제거
        enemy.onDeath += () => enemies.Remove(enemy);
        // 사망한 적을 10 초 뒤에 파괴
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10f));
        // 적 사망시 점수 상승
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }

    //DestroyAfter( ) 코루틴 메서드 추가
    //포톤의 Network.Destroy()는 지연 파괴를 지원하지 않으므로 지연 파괴를 직접 구현함
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        //delay만큼 쉬고
        yield return new WaitForSeconds(delay);
        //target이 아직 파괴되지 않았다면
        if(target != null)
        {
            //target을 모든 네트워크 상에서 파괴
            PhotonNetwork.Destroy(target);
        }
    }
}

