using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//�� ����
public class Gun : MonoBehaviourPun, IPunObservable
{
    //���� ���¸� ǥ���ϴ� �� ����� Ÿ�� ����
    public enum State
    {
        Ready,    // �߻� �غ��
        Empty,    // źâ�� ��
        Reloading // ������ ��
    }
    public State state { get; private set; }      // ���� ���� ����

    public GunData gunData;                       // ���� ���� ������

    public Transform fireTransform;               // �Ѿ��� �߻�� ��ġ
    public ParticleSystem muzzleFlashEffect;      // �ѱ� ȭ�� ȿ��
    public ParticleSystem shellEjectEffect;       // ź�� ���� ȿ��

    private LineRenderer bulletLineRenderer;      // �Ѿ� ������ �׸��� ���� ������

    private AudioSource gunAudioPlayer;           // �� �Ҹ� �����
    //public AudioClip shotClip;                  // �߻� �Ҹ�
    //public AudioClip reloadClip;                // ������ �Ҹ�

    //private float damage = 25;                  // ���ݷ�
    private float fireDistance = 50f;             // �����Ÿ�

    public int ammoRemain = 100;                  // ���� ��ü ź��
    //public int magCapacity = 25;                // źâ �뷮
    public int magAmmo;                           // ���� źâ�� �����ִ� ź��

    //public float timeBetFire = 0.12f;           // �Ѿ� �߻� ����
    //public float reloadTime = 1.8f;             // ������ �ҿ� �ð�
    private float lastFireTime;                   // ���� ���������� �߻��� ���� 

    //GunData Class���� ������ ������ �ּ� ó��

    //�ֱ������� �ڵ� ����Ǵ� ����ȭ �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���� ������Ʈ���, ���� �κ��� �����
        if (stream.IsWriting)
        {
            //���� ź����� ��Ʈ��ũ�� ���� ������
            stream.SendNext(ammoRemain);
            //źâ�� ź�� ���� ��Ʈ��ũ�� ���� ������
            stream.SendNext(magAmmo);
            //���� ���� ���¸� ��Ʈ��ũ�� ���� ������
            stream.SendNext(state);
        }
        else
        {
            //����Ʈ ������Ʈ��� �б� �κ��� �����
            //���� ź�� ���� ��Ʈ��ũ�� ���� �ޱ�
            ammoRemain = (int)stream.ReceiveNext();
            //źâ�� ź�� ���� ��Ʈ��ũ�� ���� �ޱ�
            magAmmo = (int)stream.ReceiveNext();
            //���� ���� ���¸� ��Ʈ��ũ�� ���� �ޱ�
            state = (State)stream.ReceiveNext();
        }
    }
    //���� ź���� �߰��ϴ� �޼���
    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;
    }
    private void Awake()
    {
        // ����� ������Ʈ���� ������ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        // ����� ���� 2���� ����(���� ���۵Ǵ� ���� ������ ���� �ʿ�. �������� ���� �ΰ��� ���� ����)
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        // ��ü ���� ź�� ���� �ʱ�ȭ
        ammoRemain = gunData.StartAmmoRemain;
        // ���� źâ�� ���� ä���  
        magAmmo = gunData.magCapacity;
        // ���� ���� ���¸� �غ� ���·� ����
        state = State.Ready;
        // ���������� ���� �� ������ �ʱ�ȭ
        lastFireTime = 0;
        
    }

    // �߻� �õ�
    public void Fire()
    {
        // ���� ���°� �߻� ������ �����̰�
        // && ������ �� �߻� �������� timeBetFire �̻��� �ð��� ����
        if (state == State.Ready
            && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            // ������ �� �߻� ������ ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            Shot();
        }
    }

    // ���� �߻� ó��
    private void Shot()
    {
        //���� �߻� ó���� ȣ��Ʈ���� �븮
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        // ���� źȯ�� ���� -1
        magAmmo--;
        if (magAmmo <= 0)
        {
            // źâ�� ���� ź���� ���ٸ�, ���� ���� ���¸� Empty���� ����
            state = State.Empty;
        }
    }

    // ȣ��Ʈ���� ����Ǵ�, ���� �߻� ó��
    [PunRPC]
    private void ShotProcessOnServer()
    {
        //����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        //�Ѿ��� ���� ���� ������ ����
        Vector3 hitposition = Vector3.zero;
        
        //����ĳ��Ʈ(��������, ����, �浹 ���� �����̳�, �����Ÿ�)
        if(Physics.Raycast(fireTransform.position,fireTransform.forward,out hit, fireDistance))
        {
            //���̰� � ��ü�� �浹�� ���
            //�浹�� �������κ��� IDamageable ������Ʈ�� �������� �õ�
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            //�������κ��� IDamageable ������Ʈ�� �������� �� �����ߴٸ�
            if(target!= null)
            {
                //������ OnDamage �Լ��� ������Ѽ� ���濡�� ����� �ֱ�
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
            //���̰� �浹�� ��ġ ����
            hitposition = hit.point;
        }
        else
        {
            //���̰� �ٸ� ��ü�� �浹���� �ʾҴٸ�
            //�Ѿ��� �ִ� �����Ÿ����� ���ư��� ���� ��ġ�� �浹 ��ġ�� ���
            hitposition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        //�߻� ����Ʈ ���, ����Ʈ ����� ��� Ŭ���̾�Ʈ�鿡�� ����
        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitposition);
    }
    //����Ʈ ��� �ڷ�ƾ�� �����ϴ� �޼���
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitposition)
    {
        StartCoroutine(ShotEffect(hitposition));
    }

    // �߻� ����Ʈ�� �Ҹ��� ����ϰ� �Ѿ� ������ �׸���
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        // �ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        // ź�� ���� ȿ�� ���
        shellEjectEffect.Play();

        // �Ѱ� �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        // ���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // ���� ������ �Է����� ���� �浹 ��ġ
        bulletLineRenderer.SetPosition(1, hitPosition);
        // ���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���
        bulletLineRenderer.enabled = true;

        // 0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }

    // ������ �õ�
    public bool Reload()
    {
        if (state == State.Reloading ||
            ammoRemain <= 0 || magAmmo >= gunData.magCapacity)
        {
            // �̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�
            // źâ�� �Ѿ��� �̹� ������ ��� ������ �Ҽ� ����
            return false;
        }

        // ������ ó�� ����
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // ���� ������ ó���� ����
    private IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ
        state = State.Reloading;
        // ������ �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);

        // ������ �ҿ� �ð� ��ŭ ó���� ����
        yield return new WaitForSeconds(gunData.reloadTime);

        // źâ�� ä�� ź���� ����Ѵ�
        int ammoToFill = gunData.magCapacity - magAmmo;

        // źâ�� ä������ ź���� ���� ź�ຸ�� ���ٸ�,
        // ä������ ź�� ���� ���� ź�� ���� ���� ���δ�
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        // źâ�� ä���
        magAmmo += ammoToFill;
        // ���� ź�࿡��, źâ�� ä�ŭ ź���� �A��
        ammoRemain -= ammoToFill;

        // ���� ���� ���¸� �߻� �غ�� ���·� ����
        state = State.Ready;
    }

    
}
