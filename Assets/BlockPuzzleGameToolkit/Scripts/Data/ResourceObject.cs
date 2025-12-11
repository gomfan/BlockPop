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
using System.Threading.Tasks;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    /// <summary>
    /// 게임 내 모든 리소스(재화)의 기본이 되는 추상 클래스입니다.
    /// ScriptableObject를 상속받아 데이터 파일로 관리가 가능하며,
    /// PlayerPrefs를 이용한 저장/불러오기 기능을 내장하고 있습니다.
    /// </summary>
    public abstract class ResourceObject : ScriptableObject
    {
        public ResourceValue ResourceValue;

        //name of the resource
        // 리소스 이름 (ScriptableObject 파일 이름을 키로 사용)
        private string ResourceName => name;

        // 리소스의 기본값 (자식 클래스에서 정의해야 함)
        public abstract int DefaultValue { get; }

        //value of the resource
        // 현재 리소스 보유량 (메모리 상의 값)
        private int Resource;

        public AudioClip sound;

        // 리소스 값이 변경될 때 알림을 받기 위한 델리게이트 및 이벤트
        public delegate void ResourceUpdate(int count);
        public event ResourceUpdate OnResourceUpdate;

        //runs when the object is created
        // 객체가 활성화될 때(게임 시작 등) 비동기로 저장된 값을 불러옵니다.
        private void OnEnable()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await LoadPrefs();
            });
        }

        //loads prefs from player prefs and assigns to resource variable
        /// <summary>
        /// PlayerPrefs에서 저장된 값을 불러와 메모리에 적재합니다.
        /// </summary>
        public Task LoadPrefs()
        {
            Resource = LoadResource();
            return Task.CompletedTask;
        }

        public int LoadResource()
        {
            // 키(ResourceName)가 없으면 DefaultValue를 사용합니다.
            return PlayerPrefs.GetInt(ResourceName, DefaultValue);
        }

        //adds amount to resource and saves to player prefs
        /// <summary>
        /// 리소스를 추가하고 변경사항을 저장합니다.
        /// </summary>
        /// <param name="amount">추가할 양</param>
        public void Add(int amount)
        {
            Resource += amount;
            PlayerPrefs.SetInt(ResourceName, Resource);
            OnResourceChanged();
        }

        //sets resource to amount and saves to player prefs
        /// <summary>
        /// 리소스를 특정 값으로 설정하고 저장합니다.
        /// </summary>
        /// <param name="amount">설정할 값</param>
        public void Set(int amount)
        {
            Resource = amount;
            PlayerPrefs.SetInt(ResourceName, Resource);
            PlayerPrefs.Save();
            OnResourceChanged();
        }

        //consumes amount from resource and saves to player prefs if there is enough
        /// <summary>
        /// 리소스를 소비합니다.
        /// </summary>
        /// <param name="amount">소비할 양</param>
        /// <returns>소비 성공 여부 (잔액이 충분하면 true)</returns>
        public virtual bool Consume(int amount)
        {
            if (IsEnough(amount))
            {
                Resource -= amount;
                PlayerPrefs.SetInt(ResourceName, Resource);
                PlayerPrefs.Save();
                OnResourceChanged();
                return true;
            }

            return false;
        }

        //callback for ui elements
        // 값이 변경되었을 때 이벤트를 호출하여 UI 등을 갱신하게 합니다.
        private void OnResourceChanged()
        {
            OnResourceUpdate?.Invoke(Resource);
        }

        //get the resource
        /// <summary>
        /// 현재 보유량 확인
        /// </summary>
        public int GetValue()
        {
            return Resource;
        }

        /// <summary>
        /// 리소스가 충분한지 검사합니다.
        /// </summary>
        /// <param name="targetAmount">필요한 양</param>
        /// <returns>충분하면 true</returns>
        public bool IsEnough(int targetAmount)
        {
            if (GetValue() < targetAmount)
            {
                Debug.Log("Not enough " + ResourceName);
            }

            return GetValue() >= targetAmount;
        }

        public abstract void ResetResource();
    }

    [Serializable]
    public class ResourceValue
    {
    }
}