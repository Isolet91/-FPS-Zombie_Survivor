using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

// �÷��̾� ĳ������ ����ü�μ��� ������ ���
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;            // ü���� ǥ���� UI �����̴�

    public AudioClip deathClip;            // ��� �Ҹ�
    public AudioClip hitClip;              // �ǰ� �Ҹ�
    public AudioClip itemPickupClip;       // ������ ���� �Ҹ�

    private AudioSource playerAudioPlayer; // �÷��̾� �Ҹ� �����
    private Animator playerAnimator;       // �÷��̾��� �ִϸ�����

    private PlayerMovement playerMovement; // �÷��̾� ������ ������Ʈ
    private PlayerShooter playerShooter;   // �÷��̾� ���� ������Ʈ

    private void Awake()
    {
        // ����� ������Ʈ ��������
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        // LivingEntity�� OnEnable() ���� (���� �ʱ�ȭ)
        base.OnEnable();

        // ü�� �����̴� Ȱ��ȭ
        healthSlider.gameObject.SetActive(true);
        // ü�� �����̴��� �ִ밪�� �⺻ ü�°����� ����
        healthSlider.maxValue = startingHealth;
        // ü�� �����̴��� ���� ���� ü�°����� ����
        healthSlider.value = health;

        // �÷��̾� ������ �޴� ������Ʈ�� Ȱ��ȭ
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // ü�� ȸ��
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity�� RestoreHealth() ���� (ü�� ����)
        base.RestoreHealth(newHealth);
        // ü�� ����
        healthSlider.value = health;
    }


    // ������ ó��
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if (!dead)
        {
            // ������� ���� ��쿡�� ȿ������ ���
            playerAudioPlayer.PlayOneShot(hitClip);
        }

        // LivingEntity�� OnDamage() ����(������ ����)
        base.OnDamage(damage, hitPoint, hitDirection);
        // ���ŵ� ü���� ü�� �����̴��� �ݿ�
        healthSlider.value = health;
    }

    // ��� ó��
    public override void Die()
    {
        // LivingEntity�� Die() ����(��� ����)
        base.Die();

        // ü�� �����̴� ��Ȱ��ȭ
        healthSlider.gameObject.SetActive(false);

        // ����� ���
        playerAudioPlayer.PlayOneShot(deathClip);
        // �ִϸ������� Die Ʈ���Ÿ� �ߵ����� ��� �ִϸ��̼� ���
        playerAnimator.SetTrigger("Die");

        // �÷��̾� ������ �޴� ������Ʈ�� ��Ȱ��ȭ
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����۰� �浹�� ��� �ش� �������� ����ϴ� ó��
        // ������� ���� ��쿡�� ������ ��밡��
        if (!dead)
        {
            // �浹�� �������� ���� Item ������Ʈ�� �������� �õ�
            IItem item = other.GetComponent<IItem>();

            // �浹�� �������κ��� Item ������Ʈ�� �������µ� �����ߴٸ�
            if (item != null)
            {
                // Use �޼��带 �����Ͽ� ������ ���
                item.Use(gameObject);
                // ������ ���� �Ҹ� ���
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }
}