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

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    // 에디터가 로드될 때 초기화 작업을 수행하는 클래스입니다.
    // [InitializeOnLoad] 속성은 유니티 에디터가 시작되거나 스크립트가 컴파일된 후 자동으로 이 클래스를 초기화하도록 합니다.
    [InitializeOnLoad]
    public class Autorun
    {
        // 정적 생성자: 에디터가 스크립트를 로드할 때 호출됩니다.
        // 매 프레임 호출되는 EditorApplication.update 이벤트에 InitProject 메서드를 등록하여
        // 에디터 실행 후 가능한 빠른 시점에 로직이 수행되도록 예약합니다.
        static Autorun()
        {
            EditorApplication.update += InitProject;
        }

        // 프로젝트 실행 시 씬 자동 로드 등을 처리하는 메서드입니다.
        private static void InitProject()
        {
            // 한 번 실행되면 즉시 이벤트에서 제거하여 반복 실행을 방지합니다.
            EditorApplication.update -= InitProject;

            // 조건 확인:
            // 1. 에디터가 시작된 지 10초 미만인 경우 (막 켜진 상태)
            // 2. 또는 이전에 이 자동 오픈 로직이 실행된 적이 없는 경우 (!EditorPrefs.GetBool)
            // Application.dataPath를 키로 포함시켜 프로젝트 경로마다 별도로 'AlreadyOpened' 상태를 저장합니다.
            if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool(Application.dataPath + "AlreadyOpened"))
            {
                // 현재 씬이 "game"이 아니고, 해당 씬 폴더가 실제로 존재하는지 확인합니다.
                if (SceneManager.GetActiveScene().name != "game" && Directory.Exists("Assets/BlockPuzzleGameToolkit/Scenes"))
                {
                    // 조건이 맞으면 메인 씬("main.unity")을 강제로 엽니다.
                    // 이는 사용자가 프로젝트를 처음 열었을 때 바로 게임 진입점 씬을 보여주기 위함입니다.
                    EditorSceneManager.OpenScene("Assets/BlockPuzzleGameToolkit/Scenes/main.unity");
                }

                // 다음에 프로젝트를 열 거나 스크립트가 리로드될 때 다시 실행되지 않도록(또는 로직에 따라 처리되도록) 플래그를 저장합니다.
                // 다만 위 로직상 timeSinceStartup < 10 조건 때문에 에디터 재시잣 시에는 다시 실행될 수 있습니다.
                EditorPrefs.SetBool(Application.dataPath + "AlreadyOpened", true);
            }
        }
    }
}