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

using BlockPuzzleGameToolkit.Scripts.Attributes;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEditor;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    // Custom attribute

    // 커스텀 속성(Attribute)에 대한 드로어 정의

    /// <summary>
    /// [IconPreview] 속성이 붙은 필드에 대해 인스펙터 상에서 아이콘 미리보기를 그려주는 클래스입니다.
    /// ScriptableData 객체의 프리팹을 기반으로 미리보기 이미지를 생성하여 표시합니다.
    /// </summary>
    [CustomPropertyDrawer(typeof(IconPreviewAttribute))]
    public class IconDrawer : PropertyDrawer
    {
        private Label m_Icon;
        private ScriptableData m_IconScriptable;
        private SerializedProperty m_property;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_property = property;
            m_Icon = new Label();
            m_Icon.style.width = 200;
            m_Icon.style.height = 200;

            // 프로퍼티가 속한 부모 객체(ScriptableData)를 가져옵니다.
            m_IconScriptable = property.serializedObject.targetObject as ScriptableData;
            if (m_IconScriptable != null)
            {
                // 데이터 변경 시 미리보기를 갱신하도록 이벤트 등록
                m_IconScriptable.OnChange += UpdatePreview;
            }

            UpdatePreview();
            return m_Icon;
        }

        private void UpdatePreview()
        {
            // 에디터 지연 호출을 사용하여 렌더링 타이밍 문제 방지
            EditorApplication.delayCall += () => 
            { 
                var itemTemplate = m_IconScriptable as ItemTemplate;
                
                // 커스텀 프리팹이 있는 경우 (예: 아이템) 해당 프리팹의 미리보기 생성
                if (itemTemplate != null && itemTemplate.HasCustomPrefab())
                {
                    m_Icon.style.backgroundImage = EditorUtils.GetPrefabPreview(itemTemplate.customItemPrefab.gameObject);
                }
                else
                {
                    // 일반적인 경우 캔버스 기반 미리보기 생성 (색상 채우기 등 적용)
                    m_Icon.style.backgroundImage = EditorUtils.GetCanvasPreviewVisualElement(m_IconScriptable.prefab, obj => obj.FillIcon(m_IconScriptable));
                }
            };
        }
    }
}