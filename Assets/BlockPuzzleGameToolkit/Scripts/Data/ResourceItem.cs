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

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    /// <summary>
    /// 에디터에서 생성 가능한 일반적인 리소스 데이터입니다.
    /// 코인 외의 다른 재화나 아이템(예: 하트, 보석 등)을 정의할 때 사용됩니다.
    /// ScriptableObject로 생성하여 인스펙터에서 초기값을 설정할 수 있습니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Resource", menuName = "BlockPuzzleGameToolkit/Data/ResourceItem", order = 1)]
    public class ResourceItem : ResourceObject
    {
        [Tooltip("이 리소스의 기본 초기 수량")]
        public int defaultValue;

        // ResourceObject의 DefaultValue 프로퍼티를 오버라이드하여 인스펙터 설정값을 반환
        public override int DefaultValue => defaultValue;

        public override void ResetResource()
        {
            // 리소스 초기화 로직이 필요한 경우 여기에 구현
        }
    }
}