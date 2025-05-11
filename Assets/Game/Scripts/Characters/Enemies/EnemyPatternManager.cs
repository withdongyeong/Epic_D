using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Characters.Player;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 적 공격 패턴 관리 클래스
    /// </summary>
    public class EnemyPatternManager : MonoBehaviour
    {
        private EnemyAttackManager _attackManager;
        
        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize(EnemyAttackManager attackManager)
        {
            _attackManager = attackManager;
        }
        
        /// <summary>
        /// 랜덤 패턴 실행
        /// </summary>
        public void ExecuteRandomPattern()
        {
            switch (Random.Range(0, 8)) // 8개 패턴 (메테오 추가)
            {
                case 0:
                    StartCoroutine(AreaAttack());
                    break;
                case 1:
                    StartCoroutine(HalfGridAttack());
                    break;
                case 2:
                    StartCoroutine(RapidFirePattern());
                    break;
                case 3:
                    StartCoroutine(CrossAttackPattern());
                    break;
                case 4:
                    StartCoroutine(MultiAreaAttack());
                    break;
                case 5:
                    StartCoroutine(DiagonalAttackPattern());
                    break;
                case 6:
                    StartCoroutine(DiagonalCrossPattern());
                    break;
                case 7:
                    StartCoroutine(_attackManager.ExecuteMeteorAttack(Vector3.zero));
                    break;
            }
        }
        
        /// <summary>
        /// 패턴 1: 플레이어 위치 범위 공격
        /// </summary>
        /// <summary>
        /// 패턴 1: 플레이어 위치 범위 공격
        /// </summary>
        private IEnumerator AreaAttack()
        {
            // 그리드 시스템과 플레이어 위치 참조 얻기
            GridSystem gridSystem = FindAnyObjectByType<GridSystem>();
            PlayerController player = FindAnyObjectByType<PlayerController>();
    
            // 플레이어 위치 가져오기
            int playerX, playerY;
            gridSystem.GetXY(player.transform.position, out playerX, out playerY);
    
            // 경고 타일 표시 (3x3 영역)
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> attackPositions = new List<Vector3>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int tileX = playerX + x;
                    int tileY = playerY + y;
    
                    if (gridSystem.IsValidPosition(tileX, tileY))
                    {
                        Vector3 tilePos = gridSystem.GetWorldPosition(tileX, tileY);
                        attackPositions.Add(tilePos);
                        warningTiles.Add(_attackManager.CreateWarningTile(tilePos));
                    }
                }
            }
    
            // 경고 대기
            yield return new WaitForSeconds(1.5f);
    
            // 플레이어가 영역 내에 있는지 확인
            gridSystem.GetXY(player.transform.position, out int currentX, out int currentY);
            if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
            {
                _attackManager.ApplyDamageWithEffect(15);
            }
    
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                _attackManager.CreateDamageEffect(pos);
            }
    
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
        }
        
        /// <summary>
        /// 패턴 2: 그리드 절반 공격
        /// </summary>
        private IEnumerator HalfGridAttack()
        {
            // 여기에 구현
            yield return null;
        }
        
        /// <summary>
        /// 패턴 3: 연속 투사체 발사
        /// </summary>
        private IEnumerator RapidFirePattern()
        {
            // 여기에 구현
            yield return null;
        }
        
        /// <summary>
        /// 패턴 4: 십자 공격
        /// </summary>
        private IEnumerator CrossAttackPattern()
        {
            // 여기에 구현
            yield return null;
        }
        
        /// <summary>
        /// 패턴 5: 연속 영역 공격
        /// </summary>
        private IEnumerator MultiAreaAttack()
        {
            // 여기에 구현
            yield return null;
        }
        
        /// <summary>
        /// 패턴 6: 대각선 공격
        /// </summary>
        private IEnumerator DiagonalAttackPattern()
        {
            // 여기에 구현
            yield return null;
        }
        
        /// <summary>
        /// 패턴 7: 대각선 후 십자 공격 패턴
        /// </summary>
        private IEnumerator DiagonalCrossPattern()
        {
            // 여기에 구현
            yield return null;
        }
    }
}