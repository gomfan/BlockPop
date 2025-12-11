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
using System.Reflection;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    /// <summary>
    /// 에디터 관련 유틸리티 기능을 제공하는 정적 클래스입니다.
    /// 프리팹 미리보기 생성, 프로퍼티 필드 생성, 리플렉션 헬퍼 등의 기능을 포함합니다.
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// 주어진 프리팹 GameObject의 미리보기 텍스처를 생성합니다.
        /// </summary>
        /// <param name="prefab">미리보기를 생성할 프리팹</param>
        /// <returns>생성된 미리보기 Texture2D</returns>
        public static Texture2D GetPrefabPreview(GameObject prefab)
        {
            var previewRender = new PreviewRenderUtility();
            previewRender.camera.backgroundColor = Color.black;
            previewRender.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRender.camera.cameraType = CameraType.Game;
            previewRender.camera.farClipPlane = 1000f;
            previewRender.camera.nearClipPlane = 0.1f;

            var obj = previewRender.InstantiatePrefabInScene(prefab);
            var rect = obj.GetComponent<RectTransform>().rect;
            previewRender.BeginStaticPreview(new Rect(0.0f, 0.0f, rect.width*1.5f, rect.height*1.5f));
            
            SetupPreviewCanvas(obj, previewRender.camera);
            
            previewRender.Render();
            var texture = previewRender.EndStaticPreview();
            
            previewRender.camera.targetTexture = null;
            previewRender.Cleanup();
            return texture;
        }

        /// <summary>
        /// 미리보기 렌더링을 위한 임시 캔버스를 설정합니다.
        /// </summary>
        /// <param name="obj">캔버스를 추가할 게임 오브젝트</param>
        /// <param name="camera">렌더링할 카메라</param>
        private static void SetupPreviewCanvas(GameObject obj, Camera camera)
        {
            var canvasInstance = obj.AddComponent<Canvas>();
            canvasInstance.renderMode = RenderMode.ScreenSpaceCamera;
            canvasInstance.worldCamera = camera;

            var canvasScaler = obj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var scaleFactorX = Screen.width / canvasScaler.referenceResolution.x;
            var scaleFactorY = Screen.height / canvasScaler.referenceResolution.y;
            canvasScaler.scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY) * 7;
        }

        /// <summary>
        /// FillAndPreview를 상속받는 컴포넌트의 미리보기 VisualElement를 생성하고 텍스처로 반환합니다.
        /// 생성 전 특정 액션을 수행할 수 있습니다.
        /// </summary>
        /// <typeparam name="T">FillAndPreview를 상속받는 컴포넌트 타입</typeparam>
        /// <param name="prefab">프리팹 컴포넌트</param>
        /// <param name="action">프리팹 인스턴스화 후 수행할 액션</param>
        /// <returns>생성된 미리보기 Texture2D</returns>
        public static Texture2D GetCanvasPreviewVisualElement<T>(T prefab, Action<T> action) where T : FillAndPreview
        {
            var previewRender = new PreviewRenderUtility();
            previewRender.camera.backgroundColor = Color.black;
            previewRender.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRender.camera.cameraType = CameraType.Game;
            previewRender.camera.farClipPlane = 1000f;
            previewRender.camera.nearClipPlane = 0.1f;

            var obj = previewRender.InstantiatePrefabInScene(prefab.gameObject);
            action.Invoke(obj.GetComponent<T>());
            var rect = obj.GetComponent<RectTransform>().rect;
            previewRender.BeginStaticPreview(new Rect(0.0f, 0.0f, rect.width, rect.height));
            
            SetupPreviewCanvas(obj, previewRender.camera);
            
            previewRender.Render();
            var texture = previewRender.EndStaticPreview();
            
            previewRender.camera.targetTexture = null;
            previewRender.Cleanup();
            return texture;
        }

        /// <summary>
        /// 타겟 오브젝트에서 특정 값과 일치하는 SerializedProperty를 찾습니다.
        /// (현재 구현에서는 값 비교 로직이 주석 처리되어 있어 첫 번째 프로퍼티를 반환할 수 있음)
        /// </summary>
        /// <param name="targetObject">검색 대상 오브젝트</param>
        /// <returns>찾은 SerializedProperty 또는 null</returns>
        public static SerializedProperty GetPropertyFromValue(Object targetObject)
        {
            var serializedObject = new SerializedObject(targetObject);
            var property = serializedObject.GetIterator();

            // Go through each property in the object
            while (property.Next(true))
            {
                // Skip properties with child properties (e.g., arrays, structs)
                if (property.hasVisibleChildren)
                {
                    continue;
                }

                // Check if the property value matches the desired field value
                // if (fieldValue.Equals(GetFieldValue(targetObject, property.name)))
                {
                    // Create a copy of the property
                    var copiedProperty = property.Copy();
                    // Make sure the serializedObject is up to date
                    copiedProperty.serializedObject.Update();
                    // Apply the modified properties
                    copiedProperty.serializedObject.ApplyModifiedProperties();
                    return copiedProperty;
                }
            }

            return null; // Field value not found
        }

        /// <summary>
        /// 리플렉션을 사용하여 오브젝트의 필드 값을 가져옵니다.
        /// </summary>
        /// <param name="targetObject">대상 오브젝트</param>
        /// <param name="fieldName">필드 이름</param>
        /// <returns>필드 값 (object)</returns>
        private static object GetFieldValue(Object targetObject, string fieldName)
        {
            var field = targetObject.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                return field.GetValue(targetObject);
            }

            Debug.LogError($"Field {fieldName} not found in object {targetObject.GetType().Name}");
            return null;
        }

        /// <summary>
        /// SerializedObject의 모든 필드에 대한 UI PropertyField들을 생성하여 VisualElement에 담아 반환합니다.
        /// "m_Script" 필드는 제외됩니다.
        /// </summary>
        /// <param name="serializedObject">대상 SerializedObject</param>
        /// <param name="onChange">값이 변경될 때 호출될 콜백</param>
        /// <returns>필드들이 포함된 VisualElement</returns>
        public static VisualElement GetObjectFields(SerializedObject serializedObject, Action<SerializedProperty> onChange = null)
        {
            var visualElement = new VisualElement();
            // Iterate through the fields of the Icon class
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                // Exclude the "m_Script" field
                if (iterator.name == "m_Script")
                {
                    continue;
                }

                // Create a PropertyField for each field
                var propertyField = new PropertyField(iterator.Copy());
                propertyField.Bind(serializedObject);
                propertyField.style.flexShrink = 0;
                propertyField.style.flexGrow = 0;
                propertyField.style.width = 400;
                propertyField.RegisterValueChangeCallback(evt => { onChange?.Invoke(evt.changedProperty); });

                visualElement.Add(propertyField);
                enterChildren = false;
            }

            return visualElement;
        }

        /// <summary>
        /// SerializedProperty의 하위 필드들에 대한 UI PropertyField들을 생성하여 반환합니다.
        /// CustomSerializeTypePropertyAttribute가 있는 경우 커스텀 처리를 수행합니다.
        /// </summary>
        /// <param name="property">대상 SerializedProperty</param>
        /// <param name="children">자식 프로퍼티 순회 여부</param>
        /// <param name="onChange">변경 콜백</param>
        /// <returns>생성된 VisualElement</returns>
        public static VisualElement GetPropertyFields(SerializedProperty property, bool children, Action<SerializedProperty> onChange = null)
        {
            var visualElement = new VisualElement();
            var methods = TypeCache.GetMethodsWithAttribute<CustomSerializeTypePropertyAttribute>();
            foreach (var m in methods)
            {
                foreach (var customAttributeData in m.CustomAttributes)
                {
                    foreach (var customAttributeTypedArgument in customAttributeData.ConstructorArguments)
                    {
                        if (property.managedReferenceValue != null && (Type)customAttributeTypedArgument.Value != property.managedReferenceValue.GetType())
                        {
                            continue;
                        }

                        if (m.IsStatic)
                        {
                            return m.Invoke(null, new object[] { property }) as VisualElement;
                        }

                        var instance = Activator.CreateInstance(m.DeclaringType);
                        return m.Invoke(instance, new object[] { property }) as VisualElement;
                    }
                }
            }

            // Iterate through the fields of the Icon class
            var iterator = property.Copy();
            while (iterator.NextVisible(children))
            {
                // Exclude the "m_Script" field
                if (iterator.name == "m_Script")
                {
                    continue;
                }

                if (iterator.depth == property.depth + 1)
                {
                    // Create a PropertyField for each field
                    var propertyField = new PropertyField(iterator.Copy());
                    propertyField.Bind(property.serializedObject);
                    propertyField.style.flexShrink = 0;
                    propertyField.style.flexGrow = 0;
                    propertyField.style.width = 400;
                    propertyField.RegisterValueChangeCallback(evt => { onChange?.Invoke(evt.changedProperty); });

                    visualElement.Add(propertyField);
                }

                children = false;
            }

            return visualElement;
        }

        /// <summary>
        /// SerializeTypePropertyAttribute가 지정된 필드 타입에서 파생된 타입들을 선택할 수 있는 드롭다운을 생성합니다.
        /// 선택 시 해당 타입으로 인스턴스를 새로 생성하여 할당합니다.
        /// </summary>
        /// <param name="property">대상 SerializedProperty</param>
        /// <returns>타입 선택 DropdownField</returns>
        public static DropdownField GetTypesDropdown(SerializedProperty property)
        {
            var fieldInfo = TypeCache.GetFieldsWithAttribute<SerializeTypePropertyAttribute>()[0];
            var typesDerivedFrom = TypeCache.GetTypesDerivedFrom(fieldInfo.FieldType);
            var typeNames = typesDerivedFrom.Select(t => t.Name).ToList();
            var typeNamesFormatted = typesDerivedFrom.Select(t => t.Name.CamelCaseSplit()).ToList();
            var parentTypeName = fieldInfo.Name.CamelCaseSplit();
            // get the index of the current type
            var currentIndex = Mathf.Max(typeNames.IndexOf(property.managedReferenceValue?.GetType().Name), 0);
            var dropdown = new DropdownField(parentTypeName, typeNamesFormatted, typeNamesFormatted[currentIndex]);
            dropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var selectedTypeIndex = typeNamesFormatted.IndexOf(evt.newValue);
                if (selectedTypeIndex >= 0 && selectedTypeIndex < typesDerivedFrom.Count)
                {
                    property.managedReferenceValue = Activator.CreateInstance(typesDerivedFrom[selectedTypeIndex]);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"Selected type index {selectedTypeIndex} is out of range.");
                }
            });
            return dropdown;
        }

        /// <summary>
        /// 주어진 VisualElement 리스트의 평균 월드 위치를 부모 기준 로컬 좌표로 계산하여 반환합니다.
        /// </summary>
        /// <param name="elements">대상 VisualElement 리스트</param>
        /// <param name="parent">기준이 되는 부모 VisualElement</param>
        /// <returns>평균 위치 (Vector2)</returns>
        public static Vector2 GetAbsolutePosition(List<VisualElement> elements, VisualElement parent)
        {
            var position = Vector2.zero;
            foreach (var element in elements)
            {
                position += element.LocalToWorld(element.layout.position);
            }

            return parent.WorldToLocal(position / elements.Count);
        }
    }
}