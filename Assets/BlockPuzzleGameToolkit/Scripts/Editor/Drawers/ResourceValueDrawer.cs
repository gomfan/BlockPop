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

using BlockPuzzleGameToolkit.Scripts.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    /// <summary>
    /// ResourceValue 타입의 필드를 인스펙터에 그릴 때 사용하는 커스텀 드로어입니다.
    /// 실제 저장된 값(PlayerPrefs)을 로드하여 라벨로 표시해줍니다.
    /// 이로 인해 에디터 상에서 현재 저장된 리소스 값을 바로 확인할 수 있습니다.
    /// </summary>
    [CustomPropertyDrawer(typeof(ResourceValue))]
    public class ResourceValueDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            // 프로퍼티가 속한 ResourceObject에서 LoadResource()를 호출하여 현재 값을 가져와 표시
            root.Add(new Label("Resource Value: " + ((ResourceObject)property.serializedObject.targetObject).LoadResource()));

            return root;
        }
    }
}