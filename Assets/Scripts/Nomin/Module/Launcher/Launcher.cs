using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderData;

public class Launcher : MonoBehaviour
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public GameObject projectile;
    public Pool pool => Pool.instance;
    public Targeter targeter; // 조준경
    public SpriteRenderer spriteRenderer;
    public enum Horizontal
    {
        Left,
        Center,
        Right,
    }
    public enum Vertical
    {
        Top,
        Center,
        Bottom,
    }
    [Serializable] public struct Align
    {
        public Align(Horizontal horizontal, Vertical vertical)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        public Horizontal horizontal;
        public Vertical vertical;
    }

    /* Field & Property */
    public static List<Launcher> instances = new List<Launcher>(); // 모든 Launcher 인스턴스

    [Header("[ Launcher ]")]
    public bool align = false;
    public float turnTime = 0.1f; // 포신 회전 시간
    public float angleOffset = 0f; // 회전 보정치
    public float frame = 60; // 초당 회전 변화
    public UnityEvent launchEvent; // 발사 시 추가할 이벤트
    private float delay;
    private WaitForSeconds waitForSeconds;
    private Coroutine corLast;

    [Header("[ Launch ]")]
    [SerializeField] private Align muzzleAlign = new Align(Horizontal.Center, Vertical.Top); // 발사 위치 정렬
    public Align MuzzleAlign
    {
        get => muzzleAlign;
        set
        {
            muzzleAlign = value;
            AlignLauncher();
        }
    }
    public float speed = 0.02f; // 발사체 속도
    public float range = 5f; // 발사체 유효 사거리 (!= 타겟 감지 거리)
    private float width;
    private float height;
    private Vector3 offset = Vector3.zero; // 발사 위치 보정

    /* Intializer & Finalizer */
    private void Awake()
    {
        delay = 1 / frame;
        waitForSeconds = new WaitForSeconds(delay);
    }
    private void Start()
    {
        instances.Add(this);
        if (projectile == null) Debug.Log($"{gameObject.name} 의 Launcher 에 Projectile 이 연결되지 않았습니다.");
        width = spriteRenderer.bounds.size.x;
        height = spriteRenderer.bounds.size.y;

        AlignLauncher();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    private void OnValidate()
    {
        MuzzleAlign = muzzleAlign;
    }

    /* Public Method */
    /// <summary>
    /// 발사체를 목적지까지 등속으로 발사합니다.
    /// </summary>
    /// <param name="destination">목적지</param>
    /// <param name="angle">발사각 변경</param>
    public void Launch(Vector3 destination, float angle = 0f)
    {
        // 발사체 장전 (풀링 or 생성)
        GameObject projectile = pool.Get(this.projectile.gameObject.name);
        projectile.GetComponent<Projectile>().launcher = gameObject;
        projectile.transform.position = transform.position;

        // 발사각 정렬
        if (angle != 0)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * (destination - transform.position);
            destination = transform.position + direction;
        }

        StartCoroutine(CorLaunch(projectile, destination));
    }
    /// <summary>
    /// 일정 거리 이내의 적을 자동 타게팅하여 발사합니다.
    /// </summary>
    /// <param name="targetType">타게팅 기준</param>
    /// <param name="detection">타겟 감지 거리 (!= 유효 사거리)</param>
    /// <param name="ratio">일정 비율 이하의 체력만 타게팅 (0 ~ 1)</param>
    /// <param name="angle">발사각 변경</param>
    public void Launch(Targeter.TargetType targetType, float detection, float ratio = 1f, float angle = 0f)
    {
        GameObject temp = pool.Get(projectile.name);
        GameObject target = targeter.Targetting(targetType, temp.GetComponent<Projectile>().ClashTags, detection, ratio);
        pool.Return(temp);

        if (target != null) Launch(target.transform.position, angle);
    }

    /* Private Method */
    /// <summary>
    /// 발사체를 목표 지점까지 등속 운동 시킵니다.
    /// </summary>
    /// <param name="projectile">발사체</param>
    /// <param name="destination">목적지</param>
    private IEnumerator CorLaunch(GameObject projectile, Vector3 destination)
    {
        launchEvent?.Invoke();

        // 발사기 각도 정렬
        if (align == true)
        {
            if (corLast != null) StopCoroutine(corLast);
            corLast = StartCoroutine(CorTurn(destination));
        }

        // 발사체 위치 정렬
        Vector3 startPos = transform.position + (transform.rotation * offset);
        projectile.transform.position = startPos;
        AlignProjectile(projectile, destination);

        while (projectile != null && projectile.activeSelf == true)
        {
            // 발사체 위치 += 진행 방향 * speed
            Vector3 direction = (destination - startPos).normalized;
            projectile.transform.position += direction * speed;

            // 사거리를 벗어나면 비활성화
            if (range < (projectile.transform.position - startPos).magnitude) pool.Return(projectile.GetComponent<Projectile>().gameObject);

            yield return new WaitForSeconds(0.016f); // 대략 60 프레임 기준
        }
    }
    /// <summary>
    /// 발사기를 부드럽게 회전시킵니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorTurn(Vector3 target)
    {
        Quaternion startQuaternion = transform.rotation;
        Quaternion targetQuaternion = HandUtil.ForwardToObj(gameObject, target, angleOffset);

        float elapsedTime = 0f;

        while (elapsedTime < turnTime)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / turnTime;
            if (ratio > 1) ratio = 1;

            transform.rotation = Quaternion.Lerp(startQuaternion, targetQuaternion, ratio);
            yield return waitForSeconds;
        }

        corLast = null;
    }
    /// <summary>
    /// <br>발사체가 타겟을 바라보게끔 정렬합니다.</br>
    /// <br>y+ 축으로 정렬된 발사체 기준이며, 로컬 축이 다를 경우 보정값 (angle) 을 입력합니다.</br>
    /// </summary>
    /// <param name="projectile">발사체</param>
    /// <param name="destination">타겟</param>
    /// <param name="angle">보정값</param>
    private void AlignProjectile(GameObject projectile, Vector3 destination, float angle = 0)
    {
        Vector3 direction = destination - projectile.transform.position;
        float value = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f + angle;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, value);
    }
    /// <summary>
    /// <br>발사 위치를 정렬합니다.</br>
    /// </summary>
    private void AlignLauncher()
    {
        offset = Vector3.zero;

        switch (muzzleAlign.horizontal)
        {
            case Horizontal.Left:
                offset.x -= width / 2;
                break;
            case Horizontal.Right:
                offset.x += width / 2;
                break;
        }
        switch (muzzleAlign.vertical)
        {
            case Vertical.Top:
                offset.y += height / 2;
                break;
            case Vertical.Bottom:
                offset.y -= height / 2;
                break;
        }
    }

    /* Test Method */
    [ContextMenu("LaunchTest")]
    private void LaunchTest()
    {
        Launch(Vector3.zero);
    }
}