using BlockPuzzleGameToolkit.Scripts.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    /// <summary>
    /// ShopItem 구조체/클래스를 에디터 인스펙터 상에서 한 줄로 깔끔하게 표시하기 위한 커스텀 드로어입니다.
    /// 상품 ID, 수량, 프리팹 필드를 가로로 배열하여 보여줍니다.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShopItem))]
    public class ShopItemDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // 가로 배치를 위한 컨테이너 생성
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            // 각 내부 프로퍼티 찾기
            var productIDProp = property.FindPropertyRelative("productID");
            var countProp = property.FindPropertyRelative("count");
            var prefabProp = property.FindPropertyRelative("prefab");

            // 상품 ID 필드 설정
            var productIDField = new PropertyField(productIDProp);
            productIDField.label = ""; // 라벨 제거 (공간 절약)
            productIDField.style.flexGrow = 1;
            productIDField.style.marginRight = 2;
            productIDField.tooltip = "Product ID reference (상품 ID)";

            // 수량 필드 설정
            var countField = new PropertyField(countProp);
            countField.label = "";
            countField.style.flexGrow = 1;
            countField.style.marginRight = 2;
            countField.tooltip = "Coin count (코인/아이템 개수)";

            // 프리팹 필드 설정
            var prefabField = new PropertyField(prefabProp);
            prefabField.label = "";
            prefabField.style.flexGrow = 1;
            prefabField.tooltip = "Custom prefab (선택 사항: 커스텀 프리팹)";

            // 프리팹이 비어있다면, 기본 프리팹(defaultPrefab)이 설정되어 있는지 확인하고 자동 할당
            if (prefabProp.objectReferenceValue == null)
            {
                var settingsProp = property.serializedObject.FindProperty("defaultPrefab");
                if (settingsProp != null && settingsProp.objectReferenceValue != null)
                {
                    prefabProp.objectReferenceValue = settingsProp.objectReferenceValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            // 프리팹 값 변경 시 즉시 저장 및 갱신
            prefabField.RegisterValueChangeCallback(evt =>
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            });

            container.Add(productIDField);
            container.Add(countField);
            container.Add(prefabField);

            return container;
        }
    }
}
