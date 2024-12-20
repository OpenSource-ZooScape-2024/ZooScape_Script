# 🦁동물 러닝 게임 ZooScpae
![image](https://github.com/user-attachments/assets/56f14719-fbff-4615-b701-1dbe1a8335c1)

## 팀원 구성
<div align="center">

| **김학수** | **박찬수** | **정석영** | **조재혁** |
| :------: |  :------: | :------: | :------: |
| [<img src="https://avatars.githubusercontent.com/0hsk" height=150 width=150> <br/> @0hsk](https://github.com/0hsk) | [<img src="https://avatars.githubusercontent.com/Ch4nso0" height=150 width=150> <br/> @Ch4nso0](https://github.com/Ch4nso0) | [<img src="https://avatars.githubusercontent.com/g0311" height=150 width=150> <br/> @g0311](https://github.com/g0311) | [<img src="https://avatars.githubusercontent.com/pangqil" height=150 width=150> <br/> @pangqil](https://github.com/pangqil) |

</div>

## 프로젝트 소개
<div align="center">
  
[![Video Label](http://img.youtube.com/vi/0fFHI820DOQ/0.jpg)](https://youtu.be/0fFHI820DOQ?si=EGqeO5iEhiipLUto)
  
#### 동물 캐릭터를 기반으로 한 멀티플레이 러닝 액션 게임

<br>

</div>

<br>

## 시스템 구조
![image](https://github.com/user-attachments/assets/597e6608-7769-4f64-b59e-cfceeec4eb12)

- UGS 및 NGO를 통해 멀티플레이 구현
- Firebase의 Real Time Database를 통해 랭크 시스템 구현

<br>

## 개발 환경
- Unity 2021.3.43f1
- Firebase Realtime Database
- Unity Gaming Services (Lobby, Relay)

<br>

## 역할 분담

### 김학수
- **역할**: 캐릭터 구성 및 애니메이션 제작
- **담당 업무**:
  - 캐릭터 구성 및 디자인
  - 캐릭터 애니메이션 컨트롤러 제작
  - 캐릭터 로직 설계 및 구현

### 박찬수
- **역할**: 게임 디자인 및 배치 작업
- **담당 업무**:
  - 레벨 디자인 및 장애물 구현
  - 아이템 배치 및 이벤트 구간 설계

### 정석영
- **역할**: 멀티 프로그래밍, 동기화, 디버그 및 UI 구현
- **담당 업무**:
  - 로비 구현 및 호스트/조인 시스템 개발
  - 게임모드 및 오브젝트 동기화 작업
  - UI 설계 및 구현
  - 멀티플레이 동기화 문제 해결을 위한 디버깅
  - 씬 전환 로직 개발 및 게임 기능 구현

### 조재혁
- **역할**: 게임 로직 설계 및 기능 구현
- **담당 업무**:
  - 게임 목표 설정 및 캐릭터 기획
  - 캐릭터와 특수 능력 설계
  - 아이템 설계 및 구현


<br>

## 조작 방법
- **이동**: `W`, `A`, `S`, `D` 키를 사용해 캐릭터를 이동, `LShift` 키를 이용해 대쉬 
- **아이템 사용**: `F` 키를 눌러 획득한 아이템을 사용
- **스킬 사용**: `R` 키를 눌러 캐릭터의 고유 스킬을 발동

<br>

## 개발 일지


### 2024년 12월 20일
- **[FIX] Refactor** `g0311` - 리펙토링
- **[FIX] Refactor** `g0311` - 리펙토링

### 2024년 12월 11일
- **[FIX] Point Logic & Car Speed** `g0311` - 포인트 로직 및 차 속도 조정
- **[FIX] SawTrap Movement** `g0311` - 톱 이동 로직 수정
- **[FIX] Point Logic** `g0311` - 포인트 로직 수정
- **[ADD] Progress Bar** `g0311` - 진행 상태 UI 추가

### 2024년 12월 10일
- **[FIX] Player Force Quit** `g0311` - 플레이어 강제 종료 처리
- **[MERGE]** `g0311` - `gsy` 브랜치 병합
- **[FIX] DEBUG** `g0311` - 디버그 관련 문제 수정

### 2024년 12월 9일
- **[UPDATE] Obstacles** `Ch4nso0` - 장애물 관련 업데이트
- **[FIX] Projectile Mat** `g0311` - 발사체 매트 수정
- **[ADD] Police Car Effect** `g0311` - 경찰차 이펙트 추가
- **[ADD] Loading Scene** `g0311` - 로딩 씬 추가

### 2024년 12월 8일
- **[MERGE]** `g0311` - `gsy` 브랜치 병합
- **[FIX] Character Refactor** `g0311` - 캐릭터 리팩토링
- **[FIX] Rank Sort** `g0311` - 랭크 정렬 수정
- **[FIX] Game Over Sync & UI** `g0311` - 게임 오버 관련 동기화 및 UI 수정

### 2024년 12월 7일
- **[FIX] Sync** `g0311` - 동기화 관련 버그 수정
- **[ADD] Jump sound** `0hsk` - 점프 사운드 추가
- **[FIX] Menu Scene Update** `g0311` - 메뉴 씬 업데이트
- **[FIX] HUD Update** `g0311` - HUD 업데이트
- **[ADD] Item & Character Shader** `g0311` - 아이템과 캐릭터 셰이더 추가
- **[ADD] Obstacle SFX & Collide Effect** `g0311` - 장애물 SFX 및 충돌 이펙트 추가

### 2024년 12월 6일
- **[FIX] sound** `0hsk` - 사운드 수정
- **[UPDATE] effect, sound** `0hsk` - 이펙트 및 사운드 업데이트
- **[ADD] Toon Shader** `g0311` - 툰 셰이더 추가
- **[FIX] Obstacle Sync** `g0311` - 장애물 동기화 수정

### 2024년 12월 5일
- **[UPDATE] MAP** `Ch4nso0` - 맵 업데이트
- **[FIX] Ability Effect Spawn** `g0311` - 능력 이펙트 스폰 관련 수정
- **[UPDATE] Effect** `0hsk` - 이펙트 업데이트
- **[ADD] prefab aura and sfx setting** `pangqil` - 프리팹 오라 및 SFX 설정 추가

### 2024년 12월 4일
- **[MERGE]** `g0311` - `gsy` 브랜치 병합
- **[FIX] Firebase Dependency** `g0311` - Firebase 의존성 문제 수정
- **[ADD] Ability & Item SFX attr** `g0311` - 능력 및 아이템 SFX 속성 추가
- **[FIX] Ability Effect Async** `g0311` - 능력 이펙트 비동기 처리 수정
- **[FIX] Obj Pool Sync** `g0311` - 객체 풀 동기화 문제 수정

### 2024년 11월 30일
- **[ADD] Game End Routine** `g0311` - 게임 종료 루틴 추가
- **[ADD] Effect** `pangqil` - 효과 추가

### 2024년 11월 28일
- **[UPDATE] Cobra** `Ch4nso0` - Cobra 업데이트
- **[ADD] policecar sound** `0hsk` - 경찰차 사운드 추가
- **[FIX] Merge** `g0311` - 병합 관련 문제 수정
- **[UPDATE] MAP** `Ch4nso0` - 맵 업데이트

### 2024년 11월 27일
- **[Edit] DistanceHUD** `pangqil` - 거리 HUD 수정
- **[FIX] debug status** `g0311` - 디버그 상태 수정
- **[UPDATE] MAP** `Ch4nso0` - 맵 업데이트
- **[ADD] HUD_ Point & Item** `g0311` - HUD 포인트 및 아이템 추가
- **[ADD] HUD CoolDowns** `g0311` - HUD 쿨다운 추가
- **[FIX] Merge Conflict** `g0311` - 병합 충돌 해결

### 2024년 11월 26일
- **[UPDATE] character** `0hsk` - 캐릭터 업데이트
- **[FIX] revert merge** `g0311` - 병합 취소 및 수정
- **[FIX] Merge Conflict** `g0311` - 병합 충돌 해결
- **[ADD] Chaser** `pangqil` - 추적자 추가
- **[ADD] SteakSpawner & [UPDATE] ObstacleManager** `Ch4nso0` - 스테이크 스폰너 추가 및 장애물 관리자 업데이트

### 2024년 11월 24일
- **[MERGE]** `g0311` - `hsk` 브랜치 병합
- **[ADD] Map** `Ch4nso0` - 맵 추가
- **[ADD] Game Manager** `g0311` - 게임 매니저 추가

### 2024년 11월 22일
- **[FIX] Firebase Dependency** `g0311` - Firebase 의존성 문제 수정
- **[TEST]** `g0311` - 테스트 커밋
- **[TEST]** `g0311` - 추가 테스트

### 2024년 11월 21일
- **[ADD] Anot Sync** `g0311` - Anot 동기화 추가
- **[ADD] Relay Manager** `g0311` - 릴레이 관리자 추가
- **[UPDATE] Character** `0hsk` - 캐릭터 업데이트

### 2024년 11월 20일
- **TEMP** `g0311` - 임시 작업 커밋
- **TEMP** `g0311` - 임시 작업 커밋
- **[ADD] animal ability** `0hsk` - 동물 능력 추가

### 2024년 11월 14일
- **[ADD] character** `0hsk` - 캐릭터 추가
- **[ADD] police officer** `0hsk` - 경찰관 캐릭터 추가
- **[ADD] Lobby UI & Scripts** `g0311` - 로비 UI 및 스크립트 추가

### 2024년 11월 13일
- **Revert "first commit"** `pangqil` - 첫 커밋 되돌리기
- **first commit** `pangqil` - 첫 커밋
- **[ADD] Folder Struct** `g0311` - 폴더 구조 추가

### 2024년 11월 11일
- **[Update] Lobby List UI** `g0311` - 로비 리스트 UI 업데이트

### 2024년 11월 10일
- **[ADD] LobbyManager** `g0311` - 로비 관리자 추가

### 2024년 11월 9일
- **[UPDATE] Resources** `g0311` - 자원 업데이트
- **[ADD] Resources** `g0311` - 자원 추가
- **[Update] .gitignore** `g0311` - `.gitignore` 파일 업데이트
- **[Merge]** `g0311` - `develope` 브랜치 병합
- **[ADD] Lobby Code and Lobby UI** `g0311` - 로비 코드 및 UI 추가

### 2024년 11월 2일
- **[Merge]** `g0311` - `develope` 브랜치 병합
- **[First Commit]** `g0311` - 첫 커밋
