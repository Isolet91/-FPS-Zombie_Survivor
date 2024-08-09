using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 오브젝트를 지속적으로 회전하는 스크립트
public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 60f;
  
    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
