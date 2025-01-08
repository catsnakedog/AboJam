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
level				get			성장 레벨 (경작 / 씨앗 / 나무 / 열매)
quality			static			품질
quality_max			get set		최고 품질
harvest			get set		아보카도 수확량

/* Method */
GrowUp						성장
Harvest						수확
Promotion						품질 강화 (수확량 UP)
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Grid ]

/* Field */
instance			get set		싱글턴
Row				get, inspector	행 개수
Column			get, inspector	열 개수
StartPos			get			맵의 맨 왼쪽 위 꼭짓점 좌표
Width				get			맵 너비
Height			get			맵 높이
CellWidth			get			한 칸 너비
CellHeight			get			한 칸 높이

/* Method */
GetObject						그리드에서 [i][j] 오브젝트 반환
Create						(i, j) 에 오브젝트 생성
Delete						(i, j) 에  오브젝트 제거
GetTile						(i, j) 에  타일 반환
GetNearestTile					특정 좌표와 가장 가까운 타일 반환

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Tile ]

/* Field */
instances			get set		모든 타일 인스턴스
Go				get set		타일에 배치된 프리팹 (아보카도 / 타워) 
i				get			행 번호 (시작 0)
j				get			열 번호 (시작 0)
pos				get			타일 중심 좌표
		
/* Method */
OnClick						타일 클릭 이벤트
Create						프리팹 건설
Delete						프리팹 철거
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Promotion ]

/* Field */
instance			get set inspector	싱글턴
price_guard			get set inspector	방어 타워 건설 비용
price_auto			get set inspector	연사 타워 건설 비용
price_production		get set inspector	생산 타워 건설 비용
price_heal			get set inspector	회복 타워 건설 비용
price_splash			get set inspector	광역 타워 건설 비용
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

/* Module */
Targeter
Projectile

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
[ Targeter ]

/* Method */
Targetting							범위 내 타입에 맞는 단일 대상 반환
Near								범위 내 가장 가까운 대상 반환
LowHP							범위 내 가장 체력 낮은 대상 반환
GetTargets							범위 내 태그된 오브젝트 모두 반환
GetTaged							태그된 오브젝트 모두 반환
GetDistances						리스트를 <오브젝트, 거리> 로 치환
CheckRange							리스트를 범위 내로 필터링 후 반환

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Projectile ]
/* Module */
Explosion

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

/* Module */
Targeter

/* Field */
radius				get set inspector		폭발 반경
damage			get set inspector		폭발 데미지
time				get set inspector		폭발 시간

/* Method */
Explode							폭발
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Tower ]

/* Module */
HP

/* Field & Property */
instances			get set			모든 타워 인스턴스

/* Method */
Reinforce							타워 증강
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Auto : Tower ]

/* Module */
Launcher

/* Field & Property */
instances			get set			모든 연사 타워 인스턴스
delay				inspector			공격 딜레이
detection			get set inspector		적 감지 범위

/* Public Method */
Fire								공격 On / Off
SetDelay							공격 딜레이 재설정
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Guard : Tower ]

/* Field & Property */
instances			get set			모든 타워 인스턴스
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Heal : Tower ]

/* Module */
Launcher

/* Field & Property */
instances			get set			모든 회복 타워 인스턴스
delay				inspector			공격 딜레이
ratio				get set inspector		체력 회복 기준 비율
detection			get set inspector		아군 감지 범위

/* Public Method */
Healing							회복 On / Off
SetDelay							회복 딜레이 재설정
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Inventory ]

/* Field & Property */
instance			get set			싱글턴

/* Public Method */
UpdateAbocado						아보카도 개수 갱신
UpdateGaru							가루 개수 갱신
UpdateWater						물 이미지 갱신
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Close ]

/* Field & Property */
instance			get set			싱글턴
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Change ]

/* Field & Property */
instance			get set			싱글턴
Price				get set			아보카도 가격
Quantity			get set			회당 거래 개수

/* Public Method */
Trade								아보카도 판매

ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ BTN_Weapons ]

/* Field & Property */
instances_melee		get set			근거리 무기 버튼 인스턴스
instances_range		get set			원거리 무기 버튼 인스턴스
Price				get set inspector		상품 가격
purchase			inspector			구매 여부

/* Method */
Buy								현재 버튼의 무기 구매
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ BTN_Upgrade ]

/* Field & Property */
instances			get set			모든 버튼 인스턴스
prices				inspector			모든 업그레이드 비용
Price				get set			현재 업그레이드 비용
Level				get				현재 레벨
maxLevel			inspector			만랩

/* Method */
Upgrade							현재 버튼의 업그레이드 구매
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Upgrade ] : 모든 업그레이드 버튼에 접근할 수 있습니다.

/* Field & Property */
BTN_Damage		get set			버튼 : 데미지
BTN_Speed			get set			버튼 : 공격 속도
BTN_Range			get set			버튼 : 사거리
BTN_Knockback		get set			버튼 : 넉백
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Weapons ] : 모든 무기 버튼에 접근할 수 있습니다.

/* Field & Property */
BTN_Gun			get set			버튼 : 권총
BTN_Shotgun		get set			버튼 : 샷건
BTN_Sniper			get set			버튼 : SR
BTN_Riple			get set			버튼 : AR
BTN_Knife			get set			버튼 : 칼
BTN_Bat			get set			버튼 : 방망이
BTN_Spear			get set			버튼 : 창
BTN_Chainsaw		get set			버튼 : 전기톱
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ OnOff ] : 상점을 끄거나 킵니다.

/* Method */
Switch							상점 On / Off
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Message ]

/* Method */
On								메시지 출력
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ RayCaster2D ]

/* Field & Property */
instance			get set			싱글턴

/* Method */
RayCastUI							UI 타겟 다수 레이캐스팅
RayCast							UI 제외 단일 레이캐스팅
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ RayCastee2D ]

/* Field & Property */
instances			get set			모든 인스턴스
unityEvent			get set			캐스팅 시 이벤트 핸들러

/* Method */
OnClick							캐스팅 시 이벤트 Invoke
ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
[ Receiver ]

/* Field & Property */
instance			get set			싱글턴