# 메서드 파라미터 설명은 xml 주석으로 설명되어 있습니다.
# 대문자 필드는 프로퍼티입니다.

[ 스니펫 ]
/* Dependency */
/* Field & Property */
/* Intializer & Finalizer */
/* Public Method */
/* Private Method */

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Abocado ]

/* Field */
instances			get set		모든 아보카도 인스턴스

/* Method */
GrowUp			성장 
Harvest			수확
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Grid ]

/* Field */
instance			get set		싱글턴
CellSize			get			한 셀 너비
Spacing			get			셀 사이 공간
I				get			행 개수
J				get			열 개수

/* Method */
GetObject			그리드에서 [i][j] 오브젝트 반환

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
OnClick			타일 클릭 이벤트
SwitchWall			이동 가능 On Off
Create			프리팹 건설
Delete			프리팹 철거
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
death				get set inspector		체력이 0 이 되었을 때 실행될 대리자 (인스펙터 연결)
HP_max			get				게임 중 실시간 최대 체력
HP_current			get				게임 중 실시간 현재 체력
HP_ratio			get				게임 중 실시간 현재 체력 / 최대 체력

/* Method */
Damage			피해 입음
Heal				체력 회복
SetMaxHP			최대 체력 변경
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

/* Method */
SkipNight()			밤 스킵
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Launcher ]

/* Field */
instances			get set			모든 발사 장치 인스턴스
pool				get				발사체 풀
projectile			get set inspector		발사체
delay				get set inspector		발사 딜레이
count				get set inspector		발사체 수
speed				get set inspector		발사체 속도

/* Method */
Launch()			발사
ChangeProjectile		발사체 변경

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Projectile ]

/* Field */
instances			get set			모든 발사체 인스턴스
damage			get set inspector		발사체 데미지

/* Method */
InActive()							발사체 비활성화

@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
1	Tree 클릭 > Promotion 열림
2	버튼 클릭 : Upgrade.Promotion() > 특정 프리팹 타워로 프로모션
3	Tower 클릭 > Upgrade 열림
4	버튼 클릭 : 기존 타워의 모듈에 접근해서 능력치 UP !

[ Tower ]
/* 모듈 */
HP

포신 (동시 N발)
총알 (단일, 관통)
포탄 (광역)
생산 (? 막막한데..)
힐