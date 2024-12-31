# 메서드 파라미터 설명은 xml 주석으로 설명되어 있습니다.
# 대문자 필드는 프로퍼티입니다.

[ 스니펫 ]
/* Dependency */
/* Field & Property */
/* Intializer & Finalizer & Updater */
/* Public Method */
/* Private Method */

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Abocado ]

/* Field */
instances			get set		모든 아보카도 인스턴스

/* Method */
GrowUp						성장 
Harvest						수확
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Grid ]

/* Field */
instance			get set		싱글턴
CellSize			get			한 셀 너비
Spacing			get			셀 사이 공간
I				get			행 개수
J				get			열 개수

/* Method */
GetObject						그리드에서 [i][j] 오브젝트 반환

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Tile ]

/* Field */
instances			get set		모든 타일 인스턴스
currentTile			get set		마지막으로 클릭한 타일
Go				get set		타일에 배치된 건물 (아보카도 / 타워)
I				get			행 번호 (시작 1)
J				get			열 번호 (시작 1)
Index				get			타일 고유 번호
IsWall				get			벽 On / Off
		
/* Method */
OnClick						타일 클릭 이벤트
SwitchWall						이동 가능 On Off
Create						프리팹 건설
Delete						프리팹 철거
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Promotion ]

/* Field */
instance			get set		싱글턴
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ HP ]

/* Field */
HP_MAX			inspector			초기 최대 체력
speed				inspector			체력바 UI 조절 속도
HideFullHP			inspector			최대 체력일 때 체력바 UI 숨기기
instances			get set			모든 HP 인스턴스
death				get set inspector		체력이 0 이 되었을 때 실행될 대리자 (인스펙터 연결)
HP_max			get				게임 중 실시간 최대 체력
HP_current			get				게임 중 실시간 현재 체력
HP_ratio			get				게임 중 실시간 현재 체력 / 최대 체력

/* Method */
Damage							피해 입음
Heal								체력 회복
SetMaxHP							최대 체력 변경
FindHP			static				오브젝트의 HP 인스턴스 탐색
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Date ]

/* Field */
instance			get set			싱글턴
timeFlow			get set inspector		시간 흐름 On / Off
secondsPerDay		inspector			게임의 하루 = secondsPerDay 초
startTime			inspector			게임 시작 시각
morningStartTime		get set inspector		낮 시작 시각
morningEndTime		get set inspector		낮 종료 시각
isMorning			get				Morning / Night 조회
dateTime			get set			게임 시각
morningStart		get set inspector		아침 시작 시 실행될 메서드
nightStart			get set inspector		밤 시작 시 실행될 메서드

/* Method */
SkipNight							밤 스킵
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Launcher ]

/* Field */
instances			get set			모든 발사 장치 인스턴스
pool				get				발사체 풀
pool_hierarchy		get				발사체 풀 (하이라키 정리용)
projectile			get set inspector		발사체
speed				get set inspector		발사체 속도
range				get set			발사체 유효 사거리

/* Method */
Launch							발사
ChangeProjectile						발사체 변경

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Projectile ]

/* Dependency */
colider2D			get set			발사체 콜라이더

/* Field */
instances_enable		get set			모든 발사체 인스턴스 (활성화)
instances_disable		get set			모든 발사체 인스턴스 (비활성화)
clashTag			get set inspector		충돌 타겟 태그 지정
damage			get set inspector		발사체 데미지
penetrate			get set inspector		총 관통 수

/* Method */
Clash								타겟에 명중
Disappear							비활성화
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Explosion ]

/* Field */
radius				폭발 반지름

/* Method */
Explode							폭발 : 피해 입힘, 애니메이션 재생

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Tower ]

/* Dependency */
hp				get set			체력 모듈

/* Field & Property */
instances			get set			모든 타워 인스턴스
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Auto : Tower ]

/* Field & Property */
instances			get set			모든 연사 타워 인스턴스
launcher			get set			발사체 모듈
delay				inspector			공격 딜레이
detection			get set inspector		적 감지 범위

/* Public Method */
Fire								공격 On / Off
SetDelay							공격 딜레이 재설정
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Auto : Guard ]

/* Field & Property */
instances			get set			모든 타워 인스턴스
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ


ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ

1	1 발 = 목적지
2	2 발 = 양 끝
3	3 발 = 양 끝 + 2 등분 지점
4	4 발 = 양 끝 + 3 등분 지점
5	5 발 = 양 끝 + 4 등분 지점
6	6 발 = 양 끝 + 5 등분 지점


Vector3 ortho = origin 의 직교
Vector3 left = origin * -angle / 2
Vector3 right = origin * angle / 2

Vector3 leftEndPoint = 
Vector3 rightEndPoint


@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
1	Tree 클릭 > Promotion 열림
2	버튼 클릭 : Upgrade.Promotion() > 특정 프리팹 타워로 프로모션
3	Tower 클릭 > Upgrade 열림
4	버튼 클릭 : 기존 타워의 모듈에 접근해서 능력치 UP !


밤 시작
아침 시작

// 일정 거리 까지 투사

목적지 도착하면 증발해버리니까 관통이 안됨
타겟 위치에서 터트리기 vs 직선으로 쭉 쏘기

포탄 (광역)
생산 (? 막막한데..)
힐