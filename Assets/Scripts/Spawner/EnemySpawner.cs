using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �� ���� ������Ʈ�� �ֱ������� ����
public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab;       // ������ �� AI

    public ZombieData[] zombieDatas; // ����� ���� �¾� �����͵�
    
    public Transform[] spawnPoints; // �� AI�� ��ȯ�� ��ġ�� (���� ��ġ�� ����� Ʈ������ ���� �迭)

    private List<Enemy> enemies = new List<Enemy>(); // ������ ������ ��� ����Ʈ

    //��� ���� ������ ������ ���̺� 1�� ����, ���̺� ���� Ŭ���� �ѹ��� �����Ǵ� ���� �� ����
    private int wave; // ���� ���̺� 

    private void Update()
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

        // UI ����
        UpdateUI();
    }

    // ���̺� ������ UI�� ǥ��
    private void UpdateUI()
    {
        // ���� ���̺�� ���� ���� �� ǥ��
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
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

        // �� ���������κ��� �� ����
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ������ ���� �ɷ�ġ ����
        enemy.Setup(zombieData);

        // ������ ���� ����Ʈ�� �߰�
        enemies.Add(enemy);

        // ���� onDeath �̺�Ʈ�� �͸� �޼��� ���
        // ����� ���� ����Ʈ���� ����
        enemy.onDeath += () => enemies.Remove(enemy);
        // ����� ���� 10 �� �ڿ� �ı�
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);
        // �� ����� ���� ���
        enemy.onDeath += () => GameManager.instance.AddScore(100);
    }
}