using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static EnumData;

public class Abocado : RecordInstance<Table_Abocado, Record_Abocado>, IPoolee
{
    /* Dependency */
    public SpriteRenderer spriteRenderer; // 하이라키 연결
    public HP hp; // 하이라키 연결
    private Grid grid => Grid.instance; // 하드 링크
    private Pool pool => Pool.instance; // 하드 링크
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Abocado> instances = new List<Abocado>(); // 모든 아보카도 인스턴스
    public static Abocado currentAbocado; // 최근 선택된 아보카도
    private EnumData.Abocado level; public EnumData.Abocado Level { get => level; private set => level = value; } // 아보카도 레벨
    private int quality = 0; public int Quality { get => quality; private set => quality = value; } // 아보카도 품질 (Promotion)
    public int quality_max = 1; // 아보카도 최고 품질 (Promotion)
    public int harvest = 1; // 수확량
    public int harvestPlus = 1; // 수확 증가량
    private string path = "Images/Abocado/"; // 아보카도 이미지 Resources 경로
    private Sprite[] spr_level; // 레벨에 대응하는 스프라이트
    private (int, int) coord;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        int length_level = Enum.GetValues(typeof(EnumData.Abocado)).Length;
        spr_level = new Sprite[length_level];

        // 레벨에 맞는 이미지를 spr_level 에 바인딩합니다.
        for (int i = 0; i < length_level; i++)
        {
            Sprite temp = Resources.Load<Sprite>(path + (EnumData.Abocado)i);
            if (temp != null) spr_level[i] = temp;
            else spr_level[i] = Resources.Load<Sprite>(path + "Default");
        }
    } // 최초 생성 시 (최초 초기화 - 1)
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();
        instances.Add(this);

        hp.death.AddListener(() => StartCoroutine(CorDeath(2)));
        Date.instance.morningStart.AddListener(() => { this.GrowUp(); });
    } // 최초 생성 시 (최초 초기화 - 2)
    private void OnDestroy()
    {
        instances.Remove(this);
    } // 오브젝트 삭제 시 (완전 제거)
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();

        database_abojam.ExportAbocado(initialRecords[0].ID, ref level, ref quality, ref quality_max, ref harvest, ref harvestPlus);
        hp.Load();
        spriteRenderer.sprite = spr_level[0];
    } // 풀에서 꺼낼 때 / Import 시 자동 실행
    public void Save()
    {
        grid.GetNearestTile(gameObject.transform.position).UnBind();
    } // 풀에 집어 넣을 때 자동 실행

    /* Static Method */
    /// <summary>
    /// 타일에 아보카도를 건설합니다.
    /// </summary>
    public static void Cultivate(Tile tile)
    {
        // 빈 타일이면 풀에서 아보카도 꺼내서 타일이랑 바인딩
        if (tile.Go == null) tile.Bind(tile.pool.Get("Abocado"), EnumData.TileIndex.AboCado);
        else { Debug.Log($"타일 ({tile.i}, {tile.j}) 에 이미 {tile.Go.name} 가 바인딩 되어 있습니다."); return; };
    }

    /* Public Method */
    /// <summary>
    /// <br>아보카도가 성장합니다.</br>
    /// </summary>
    /// <param name="forced">Cultivated >> Seed 성장 On / Off</param>
    public void GrowUp(bool forced = false)
    {
        LevelUp(forced);
    }
    /// <summary>
    /// <br>아보카도를 수확합니다.</br>
    /// </summary>
    public void Harvest()
    {
        if (Level == EnumData.Abocado.Fruited)
        {
            StaticData.Abocado += harvest;
            LevelDown();
        }
    }
    /// <summary>
    /// <br>아보카도의 품질을 향상시킵니다.</br>
    /// <br>수확량이 증가하며, 이미지가 바뀝니다.</br>
    /// </summary>
    public void Promote()
    {
        if (Quality == quality_max)
        {
            Debug.Log("이미 최고 품질입니다.");
            return;
        }

        Quality++;
        harvest += harvestPlus;

        spr_level[(int)EnumData.Abocado.Tree] = Resources.Load<Sprite>($"{path}{EnumData.Abocado.Tree}{string.Concat(Enumerable.Repeat("+", Quality))}");
        spr_level[(int)EnumData.Abocado.Fruited] = Resources.Load<Sprite>($"{path}{EnumData.Abocado.Fruited}{string.Concat(Enumerable.Repeat("+", Quality))}");
        if (spr_level[(int)EnumData.Abocado.Tree] == null || spr_level[(int)EnumData.Abocado.Fruited] == null) Debug.Log("아보카도 품질에 맞는 이미지가 없습니다.");

        spriteRenderer.sprite = spr_level[(int)Level];
    }
    /// <summary>
    /// 아보카도 클릭 시 상호작용 입니다.
    /// </summary>
    public void OnClick()
    {
        currentAbocado = this;
        Reinforcement.instance.Off();
        Promotion.instance.Off();

        switch (Level)
        {
            // Cultivated : Seed 로 성장
            case EnumData.Abocado.Cultivated:
                if (StaticData.Abocado > 0)
                {
                    StaticData.Abocado--;
                    StaticData.gameData.abocado++;
                    GrowUp(true);
                }
                break;
            // Tree : 타워 업그레이드 패널 On
            case EnumData.Abocado.Tree:
                if (Quality == 0) Promotion.instance.On();
                break;
            // Fruited : 수확
            case EnumData.Abocado.Fruited:
                Harvest();
                break;
        }
    }
    /// <summary>
    /// 천천히 죽음을 맞이합니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CorDeath(float time)
    {
        string originTag = gameObject.tag;
        gameObject.tag = "Untagged";

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
        gameObject.tag = originTag;
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

    /* Private Method */
    private void LevelUp(bool forced = false)
    {
        // 개간 상태, 열매 상태는 레벨 업 X
        if ((Level == EnumData.Abocado.Cultivated || Level == EnumData.Abocado.Fruited) && forced == false) return;

        Level++;
        spriteRenderer.sprite = spr_level[(int)Level];
        hp.Heal(hp.Hp_max);
    }
    private void LevelDown()
    {
        // 개간 상태, 씨앗 상태는 레벨 다운 X
        if ((Level == EnumData.Abocado.Cultivated || Level == EnumData.Abocado.Seed)) return;

        Level--;
        spriteRenderer.sprite = spr_level[((int)Level)];
    }
}