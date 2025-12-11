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

using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.Audio;

namespace BlockPuzzleGameToolkit.Scripts.Audio
{
    /// <summary>
    /// 배경음악(BGM)의 초기 설정을 관리하는 싱글톤 클래스입니다.
    /// 게임 시작 시 저장된 설정(PlayerPrefs)을 읽어 오디오 믹서의 볼륨을 조절합니다.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicBase : SingletonBehaviour<MusicBase>
    {
        // 제어할 오디오 믹서 참조
        [SerializeField]
        private AudioMixer mixer;

        // 믹서에서 제어할 볼륨 파라미터의 이름 (Exposed Parameter)
        [SerializeField]
        private string musicParameter = "musicVolume";

        private void Start()
        {
            // "Music" 키로 저장된 설정을 확인합니다 (기본값 1: 켜짐).
            // 0이면(꺼짐) 볼륨을 -80dB로 설정하여 음소거하고, 그렇지 않으면 0dB로 설정합니다.
            mixer.SetFloat(musicParameter, PlayerPrefs.GetInt("Music", 1) == 0 ? -80 : 0);
        }
    }
}