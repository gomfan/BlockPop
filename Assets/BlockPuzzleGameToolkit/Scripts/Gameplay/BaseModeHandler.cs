using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using TMPro;
using UnityEngine;
using System.Collections;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    /// <summary>
    /// 게임 모드의 기본 동작을 정의하는 추상 클래스입니다.
    /// 점수 관리, UI 업데이트, 게임 상태 저장/로드 등의 공통 기능을 담당합니다.
    /// </summary>
    public abstract class BaseModeHandler : MonoBehaviour
    {
        [Header("UI References")]
        // 현재 점수를 표시할 UI 텍스트
        public TextMeshProUGUI scoreText;
        // 최고 점수를 표시할 UI 텍스트
        public TextMeshProUGUI bestScoreText;

        [HideInInspector]
        // 현재 최고 점수 저장 변수
        public int bestScore;

        [HideInInspector]
        // 현재 게임 점수 저장 변수
        public int score;

        // 레벨 매니저 참조 (게임의 전반적인 상태 관리)
        protected LevelManager _levelManager;
        // 점수 카운팅 애니메이션을 위한 코루틴 참조
        protected Coroutine _counterCoroutine;
        // 화면에 표시되는 현재 점수 (애니메이션용)
        protected int _displayedScore = 0;
        
        [SerializeField]
        // 점수가 올라가는 애니메이션 속도
        protected float counterSpeed = 0.01f;

        /// <summary>
        /// 객체가 활성화될 때 호출됩니다.
        /// 초기화 및 이벤트 구독을 수행합니다.
        /// </summary>
        protected virtual void OnEnable()
        {
            // 씬에서 LevelManager를 찾습니다.
            _levelManager = FindObjectOfType<LevelManager>(true);
            
            if (_levelManager == null)
            {
                Debug.LogError("LevelManager not found!");
                return;
            }

            // 게임 패배 및 점수 획득 이벤트에 함수를 연결합니다.
            _levelManager.OnLose += OnLose;
            _levelManager.OnScored += OnScored;

            // 저장된 점수를 불러옵니다.
            LoadScores();
        }

        /// <summary>
        /// 객체가 비활성화될 때 호출됩니다.
        /// 이벤트 연결을 해제하여 메모리 누수를 방지합니다.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_levelManager != null)
            {
                _levelManager.OnLose -= OnLose;
                _levelManager.OnScored -= OnScored;
            }
        }

        /// <summary>
        /// 앱이 일시정지될 때 호출됩니다.
        /// 게임 중이라면 상태를 저장합니다.
        /// </summary>
        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && EventManager.GameStatus == EGameState.Playing)
            {
                SaveGameState();
            }
        }

        /// <summary>
        /// 앱이 종료될 때 호출됩니다.
        /// 게임 중이라면 상태를 저장합니다.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            if (EventManager.GameStatus == EGameState.Playing)
            {
                SaveGameState();
            }
        }

        /// <summary>
        /// 점수를 획득했을 때 호출되는 함수입니다.
        /// 점수를 더하고 UI 애니메이션을 시작합니다.
        /// </summary>
        /// <param name="scoreToAdd">추가할 점수</param>
        public virtual void OnScored(int scoreToAdd)
        {
            int previousScore = this.score;
            this.score += scoreToAdd;

            // UI 텍스트를 즉시 업데이트 (최종값 보장)
            scoreText.text = score.ToString();

            // 이전 애니메이션이 있다면 중지하고 새로운 애니메이션 시작
            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
            }
            _counterCoroutine = StartCoroutine(CountScore(previousScore, this.score));
        }

        /// <summary>
        /// 점수가 순차적으로 올라가는 연출을 위한 코루틴입니다.
        /// </summary>
        protected IEnumerator CountScore(int startValue, int endValue)
        {
            _displayedScore = startValue;

            // 점수 차이에 따라 카운팅 속도 조절
            float actualSpeed = counterSpeed;
            if (endValue - startValue > 100)
                actualSpeed = counterSpeed * 0.5f;
            else if (endValue - startValue > 500)
                actualSpeed = counterSpeed * 0.2f;

            // 목표 점수까지 1씩 증가시키며 UI 업데이트
            while (_displayedScore < endValue)
            {
                _displayedScore++;
                scoreText.text = _displayedScore.ToString();
                yield return new WaitForSeconds(actualSpeed);
            }

            // 최종 점수 확정
            _displayedScore = endValue;
            scoreText.text = endValue.ToString();
        }

        /// <summary>
        /// 게임에서 패배했을 때 호출됩니다.
        /// 저장된 게임 데이터를 삭제합니다.
        /// </summary>
        public virtual void OnLose()
        {
            DeleteGameState();
        }

        /// <summary>
        /// 점수를 강제로 특정 값으로 업데이트합니다.
        /// </summary>
        public virtual void UpdateScore(int newScore)
        {
            int previousScore = this.score;
            this.score = newScore;
            
            // UI 즉시 업데이트
            scoreText.text = score.ToString();
            
            // 변경 애니메이션 실행
            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
            }
            _counterCoroutine = StartCoroutine(CountScore(previousScore, this.score));
        }

        /// <summary>
        /// 점수를 0으로 초기화합니다.
        /// </summary>
        public virtual void ResetScore()
        {
            // 진행 중인 점수 애니메이션 중지
            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
                _counterCoroutine = null;
            }

            // 현재 점수만 초기화하고 최고 점수는 유지
            score = 0;
            _displayedScore = 0;

            // UI 업데이트
            scoreText.text = "0";

            // 게임 데이터를 삭제하지 않고, 초기화된 점수 상태로 저장
            SaveGameState();
        }

        // 자식 클래스에서 구현해야 할 추상 메서드들
        protected abstract void LoadScores();      // 점수 불러오기
        protected abstract void SaveGameState();   // 게임 상태 저장
        protected abstract void DeleteGameState(); // 게임 데이터 삭제
    }
}