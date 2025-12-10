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

using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    // 유니티 에디터 상단 메뉴에 'Tools' 메뉴를 추가하여 게임 설정, 씬 이동, 문서 확인 등의 편의 기능을 제공하는 정적 클래스입니다.
    public static class EditorMenu
    {
        public static string BlockPuzzleGameToolkit = "BlockPuzzleGameToolkit";

        // 상점(Shop) 설정 파일을 선택합니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Shop settings")]
        public static void IAPProducts()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/CoinsShopSettings.asset");
        }

        // 광고(Ads) 설정 파일을 선택합니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Ads settings")]
        public static void AdsSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/AdsSettings.asset");
        }

        // 데일리 보너스(Daily Bonus) 설정 파일을 선택합니다.
        //DailyBonusSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Daily bonus settings")]
        public static void DailyBonusSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/DailyBonusSettings.asset");
        }

        // 게임 전반적인 설정(Game Settings) 파일을 선택합니다.
        //GameSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Game settings")]
        public static void GameSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/GameSettings.asset");
        }

        // 스핀(Spin) 룰렛 설정 파일을 선택합니다.
        //SpinSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Spin settings")]
        public static void SpinSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/SpinSettings.asset");
        }

        // 디버그 설정(Debug Settings) 파일을 선택합니다.
        //DebugSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Debug settings")]
        public static void DebugSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/DebugSettings.asset");
        }

        // 튜토리얼 설정(Tutorial Settings) 파일을 선택합니다.
        //TutorialSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Tutorial settings")]
        public static void TutorialSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/TutorialSettings.asset");
        }

        // 메인 씬(Main scene)을 열고 상태를 MainMenu로 설정합니다. 단축키 Alt+1 (&1)이 지정되어 있습니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Main scene &1", priority = 0)]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/" + BlockPuzzleGameToolkit + "/Scenes/main.unity");
            StateManager.instance.CurrentState = EScreenStates.MainMenu;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // 게임 씬 상태로 전환합니다. (실제 씬을 로드하지는 않고 상태만 변경하는 것으로 보임) 단축키 Alt+2 (&2)
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Game scene &2")]
        public static void GameScene()
        {
            StateManager.instance.CurrentState = EScreenStates.Game;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // 맵 씬 상태로 전환합니다. 단축키 Alt+3 (&3)
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Map scene &3")]
        public static void MapScene()
        {
            StateManager.instance.CurrentState = EScreenStates.Map;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        // 레벨 데이터 파일(Level_1.asset)을 선택하여 인스펙터 창에서 편집할 수 있게 합니다. 단축키 Shift+C (_C)
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Level Editor _C", priority = 1)]
        public static void LevelEditor()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Levels/Level_1.asset");
        }

        // 색상 관련 설정 파일(ItemTemplate 0.asset)을 선택합니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Color editor", priority = 1)]
        public static void ColorEditor()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Items/ItemTemplate 0.asset");
        }

        // 모양(Shape) 데이터 파일 중 첫 번째를 찾아 선택합니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Shape editor", priority = 1)]
        public static void ShapeEditor()
        {
            var shapeAssets = Resources.LoadAll("Shapes");
            if (shapeAssets.Length > 0)
            {
                Selection.activeObject = shapeAssets[0];
            }
            else
            {
                Debug.LogWarning("No shape assets found in the specified folder.");
            }
        }

        // 온라인 메인 문서를 브라우저로 엽니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/Main", priority = 2)]
        public static void MainDoc()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/main");
        }

        // 광고 설정 관련 문서를 엽니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/ADS/Setup ads")]
        public static void UnityadsDoc()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/bubble-shooter-toolkit/tutorials/ads-setup/");
        }

        // 인앱 결제(IAP) 관련 문서를 엽니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/Unity IAP (in-apps)")]
        public static void Inapp()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/main/block-puzzle-game-toolkit/setting-up-in-app-purchase-products");
        }


        // 플레이어 데이터(PlayerPrefs)를 초기화합니다. 개발 시 세이브 데이터를 날리고 테스트할 때 유용합니다.
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Reset PlayerPrefs")]
        private static void ResetPlayerPrefs()
        {
            GameDataManager.ClearALlData();
            PlayerPrefs.DeleteKey("GameState");
            Debug.Log("PlayerPrefs are reset");
        }
    }
}