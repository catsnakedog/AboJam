using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI Abocado;
    public TextMeshProUGUI Garu;
    public GameObject HP;
    public TextMeshProUGUI Date_Day;
    public TextMeshProUGUI Date_Time;

    private void Start()
    {
        StartCoroutine(CorUpdate());
    }

    public IEnumerator CorUpdate()
    {
        while (true)
        {
            Abocado.text = StaticData.Abocado.ToString();
            Garu.text = StaticData.Garu.ToString();
            Date_Day.text = "DAY - " + StaticData.Date_Day.ToString();
            Date_Time.text = FormatTime(StaticData.Date_Time);

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 분 단위 숫자를 00:00 형식 (시간:분)으로 변환합니다.
    /// </summary>
    public string FormatTime(int minutes)
    {
        int hours = (minutes / 60) + 6;
        int remainingMinutes = minutes % 60;
        return $"{hours}:{remainingMinutes}";
    }

    //private IEnumerator CorHP()
    //{
    //    //if (StaticData.HP_Player_Current == StaticData.HP_Player_Max) yield return new WaitForSeconds(0.05f);
    //    //if (slider.value > HP / StaticData.HP_Abocado) slider.value -= (StaticData.HP_Abocado * 0.07f);
    //    //yield return new WaitForSeconds(0.1f);
    //}
}
