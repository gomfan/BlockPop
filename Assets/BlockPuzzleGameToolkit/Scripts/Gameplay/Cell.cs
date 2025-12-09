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

using System.Collections;
using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    /// <summary>
    /// 게임 보드(격자)의 한 칸을 나타내는 클래스입니다.
    /// 블록(Item)이 놓일 수 있는 공간이며, 블록의 상태(비어있음, 채워짐, 파괴됨 등)를 관리합니다.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        // 커스텀 아이템(특수 블록 등)을 재사용하기 위한 오브젝트 풀 딕셔너리
        private static readonly Dictionary<string, ObjectPool<Item>> CustomItemPools = new();
        
        // 현재 이 셀에 들어있는 아이템(블록) 참조
        public Item item;
        // 아이템의 투명도 등을 조절하기 위한 CanvasGroup
        private CanvasGroup group;
        // 셀이 현재 사용 중인지(블록이 채워져 있는지) 여부
        public bool busy;
        // 임시로 저장된 아이템 템플릿 (상태 복구용)
        private ItemTemplate saveTemplate;
        // 충돌 감지용 콜라이더
        private BoxCollider2D _boxCollider2D;
        // 셀이 파괴되는 중인지 여부
        private bool isDestroying;
        // 원래 가지고 있던 기본 아이템 (커스텀 아이템으로 교체 시 백업용)
        private Item originalItem;
        // 현재 적용된 커스텀 아이템 (특수 블록 등)
        private Item customItem;

        // 셀 배경 이미지
        public Image image;

        // 셀이 비어있는지 확인하는 프로퍼티 (busy가 false면 비어있음)
        private bool isEmpty => !busy;
        // 미리보기 상태에서 비어있는지 확인 (투명도가 0이면 비어있음)
        private bool IsEmptyPreview => group.alpha == 0;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            group = item.GetComponent<CanvasGroup>();
            CustomItemPools.Clear();
        }

        /// <summary>
        /// 아이템 프리팹에 대한 오브젝트 풀을 가져오거나 생성합니다.
        /// </summary>
        /// <param name="prefab">풀을 생성하거나 가져올 아이템 프리팹</param>
        /// <returns>해당 아이템 프리팹에 대한 오브젝트 풀</returns>
        private ObjectPool<Item> GetOrCreatePool(Item prefab)
        {
            if (!CustomItemPools.TryGetValue(prefab.name, out var pool))
            {
                pool = new ObjectPool<Item>(
                    createFunc: () =>
                    {
                        var instantiate = Instantiate(prefab,transform);
                        instantiate.name = prefab.name;
                        return instantiate;
                    },
                    actionOnGet: item =>
                    {
                        if (GetValue(prefab, item, pool))
                        {
                            return;
                        }
                        item.transform.SetParent(transform);
                        item.gameObject.SetActive(true);
                    },
                    actionOnRelease: item =>
                    {
                        if (GetValue(prefab, item, pool))
                        {
                            return;
                        }
                        if (item?.gameObject != null)
                            item.gameObject.SetActive(false);
                    },
                    actionOnDestroy: item =>
                    {
                        if (GetValue(prefab, item, pool))
                        {
                            return;
                        }
                        if (item?.gameObject != null)
                            Destroy(item.gameObject);
                    }
                );
                CustomItemPools[prefab.name] = pool;
            }
            return pool;
        }

        /// <summary>
        /// 풀에서 아이템을 가져오거나 반환할 때 유효성 검사를 수행합니다.
        /// 아이템이 null인 경우 풀을 정리하고 셀을 비웁니다.
        /// </summary>
        /// <param name="prefab">아이템의 원본 프리팹</param>
        /// <param name="item">현재 처리 중인 아이템 인스턴스</param>
        /// <param name="pool">해당 아이템의 오브젝트 풀</param>
        /// <returns>아이템이 null이어서 풀이 정리되었으면 true, 아니면 false</returns>
        private bool GetValue(Item prefab, Item item, ObjectPool<Item> pool)
        {
            if (item == null)
            {
                pool.Clear();
                CustomItemPools.Remove(prefab.name);
                ClearCell();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 현재 아이템을 커스텀 아이템(특수 블록 등)으로 교체합니다.
        /// 기존 아이템은 백업하고, 새 커스텀 아이템을 풀에서 가져와 설정합니다.
        /// </summary>
        /// <param name="itemTemplate">교체할 커스텀 아이템의 템플릿</param>
        private void ReplaceWithCustomItem(ItemTemplate itemTemplate)
        {
            // 원본 아이템 백업
            if (originalItem == null)
            {
                originalItem = item;
                originalItem.gameObject.SetActive(false);
            }

            // 기존 커스텀 아이템 반환
            if (customItem != null)
            {
                GetOrCreatePool(itemTemplate.customItemPrefab).Release(customItem);
            }
            
            // 새 커스텀 아이템 가져오기
            customItem = GetOrCreatePool(itemTemplate.customItemPrefab).Get();
            customItem.transform.SetParent(transform);
            customItem.transform.position = originalItem.transform.position;
            customItem.transform.localScale = originalItem.transform.localScale;
            
            // RectTransform 속성 복사
            var rectTransform = customItem.GetComponent<RectTransform>();
            var originalRect = originalItem.GetComponent<RectTransform>();
            if (rectTransform != null && originalRect != null)
            {
                rectTransform.anchorMin = originalRect.anchorMin;
                rectTransform.anchorMax = originalRect.anchorMax;
                rectTransform.pivot = originalRect.pivot;
                rectTransform.sizeDelta = originalRect.sizeDelta;
                rectTransform.anchoredPosition = originalRect.anchoredPosition;
            }
            item = customItem;
            group = item.GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 셀에 아이템을 채웁니다.
        /// 커스텀 아이템이 지정된 경우 해당 아이템으로 교체하고, 그렇지 않으면 기본 아이템을 사용합니다.
        /// </summary>
        /// <param name="itemTemplate">셀에 채울 아이템의 템플릿</param>
        public void FillCell(ItemTemplate itemTemplate)
        {
            if (itemTemplate.customItemPrefab != null)
            {
                ReplaceWithCustomItem(itemTemplate);
            }
            else 
            {
                // 커스텀 아이템이 아닌 경우 원본 아이템으로 복구
                if (originalItem != null)
                {
                    if (customItem != null)
                    {
                        Destroy(customItem.gameObject);
                        customItem = null;
                    }
                    item = originalItem;
                    item.gameObject.SetActive(true);
                    originalItem = null;
                    group = item.GetComponent<CanvasGroup>();
                }
            }

            item.FillIcon(itemTemplate);
            group.alpha = 1;
            busy = true;
        }

        /// <summary>
        /// 셀 채우기 실패 시 호출 (시각적 처리만 수행).
        /// 아이템 아이콘을 채우고 투명도를 1로 설정합니다.
        /// </summary>
        /// <param name="itemTemplate">채우려던 아이템의 템플릿</param>
        public void FillCellFailed(ItemTemplate itemTemplate)
        {
            item.FillIcon(itemTemplate);
            group.alpha = 1;
        }

        /// <summary>
        /// 셀이 비어있는지 확인합니다.
        /// </summary>
        /// <param name="preview">미리보기 모드인지 여부. true면 투명도와 파괴 중 상태를 고려합니다.</param>
        /// <returns>셀이 비어있으면 true, 아니면 false</returns>
        public bool IsEmpty(bool preview = false)
        {
            return preview ? IsEmptyPreview || isDestroying: isEmpty;
        }

        /// <summary>
        /// 셀을 비웁니다. (블록 제거)
        /// 커스텀 아이템이 있으면 풀에 반환하고, 원본 아이템으로 복구합니다.
        /// 저장된 템플릿이 있으면 해당 템플릿으로 다시 채웁니다.
        /// </summary>
        public void ClearCell()
        {
            // 커스텀 아이템 정리
            if (customItem != null)
            {
                // customItem.GetComponent<Item>() 대신 customItem 자체를 사용해야 함
                GetOrCreatePool(customItem).Release(customItem);
                customItem = null;
            }
            item.itemTemplate = null;
            
            // 원본 아이템 복구
            if (originalItem != null)
            {
                item = originalItem;
                item.gameObject.SetActive(true);
                originalItem = null;
                group = item.GetComponent<CanvasGroup>();
            }
            
            item.transform.localScale = Vector3.one;
            
            // 저장된 템플릿이 없으면 완전히 비움
            if (saveTemplate == null && !busy)
            {
                group.alpha = 0;
                busy = false;
            }
            // 저장된 템플릿이 있으면 복구 (예: 하이라이트 후 원래 상태로)
            else if (saveTemplate != null && busy)
            {
                FillCell(saveTemplate);
                saveTemplate = null;
            }
        }

        /// <summary>
        /// 블록을 놓을 수 있는지 미리보기(하이라이트)를 표시합니다.
        /// 아이템 템플릿에 따라 커스텀 아이템으로 교체하거나 기본 아이템을 사용하며, 반투명하게 표시합니다.
        /// </summary>
        /// <param name="itemTemplate">하이라이트할 아이템의 템플릿</param>
        public void HighlightCell(ItemTemplate itemTemplate)
        {
            if (itemTemplate.customItemPrefab != null)
            {
                ReplaceWithCustomItem(itemTemplate);
            }
            else 
            {
                if (originalItem != null)
                {
                    if (customItem != null)
                    {
                        Destroy(customItem.gameObject);
                        customItem = null;
                    }
                    item = originalItem;
                    item.gameObject.SetActive(true);
                    originalItem = null;
                    group = item.GetComponent<CanvasGroup>();
                }
            }

            item.FillIcon(itemTemplate);
            group.alpha = 0.05f; // 반투명하게 표시하여 하이라이트 효과
        }

        /// <summary>
        /// 튜토리얼용 하이라이트 (셀 배경 색상 변경)
        /// </summary>
        public void HighlightCellTutorial()
        {
            image.color = new Color(43f / 255f, 59f / 255f, 120f / 255f, 1f);
        }

        /// <summary>
        /// 이미 채워진 셀에 대한 하이라이트 처리 (보너스 아이템 등).
        /// 현재 아이템 템플릿을 저장하고, 보너스 아이템이 없으면 새 템플릿으로 아이콘을 채웁니다.
        /// 투명도를 1로 설정합니다.
        /// </summary>
        /// <param name="itemTemplate">하이라이트할 아이템의 템플릿</param>
        public void HighlightCellFill(ItemTemplate itemTemplate)
        {
            saveTemplate = item.itemTemplate;
            if (!item.HasBonusItem())
            {
                item.FillIcon(itemTemplate);
            }

            group.alpha = 1f;
        }

        /// <summary>
        /// 셀의 블록을 파괴합니다. (애니메이션 포함)
        /// 블록의 크기를 0으로 줄이는 애니메이션 후 셀을 비우고 보너스 아이템을 제거합니다.
        /// </summary>
        public void DestroyCell()
        {
            saveTemplate = null;
            busy = false;
            // 크기가 줄어드는 애니메이션 후 셀 비우기
            item.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                isDestroying = false;
                ClearCell();
                item.ClearBonus();
            });
        }

        /// <summary>
        /// 셀의 경계 영역을 반환합니다.
        /// </summary>
        /// <returns>셀의 BoxCollider2D 경계</returns>
        public Bounds GetBounds()
        {
            return _boxCollider2D.bounds;
        }

        /// <summary>
        /// 아이템 초기화. 이름 설정 및 위치/크기 업데이트 코루틴 시작.
        /// </summary>
        public void InitItem()
        {
            item.name = "Item " + name;
            StartCoroutine(UpdateItem());
        }

        /// <summary>
        /// 아이템 위치 및 크기 업데이트 코루틴.
        /// 짧은 지연 후 BoxCollider2D의 크기를 RectTransform의 sizeDelta에 맞추고 아이템의 위치를 셀의 위치로 설정합니다.
        /// </summary>
        private IEnumerator UpdateItem()
        {
            yield return new WaitForSeconds(0.1f);
            _boxCollider2D.size = transform.GetComponent<RectTransform>().sizeDelta;
            // item.transform.SetParent(GameObject.Find("ItemsCanvas/Items").transform);
            item.transform.position = transform.position;
        }

        /// <summary>
        /// 셀의 아이템에 보너스를 설정합니다.
        /// </summary>
        /// <param name="bonusItemTemplate">설정할 보너스 아이템 템플릿</param>
        public void SetBonus(BonusItemTemplate bonusItemTemplate)
        {
            item.SetBonus(bonusItemTemplate);
        }

        /// <summary>
        /// 셀의 아이템이 보너스 아이템을 가지고 있는지 확인합니다.
        /// </summary>
        /// <returns>보너스 아이템이 있으면 true, 아니면 false</returns>
        public bool HasBonusItem()
        {
            return item.HasBonusItem();
        }

        /// <summary>
        /// 셀의 아이템이 가지고 있는 보너스 아이템 정보를 가져옵니다.
        /// </summary>
        /// <returns>보너스 아이템 템플릿</returns>
        public BonusItemTemplate GetBonusItem()
        {
            return item.bonusItemTemplate;
        }

        /// <summary>
        /// 채워질 때의 애니메이션 효과를 재생합니다.
        /// 아이템의 크기를 줄였다가 다시 원래대로 키우는 애니메이션입니다.
        /// </summary>
        public void AnimateFill()
        {
            item.transform.DOScale(Vector3.one * 0.5f, 0.1f).OnComplete(() => { item.transform.DOScale(Vector3.one, 0.1f); });
        }

        /// <summary>
        /// 셀을 비활성화합니다. (클릭/상호작용 불가)
        /// BoxCollider2D를 비활성화하여 상호작용을 막습니다.
        /// </summary>
        public void DisableCell()
        {
            _boxCollider2D.enabled = false;
        }

        /// <summary>
        /// 셀이 비활성화 상태인지 확인합니다.
        /// </summary>
        /// <returns>셀이 비활성화 상태이면 true, 아니면 false</returns>
        public bool IsDisabled()
        {
            return !_boxCollider2D.enabled;
        }

        /// <summary>
        /// 셀이 하이라이트 가능한 상태인지(활성화 상태인지) 확인합니다.
        /// </summary>
        /// <returns>셀이 활성화 상태이면 true, 아니면 false</returns>
        public bool IsHighlighted()
        {
            return !IsDisabled();
        }

        /// <summary>
        /// 셀의 파괴 중 상태를 설정합니다.
        /// </summary>
        /// <param name="destroying">파괴 중이면 true, 아니면 false</param>
        public void SetDestroying(bool destroying)
        {
            isDestroying = destroying;
        }

        /// <summary>
        /// 셀이 파괴 중인지 확인합니다.
        /// </summary>
        /// <returns>셀이 파괴 중이면 true, 아니면 false</returns>
        public bool IsDestroying()
        {
            return isDestroying;
        }

        /// <summary>
        /// 객체 파괴 시 호출됩니다.
        /// 커스텀 아이템이 남아있으면 오브젝트 풀에 반환하여 메모리 누수를 방지합니다.
        /// </summary>
        private void OnDestroy()
        {
            if (customItem != null)
            {
                // customItem.GetComponent<Item>() 대신 customItem 자체를 사용해야 함
                GetOrCreatePool(customItem).Release(customItem);
            }
        }
    }
}