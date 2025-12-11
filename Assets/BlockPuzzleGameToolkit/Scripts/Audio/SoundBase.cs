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

using System.Collections;
using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.Audio;

namespace BlockPuzzleGameToolkit.Scripts.Audio
{
    /// <summary>
    /// 게임의 효과음(SFX)을 관리하는 싱글톤 클래스입니다.
    /// 다양한 효과음 클립들을 보관하고 재생하는 메서드들을 제공합니다.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundBase : SingletonBehaviour<SoundBase>
    {
        [SerializeField]
        private AudioMixer mixer; // 오디오 믹서 참조

        [SerializeField]
        private string soundParameter = "soundVolume"; // 믹서의 SFX 볼륨 파라미터 이름

        // 게임에서 사용되는 주요 효과음 클립들
        public AudioClip click;           // 클릭 소리
        public AudioClip[] swish;         // 스와이프/이동 소리 (다양한 버전)
        public AudioClip coins;           // 코인 획득 소리
        public AudioClip coinsSpend;      // 코인 사용 소리
        public AudioClip luckySpin;       // 럭키 스핀 소리
        public AudioClip warningTime;     // 시간 경고 소리
        public AudioClip placeShape;      // 블록 배치 소리
        public AudioClip fillEmpty;       // 빈칸 채울 때 소리 (추정)
        public AudioClip alert;           // 알림 소리
        public AudioClip[] combo;         // 콤보 발생 소리 (단계별)

        private AudioSource audioSource;

        // 중복 재생 방지를 위해 현재 재생 중인(혹은 방금 재생된) 클립을 추적하는 세트
        private readonly HashSet<AudioClip> clipsPlaying = new();

        public override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // 게임 시작 시 저장된 사운드 설정(PlayerPrefs)을 적용 (1: 켜짐, 0: 꺼짐)
            mixer.SetFloat(soundParameter, PlayerPrefs.GetInt("Sound", 1) == 0 ? -80 : 0);
        }

        /// <summary>
        /// 지정된 오디오 클립을 한 번 재생합니다. (중첩 가능)
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// 일정 시간 딜레이 후 효과음을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="delay">지연 시간(초)</param>
        public void PlayDelayed(AudioClip clip, float delay)
        {
            StartCoroutine(PlayDelayedCoroutine(clip, delay));
        }

        private IEnumerator PlayDelayedCoroutine(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(clip);
        }

        /// <summary>
        /// 주어진 클립 배열 중 하나를 무작위로 선택하여 재생합니다.
        /// </summary>
        /// <param name="clip">오디오 클립 배열</param>
        public void PlaySoundsRandom(AudioClip[] clip)
        {
            instance.PlaySound(clip[Random.Range(0, clip.Length)]);
        }

        /// <summary>
        /// 동일한 클립이 너무 빈번하게 중복 재생되는 것을 방지하며 소리를 재생합니다.
        /// (예: 여러 블록이 동시에 터질 때 소리가 겹쳐서 시끄러운 현상 방지)
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        public void PlayLimitSound(AudioClip clip)
        {
            // 이미 재생 목록에 있다면(최근 0.1초 내) 재생하지 않음
            if (clipsPlaying.Add(clip))
            {
                PlaySound(clip);
                StartCoroutine(WaitForCompleteSound(clip));
            }
        }

        // 제한된 사운드 목록에서 제거하기 위한 대기 코루틴
        private IEnumerator WaitForCompleteSound(AudioClip clip)
        {
            yield return new WaitForSeconds(0.1f);
            clipsPlaying.Remove(clip);
        }
    }
}