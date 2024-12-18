using System;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;          // 총알 속도
    public float damage;           // 총알 데미지
    public int penetration;      // 관통 횟수
    public float maxDistance;    // 최대 거리

    private Vector3 direction;   // 총알 방향
    private float traveledDistance; // 이동한 거리
    private Type type = typeof(Bullet);

    private bool active = false;

    public void Init(Quaternion rot ,Vector3 dir, float dmg, int pen, float dist, float spd)
    {
        transform.rotation = rot;
        direction = dir.normalized;
        damage = dmg;
        penetration = pen;
        maxDistance = dist;
        speed = spd;
        traveledDistance = 0f;
        active = true;
    }

    void FixedUpdate()
    {
        if (!active)
            return;
        float step = speed * Time.deltaTime;
        transform.position += direction * step;
        traveledDistance += step;

        if (traveledDistance >= maxDistance || penetration <= 0)
        {
            ObjectPool.Instance.Return(type, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }



    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
        direction = Vector3.zero;
        damage = 0;
        penetration = 0;
        maxDistance = 0;
        speed = 0;
        traveledDistance = 0f;
        active = false;
    }
}