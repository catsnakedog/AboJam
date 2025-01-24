using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using static ObjectPool;

public class Tower : MonoBehaviour
{
    /* Dependency */
    public HP hp;
    private Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    private Message message => Message.instance; // 하드 링크
    private Pool pool => Pool.instance; // 하드 링크
    private Grid grid => Grid.instance; // 하드 링크

    /* Field & Property */
    public static List<Tower> instances = new List<Tower>(); // 모든 타워 인스턴스
    public static Tower currentTower; // 최근 선택된 타워
    public int Level { get; private set; } // 현재 레벨
    public int MaxLevel { get; private set; } // 최대 레벨
    public int[] ReinforceCost { get { return reinforceCost; } private set { reinforceCost = value; } } // 레벨업 비용 (개수 = 최대 레벨 결정)

    /* Backing Field */
    [SerializeField] private int[] reinforceCost; 

    /* Intializer & Finalizer & Updater */
    public virtual void Start()
    {
        instances.Add(this);
        Load();
        hp.death.AddListener(() => StartCoroutine(CorDeath(2)));
        MaxLevel = ReinforceCost.Length;
    } // 최초 생성 시 (최초 초기화)
    private void OnDestroy()
    {
        instances.Remove(this);
    } // 오브젝트 삭제 시 (완전 제거)
    public virtual void Load()
    {
        Level = 0;
        hp.Load();
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save()
    {
        grid.GetNearestTile(gameObject.transform.position).UnBind();
    } // 풀에 집어 넣을 때 자동 실행
    /// <summary>
    /// 천천히 죽음을 맞이합니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CorDeath(float time)
    {
        // 스프라이트 제외 기능 정지
        Component[] components = GetComponents<Component>().Where(c => c.GetType() != typeof(SpriteRenderer)).ToArray(); ;
        SwitchComponents(components, false);

        // 투명화 대상 스프라이트
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        float[] startAlpha = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++) startAlpha[i] = spriteRenderers[i].color.a;

        // 투명도 새로고침 간격
        float delay = 0.01f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);

        // 투명화
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / time;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                float alpha = Mathf.Lerp(startAlpha[i], 0f, ratio);
                spriteRenderers[i].color = new UnityEngine.Color(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, alpha);
            }

            yield return waitForSeconds;
        }

        // 투명도 복원
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new UnityEngine.Color(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, startAlpha[i]);
        }

        // 기능 복구 후 풀에 집어넣기
        SwitchComponents(components, true);
        pool.Return(gameObject);

        /// <summary>
        /// 컴포넌트를 활성화 / 비활성화 합니다.
        /// </summary>
        void SwitchComponents(Component[] components, bool OnOff)
        {
            foreach (Component component in components)
            {
                if (component is MonoBehaviour monoBehaviour) monoBehaviour.enabled = OnOff;
                else if (component is Behaviour behaviour) behaviour.enabled = OnOff;
            }
        }
    }

    /* Public Method */
    /// <summary>
    /// 타워 클릭 시 상호작용 입니다.
    /// </summary>
    public void OnClick()
    {
        currentTower = this;
        reinforcement.On();
    }
    /// <summary>
    /// <br>타워를 증강합니다.</br>
    /// <br>공통 증강만 정의되어 있으며, 개별 증강은 자식 스크립트에서 구현됩니다.</br>
    /// </summary>
    public virtual void Reinforce()
    {
        message.On("타워 증강에 성공하였습니다.", 2f);

        // 타워 공통 증강
        Debug.Log($"{name} 증강");
        hp.Heal(hp.Hp_max);
        Level++;
    }
}
