using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Farmer : MonoBehaviour
{/*
 
[ 개간 ]

1	F 누르고 빈 타일 클릭하면 해당 위치로 이동
    중간에 UI 가 아닌 클릭 / 키 입력에 의한 캐릭터 이동 시 중단
    해당 위치 인근 n 거리 내 도착 시 중단 및 개간 시작

2	개간 동안 캐릭터 머리 위에 게이지바 시작
    중간에 UI 가 아닌 클릭 / 캐릭터 이동 시 중단
    게이지바 꽉 차면 중단 및 해당 타일에 아보카도 인스턴스화

[ 물주기 ]

1	F 누르고 Seed 인 아보카도 클릭하면 해당 위치로 이동
    중간에 UI 가 아닌 클릭 / 키 입력에 의한 캐릭터 이동 시 중단
    해당 위치 인근 n 거리 내 도착 시 중단 및 물주기 시작

2	물주는 동안 캐릭터 머리 위에 게이지바 시작
    중간에 UI 가 아닌 클릭 / 키 입력에 의한 캐릭터 이동 시 중단
    게이지바 꽉 차면 중단 및 해당 아보카도의 Water = True

*/}