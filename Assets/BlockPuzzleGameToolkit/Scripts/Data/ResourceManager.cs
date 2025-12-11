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

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    /// <summary>
    /// 게임 내 모든 리소스(재화, 아이템 등)를 중앙에서 관리하는 싱글톤 클래스입니다.
    /// Resources/Variables 폴더에 있는 모든 ResourceObject를 로드하고 초기화합니다.
    /// </summary>
    public class ResourceManager : SingletonBehaviour<ResourceManager>
    {
        private ResourceObject[] resources; // 로드된 모든 리소스 배열

        // 리소스 배열에 대한 프로퍼티 (지연 초기화 지원)
        public ResourceObject[] Resources
        {
            get
            {
                // 리소스가 아직 로드되지 않았다면 초기화 진행
                if (resources == null || resources.Length == 0)
                {
                    Init();
                }

                return resources;
            }
            set => resources = value;
        }

        public override void Awake()
        {
            base.Awake();
            Init();
        }

        // 리소스 초기화: "Resources/Variables" 경로에서 모든 ResourceObject를 로드하고 저장된 값을 불러옵니다.
        private void Init()
        {
            Resources = UnityEngine.Resources.LoadAll<ResourceObject>("Variables");
            foreach (var resource in Resources)
            {
                resource.LoadPrefs();
            }
        }

        /// <summary>
        /// 특정 이름의 리소스를 소비합니다.
        /// </summary>
        /// <param name="resourceName">소비할 리소스의 이름 (파일 이름)</param>
        /// <param name="amount">소비할 양</param>
        /// <returns>소비 성공 여부 (리소스를 못 찾았거나 잔액 부족 시 false)</returns>
        public bool Consume(string resourceName, int amount)
        {
            var resource = GetResource(resourceName);
            if (resource == null)
            {
                Debug.LogError($"Resource {resourceName} not found");
                return false;
            }

            return resource.Consume(amount);
        }

        /// <summary>
        /// 이름으로 특정 리소스 객체를 찾아 반환합니다.
        /// </summary>
        /// <param name="resourceName">찾을 리소스 이름</param>
        /// <returns>ResourceObject 객체 또는 null</returns>
        public ResourceObject GetResource(string resourceName)
        {
            foreach (var resource in Resources)
            {
                if (resource.name == resourceName)
                {
                    return resource;
                }
            }

            return null;
        }
    }
}