using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    /* Field & Property */
    public enum TargetType
    {
        /// <summary>
        /// 가장 가까운 적을 타게팅
        /// </summary>
        Near,
        /// <summary>
        /// 가장 낮은 체력의 적을 타게팅
        /// </summary>
        LowHP,
    }

    /* Public Method */
    /// <summary>
    /// <br>감지 범위 이내에서 대상을 지정합니다.</br>
    /// <br>타겟이 없다면 null 을 반환합니다.</br>
    /// </summary>
    /// <param name="ratio">TargetType.LowHP 에서 일정 비율 이하의 체력만 타게팅 (0 ~ 1)</param>
    /// <returns>가장 가까운 적</returns>
    public GameObject Targetting(TargetType targetType, string[] tags, float detection, float ratio = 1.0f)
    {
        if (transform.position == Vector3.zero) { Debug.Log("(0, 0, 0) 에선 풀링을 위해 타게팅이 취소됩니다."); return null; }

        switch (targetType)
        {
            case TargetType.Near:
                return Near(tags, detection);
            case TargetType.LowHP:
                return LowHP(tags, detection, ratio);
            default:
                Debug.Log($"지정한 {targetType} 의 동작이 정의되지 않았습니다.");
                return null;
        }
    }
    /// <summary>
    /// <br>감지 범위 이내에서 가장 가까운 오브젝트를 반환합니다.</br>
    /// </summary>
    public GameObject Near(string[] tags, float detection)
    {
        List<GameObject> targets = GetTargets(tags, detection);

        if (targets.Count > 0) return targets[0];
        else return null;
    }
    /// <summary>
    /// <br>감지 범위 이내에서 체력이 가장 낮은 오브젝트를 반환합니다.</br>
    /// </summary>
    /// <param name="ratio">일정 비율 이하의 체력만 타게팅 (0 ~ 1)</param>
    /// <returns></returns>
    public GameObject LowHP(string[] tags, float detection, float ratio = 1.0f)
    {
        List<GameObject> targets = GetTargets(tags,detection);
        List<KeyValuePair<GameObject, float>> targetsWithHP = new List<KeyValuePair<GameObject, float>>();
        foreach (HP HP in HP.instances)
        {
            // 타겟의 HP 인스턴스 특정
            if (targets.Contains(HP.entity))
            {
                // 일정 비율 미만의 HP 만 타게팅
                if (HP.HP_current < HP.Hp_max * ratio) targetsWithHP.Add(new KeyValuePair<GameObject, float>(HP.entity, HP.HP_current));
            }
        }
        targetsWithHP = targetsWithHP.OrderBy(pair => pair.Value).ToList();

        if (targetsWithHP.Count > 0) return targetsWithHP[0].Key;
        else return null;
    }
    /// <summary>
    /// 감지 범위 이내의 태그된 타겟을 반환합니다. (거리 순 ASC 정렬)
    /// </summary>
    public List<GameObject> GetTargets(string[] tags, float detection)
    {
        List<GameObject> targets = GetTaged(tags);
        List<KeyValuePair<GameObject, float>> withDistance = GetDistances(targets);
        List<KeyValuePair<GameObject, float>> InRange = CheckRange(withDistance, detection);
        return InRange.Select(pair => pair.Key).ToList();
    }
    /// <summary>
    /// <br>태그에 해당하는 모든 오브젝트를 반환합니다.</br>
    /// <br>단, 현재 오브젝트를 포함한 조상 계층은 제외합니다.</br>
    /// </summary>
    public List<GameObject> GetTaged(string[] tags)
    {
        List<GameObject> targets = new List<GameObject>();
        foreach (string tag in tags)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
            {
                targets.Add(go);
            }
        }
        InnerJoin(targets, GetParentList());

        return targets;
    }
    /// <summary>
    /// 타겟 오브젝트들의 거리를 계산하여 거리 순 ASC 정렬 후 반환합니다.
    /// </summary>
    public List<KeyValuePair<GameObject, float>> GetDistances(List<GameObject> objects)
    {
        List<KeyValuePair<GameObject, float>> gos = new List<KeyValuePair<GameObject, float>>();

        foreach (GameObject go in objects)
        {
            gos.Add(new KeyValuePair<GameObject, float>(go, Vector3.Distance(transform.position, go.transform.position)));
        }

        return gos.OrderBy(pair => pair.Value).ToList(); ;
    }
    /// <summary>
    /// 사거리 이내의 요소만 반환합니다.
    /// </summary>
    public List<KeyValuePair<GameObject, float>> CheckRange(List<KeyValuePair<GameObject, float>> objects, float detection)
    {
        return objects.Where(pair => pair.Value <= detection).ToList();
    }

    /* Private Method */
    /// <summary>
    /// 타게터의 오브젝트를 포함한 모든 조상 오브젝트를 리스트로 반환합니다.
    /// </summary>
    /// <returns>조상 오브젝트 리스트</returns>
    private List<GameObject> GetParentList()
    {
        List<GameObject> parents = new List<GameObject>();
        Transform current = transform;

        while (current != null)
        {
            parents.Add(current.gameObject);
            current = current.parent;
        }

        return parents;
    }
    /// <summary>
    /// A 리스트에서 B 리스트의 요소를 제거합니다.
    /// </summary>
    private void InnerJoin(List<GameObject> listA, List<GameObject> listB)
    {
        listA.RemoveAll(item => listB.Contains(item));
    }
}
