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

using BlockPuzzleGameToolkit.Scripts.Popups;
using BlockPuzzleGameToolkit.Scripts.Settings;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    /// <summary>
    /// 게임 내 재화인 '코인'을 관리하는 클래스입니다.
    /// ResourceObject를 상속받아 코인의 획득, 소비, 초기값 설정 등을 처리합니다.
    /// </summary>
    public class Coins : ResourceObject
    {
        // 게임 설정(GameSettings)에서 정의된 코인 초기값을 가져옵니다.
        public override int DefaultValue => Resources.Load<GameSettings>("Settings/GameSettings").coins;

        /// <summary>
        /// 지정된 양의 코인을 소비합니다.
        /// </summary>
        /// <param name="amount">소비할 코인 양</param>
        /// <returns>소비 성공 여부 (잔액 부족 시 실패)</returns>
        public override bool Consume(int amount)
        {
            // 부모 클래스의 Consume을 호출하여 실제 차감을 시도합니다.
            if (!base.Consume(amount))
            {
                // 잔액이 부족하여 실패한 경우, 코인 상점(CoinsShop) 팝업을 띄웁니다.
                MenuManager.instance.ShowPopup<CoinsShop>();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 리소스 초기화 메서드입니다. (현재 버전에서는 별도의 동작 없음)
        /// </summary>
        public override void ResetResource()
        {
        }
    }
}