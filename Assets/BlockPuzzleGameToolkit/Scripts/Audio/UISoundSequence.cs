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

namespace BlockPuzzleGameToolkit.Scripts.Audio
{
    /// <summary>
    /// UI 등에서 순차적으로 다른 소리를 재생하고 싶을 때 사용하는 컴포넌트입니다.
    /// 예를 들어, 버튼을 누를 때마다 도-레-미-파... 순서로 음이 올라가는 효과 등을 구현할 때 사용됩니다.
    /// </summary>
    public class UISoundSequence : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] clips; // 재생할 오디오 클립들의 배열

        private int _index; // 현재 재생할 순서의 인덱스

        /// <summary>
        /// 설정된 클립 배열에서 현재 순서의 소리를 재생하고, 다음 순서로 넘어갑니다.
        /// 배열의 끝에 도달하면 다시 0번 인덱스로 돌아갑니다(Loop).
        /// </summary>
        public void PlaySound()
        {
            if (clips.Length == 0)
            {
                return;
            }

            SoundBase.instance.PlaySound(clips[_index]);
            _index++;
            if (_index >= clips.Length)
            {
                _index = 0;
            }
        }
    }
}