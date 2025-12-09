// // ©2015 - 2025 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using BlockPuzzleGameToolkit.Scripts.Gameplay.Pool;
using DG.Tweening;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    /// <summary>
    /// 플레이어가 드래그해서 사용할 블록(Shape)이 생성되는 대기 장소(덱)를 관리하는 클래스입니다.
    /// 화면 하단에 블록 3개가 나오는 그 공간 하나하나를 의미합니다.
    /// </summary>
    public class CellDeck : MonoBehaviour
    {
        // 현재 이 덱에 할당된 블록(도형) 객체
        public Shape shape;
        // 블록이 생성될 때 재생할 이펙트 프리팹
        public GameObject prefabFX;

        [SerializeField]
        // 블록을 놓을 수 있는지 확인하기 위한 필드 매니저 참조
        private FieldManager field;

        // 덱이 비어있는지 확인하는 프로퍼티
        public bool IsEmpty => shape == null;

        /// <summary>
        /// 매 프레임마다 호출되어 블록의 상태를 업데이트합니다.
        /// 현재 블록을 보드에 놓을 수 있는지 검사하고, 불가능하면 반투명하게 표시합니다.
        /// </summary>
        private void Update()
        {
            if (shape != null)
            {
                if (field != null)
                {
                    // 블록을 놓을 공간이 있으면 불투명(정상)하게 표시
                    if (field.CanPlaceShape(shape))
                    {
                        SetShapeTransparency(shape, 1.0f); // 완전 불투명
                    }
                    // 놓을 공간이 없으면 반투명(비활성)하게 표시 (게임 오버 임박 알림)
                    else
                    {
                        SetShapeTransparency(shape, 0.1f); // 반투명
                    }
                }
            }
        }

        /// <summary>
        /// 덱에 새로운 블록을 채워 넣습니다.
        /// </summary>
        /// <param name="randomShape">생성된 랜덤 블록</param>
        public void FillCell(Shape randomShape)
        {
            shape = randomShape;
            if (shape != null)
            {
                // 블록을 덱의 자식으로 설정하고 위치 초기화
                shape.transform.SetParent(transform);
                shape.transform.localPosition = Vector3.zero;
                
                // 덱에 있을 때는 크기를 0.5배로 줄여서 표시
                shape.transform.localScale = Vector3.one * 0.5f;
                var scale = shape.transform.localScale;
                
                // 등장 애니메이션을 위해 크기를 0으로 시작
                shape.transform.localScale = Vector3.zero;
                
                // 생성 이펙트 재생
                PoolObject.GetObject(prefabFX, shape.transform.position);
                
                // 팝업되는 애니메이션 실행 (0 -> 0.5)
                shape.transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { shape.transform.localScale = scale; });
            }
        }

        /// <summary>
        /// 블록의 투명도를 설정하는 헬퍼 함수입니다.
        /// 블록을 구성하는 모든 아이템(작은 사각형들)의 투명도를 조절합니다.
        /// </summary>
        private void SetShapeTransparency(Shape shape, float alpha)
        {
            foreach (var item in shape.GetActiveItems())
            {
                item.SetTransparency(alpha);
            }
        }

        /// <summary>
        /// 덱을 비웁니다. (블록을 사용했거나 초기화할 때)
        /// 블록 객체를 오브젝트 풀로 반환합니다.
        /// </summary>
        public void ClearCell()
        {
            if (shape != null)
            {
                PoolObject.Return(shape.gameObject);
                shape = null;
            }
        }

        /// <summary>
        /// 블록을 완전히 제거합니다. (파괴)
        /// </summary>
        public void RemoveShape()
        {
            if (shape != null)
            {
                Destroy(shape.gameObject);
                shape = null;
            }
        }
    }
}