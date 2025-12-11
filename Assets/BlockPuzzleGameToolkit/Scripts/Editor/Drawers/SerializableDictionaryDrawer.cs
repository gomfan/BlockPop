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

using System;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    /// <summary>
    /// SerializableDictionary<TKey, TValue> 타입을 인스펙터에 그리기 위한 커스텀 프로퍼티 드로어입니다.
    /// UIElements를 사용하여 효율적인 리스트 뷰 형태로 딕셔너리를 표시하고 편집할 수 있게 해줍니다.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var foldout = new Foldout();
            foldout.text = property.displayName;
            container.Add(foldout);

            var listView = new ListView();
            foldout.Add(listView);
            var dictionaryType = fieldInfo.FieldType;
            // 제네릭 인수에서 키 타입을 가져옵니다.
            var keyType = dictionaryType.GetGenericArguments()[0];
            
            // 각 항목을 그릴 VisualElement 생성 (키와 값을 나란히 표시)
            listView.makeItem = () => new PackElementDictionaryItemElement(keyType);
            
            // 데이터 바인딩: 각 항목의 키와 값 프로퍼티를 UI 요소에 연결
            listView.bindItem = (element, index) =>
            {
                var itemElement = (PackElementDictionaryItemElement)element;
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");
                itemElement.BindProperties(keysProp.GetArrayElementAtIndex(index), valuesProp.GetArrayElementAtIndex(index), property, index);
            };

            listView.showAddRemoveFooter = true;
            listView.showFoldoutHeader = false;
            listView.showBorder = true;
            listView.showBoundCollectionSize = false;
            listView.reorderable = true;

            void RefreshList()
            {
                var keysProp = property.FindPropertyRelative("keys");
                listView.itemsSource = new List<int>(Enumerable.Range(0, keysProp.arraySize));
                listView.RefreshItems();
            }

            RefreshList();

            // 항목 추가 시 호출되는 콜백
            listView.itemsAdded += indexes =>
            {
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");

                property.serializedObject.Update();
                foreach (var index in indexes)
                {
                    // 키와 값 배열에 각각 항목 추가
                    keysProp.InsertArrayElementAtIndex(index);
                    valuesProp.InsertArrayElementAtIndex(index);

                    // 새로 추가된 키 초기화 (객체 참조 null 등)
                    var keyProp = keysProp.GetArrayElementAtIndex(index);
                    keyProp.objectReferenceValue = null;

                    // 새로 추가된 값 초기화 (타입에 맞게 기본값 설정)
                    var valueProp = valuesProp.GetArrayElementAtIndex(index);
                    ResetValue(valueProp);
                }

                property.serializedObject.ApplyModifiedProperties();

                RefreshList();
            };

            listView.itemsRemoved += indexes =>
            {
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");

                property.serializedObject.Update();
                foreach (var index in indexes.OrderByDescending(i => i))
                {
                    keysProp.DeleteArrayElementAtIndex(index);
                    valuesProp.DeleteArrayElementAtIndex(index);
                }

                property.serializedObject.ApplyModifiedProperties();

                RefreshList();
            };

            return container;
        }

        /// <summary>
        /// 프로퍼티 타입에 따라 값을 초기화합니다.
        /// </summary>
        private void ResetValue(SerializedProperty valueProp)
        {
            switch (valueProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    valueProp.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    valueProp.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    valueProp.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    valueProp.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.ObjectReference:
                    valueProp.objectReferenceValue = null;
                    break;
                // Add more cases as needed for other types
            }
        }
    }

    /// <summary>
    /// 딕셔너리의 한 항목(Key-Value 쌍)을 표시하기 위한 커스텀 VisualElement입니다.
    /// 왼쪽에는 키, 오른쪽에는 값을 표시합니다.
    /// </summary>
    public class PackElementDictionaryItemElement : VisualElement
    {
        private readonly ObjectField keyField;
        private readonly PropertyField valueField;

        public PackElementDictionaryItemElement(Type keyType)
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
            style.width = Length.Percent(100);

            keyField = new ObjectField
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    marginRight = 5,
                    marginLeft = 5
                }
            };
            keyField.objectType = keyType;
            Add(keyField);

            valueField = new PropertyField
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    marginRight = 5
                }
            };
            Add(valueField);
        }

        public void BindProperties(SerializedProperty keyProp, SerializedProperty valueProp, SerializedProperty parentProp, int index)
        {
            keyField.BindProperty(keyProp);
            valueField.BindProperty(valueProp);

            keyField.label = string.Empty;
            valueField.label = string.Empty;
        }
    }
}