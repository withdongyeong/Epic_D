using Game.Scripts.Characters.Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    /// <summary>
    /// 적 머리 위에 체력바를 표시하는 클래스
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        public Slider healthSlider;       // 체력 표시 슬라이더
        public float yOffset = 1.5f;      // 머리 위 오프셋 위치

        private BaseEnemy _enemy;         // 연결된 적 참조
        private Camera _mainCamera;       // 메인 카메라 참조
        private Canvas _canvas;           // 체력바 캔버스

        /// <summary>
        /// 초기화 및 컴포넌트 참조 설정
        /// </summary>
        private void Start()
        {
            _enemy = GetComponentInParent<BaseEnemy>();
            _mainCamera = Camera.main;
            _canvas = GetComponentInParent<Canvas>();

            if (_canvas == null)
            {
                // 캔버스가 없으면 새로 생성
                GameObject canvasObj = new GameObject("EnemyHealthCanvas");
                canvasObj.transform.SetParent(transform.parent);
                _canvas = canvasObj.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.WorldSpace;

                // 캔버스 스케일러 추가
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 100f;

                // 체력바를 캔버스 하위로 이동
                transform.SetParent(_canvas.transform);
            }

            // 초기 체력 설정
            if (_enemy != null && healthSlider != null)
            {
                healthSlider.maxValue = _enemy.maxHealth;
                healthSlider.value = _enemy.maxHealth;
            }
        }

        /// <summary>
        /// 매 프레임 위치 및 체력 업데이트
        /// </summary>
        private void Update()
        {
            if (_enemy == null)
                return;

            // 체력바 위치 업데이트 (적 머리 위)
            UpdatePosition();

            // 체력바 값 업데이트
            healthSlider.value = _enemy.CurrentHealth;
        }

        /// <summary>
        /// 적 위치에 따라 체력바 위치 업데이트
        /// </summary>
        private void UpdatePosition()
        {
            if (_enemy != null)
            {
                Vector3 enemyPosition = _enemy.transform.position;
                transform.position = new Vector3(enemyPosition.x, enemyPosition.y + yOffset, enemyPosition.z);

                // 카메라가 있으면 항상 카메라를 향하도록 설정
                if (_mainCamera != null)
                {
                    transform.forward = _mainCamera.transform.forward;
                }
            }
        }
    }
}