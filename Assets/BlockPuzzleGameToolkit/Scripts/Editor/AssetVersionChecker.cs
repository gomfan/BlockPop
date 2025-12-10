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

using UnityEngine;
using UnityEditor;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using System;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    /// <summary>
    /// 에셋 변경 사항을 모니터링하고 특정 조건(커스텀 프리팹 사용 여부)을 검사하는 클래스입니다.
    /// AssetPostprocessor를 상속받아 에셋 가져오기 프로세스에 연결됩니다.
    /// </summary>
    public class AssetVersionChecker : AssetPostprocessor
    {
        /// <summary>
        /// 모든 에셋의 가져오기, 삭제, 이동 작업이 완료된 후 호출됩니다.
        /// ItemTemplate 에셋을 확인하여 커스텀 프리팹을 사용하는 경우 경고를 표시합니다.
        /// </summary>
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // "t:ItemTemplate" 필터를 사용하여 프로젝트 내의 모든 ItemTemplate 타입 에셋의 GUID를 찾습니다.
            var guids = AssetDatabase.FindAssets("t:ItemTemplate");
            foreach (string guid in guids)
            {
                // GUID를 에셋 경로로 변환합니다.
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 해당 경로에서 ItemTemplate 에셋을 로드합니다.
                var itemTemplate = AssetDatabase.LoadAssetAtPath<ItemTemplate>(assetPath);
                
                // ItemTemplate이 존재하고, 커스텀 프리팹을 사용하도록 설정되어 있는지 확인합니다.
                if (itemTemplate != null && itemTemplate.HasCustomPrefab())
                {
                    // 사용자에게 경고 다이얼로그를 표시합니다.
                    // 커스텀 프리팹 대신 스프라이트 교체를 권장하는 메시지입니다.
                    EditorUtility.DisplayDialog(
                        "Warning",
                        $"ItemTemplate '{itemTemplate.name}' uses custom prefab. Please replace sprites instead of using custom prefab.",
                        "OK"
                    );
                    // 문제가 있는 에셋을 에디터에서 선택 상태로 만듭니다.
                    Selection.activeObject = itemTemplate;
                }
            }
        }
    }
}
