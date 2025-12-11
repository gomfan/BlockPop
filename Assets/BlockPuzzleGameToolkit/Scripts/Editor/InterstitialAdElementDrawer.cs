using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using BlockPuzzleGameToolkit.Scripts.Settings;
using BlockPuzzleGameToolkit.Scripts.Popups;
using System.Linq;
using System.Collections.Generic;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    /// <summary>
    /// 전면 광고(Interstitial Ad) 설정 요소(InterstitialAdElement)를 인스펙터에서 편하게 편집하기 위한 커스텀 드로어입니다.
    /// 광고 참조, 트리거 팝업 선택, 레벨 조건 등을 직관적인 UI로 제공합니다.
    /// </summary>
    [CustomPropertyDrawer(typeof(InterstitialAdElement))]
    public class InterstitialAdElementDrawer : PropertyDrawer
    {
        private Popup[] popupPrefabs;
        private List<string> popupNames;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // 프로젝트 내의 모든 팝업 프리팹을 로드합니다.
            LoadPopupPrefabs();

            var container = new VisualElement();
            
            // 각 프로퍼티 찾기
            var adReferenceProperty = property.FindPropertyRelative("adReference");
            var elementNameProperty = property.FindPropertyRelative("elementName");
            var popupProperty = property.FindPropertyRelative("popup");
            var showOnOpenProperty = property.FindPropertyRelative("showOnOpen");
            var showOnCloseProperty = property.FindPropertyRelative("showOnClose");
            var minLevelProperty = property.FindPropertyRelative("minLevel");
            var maxLevelProperty = property.FindPropertyRelative("maxLevel");
            var frequencyProperty = property.FindPropertyRelative("frequency");

            // 광고 참조에 따라 요소 이름 자동 업데이트
            UpdateElementName(adReferenceProperty, elementNameProperty);

            // 광고 참조 필드 (값 변경 시 이름 업데이트 콜백 등록)
            var adReferenceField = new PropertyField(adReferenceProperty);
            adReferenceField.RegisterValueChangeCallback(evt =>
            {
                UpdateElementName(adReferenceProperty, elementNameProperty);
                property.serializedObject.ApplyModifiedProperties();
            });
            container.Add(adReferenceField);

            // 팝업 선택 드롭다운 (Popup 컴포넌트가 있는 프리팹 목록 표시)
            var popupDropdown = new DropdownField("Popup", popupNames, GetPopupIndex(popupProperty.objectReferenceValue as Popup));
            popupDropdown.RegisterValueChangedCallback(evt =>
            {
                int selectedIndex = popupNames.IndexOf(evt.newValue);
                if (selectedIndex == 0) // "None" 선택 시
                {
                    popupProperty.objectReferenceValue = null;
                }
                else if (selectedIndex > 0)
                {
                    popupProperty.objectReferenceValue = popupPrefabs[selectedIndex - 1];
                }
                popupProperty.serializedObject.ApplyModifiedProperties();
            });
            container.Add(popupDropdown);

            // 표시 옵션 필드
            var showOnOpenField = new PropertyField(showOnOpenProperty);
            container.Add(showOnOpenField);

            var showOnCloseField = new PropertyField(showOnCloseProperty);
            container.Add(showOnCloseField);

            // 레벨 조건 헤더
            var levelHeader = new Label("Level Conditions");
            levelHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            levelHeader.style.marginTop = 5;
            container.Add(levelHeader);

            // 레벨 조건 및 빈도 필드
            var minLevelField = new PropertyField(minLevelProperty);
            container.Add(minLevelField);

            var maxLevelField = new PropertyField(maxLevelProperty);
            container.Add(maxLevelField);

            var frequencyField = new PropertyField(frequencyProperty);
            container.Add(frequencyField);

            return container;
        }

        // 프로젝트 내의 모든 'Popup' 컴포넌트를 가진 프리팹을 검색하여 목록을 만듭니다.
        private void LoadPopupPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            var popups = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path))
                .Where(go => go != null && go.GetComponent<Popup>() != null)
                .Select(go => go.GetComponent<Popup>())
                .OrderBy(popup => popup.name)
                .ToArray();

            popupPrefabs = popups;
            popupNames = new List<string> { "None (Popup)" };
            popupNames.AddRange(popups.Select(popup => popup.name));
        }

        private int GetPopupIndex(Popup popup)
        {
            if (popup == null) return 0;
            
            for (int i = 0; i < popupPrefabs.Length; i++)
            {
                if (popupPrefabs[i] == popup)
                    return i + 1;
            }
            return 0;
        }

        // 광고 참조 객체의 이름을 요소 이름으로 자동 설정합니다.
        private void UpdateElementName(SerializedProperty adReferenceProperty, SerializedProperty elementNameProperty)
        {
            if (adReferenceProperty.objectReferenceValue != null)
            {
                string adRefName = adReferenceProperty.objectReferenceValue.name;
                elementNameProperty.stringValue = adRefName;
            }
            else
            {
                elementNameProperty.stringValue = "Unnamed";
            }
        }
    }
}