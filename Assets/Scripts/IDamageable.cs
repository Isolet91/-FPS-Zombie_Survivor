using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ���� �� �ִ� Ÿ�Ե��� ���������� ������ �ϴ� �������̽�
public interface IDamageable
{
    // �������� ���� �� �ִ� Ÿ�Ե��� IDamageable�� ����ϰ� OnDamage �޼��带 �ݵ�� �����ؾ� ��
    // OnDamage �޼���� �Է����� ������ ũ��(damage), ���� ����(hitPoint), ���� ǥ���� ����(hitNormal)�� �޴´�
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}