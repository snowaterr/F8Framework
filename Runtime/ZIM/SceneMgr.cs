using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

namespace F8Framework.Core.ZIM
{
    /// <summary>
    /// 场景管理，根据build index加载，场景应添加到build settings中
    /// </summary>
    public class SceneMgr : ModuleSingletonMono<SceneMgr>, IModule
    {
        /// <summary>
        /// 异步加载场景时，输出加载进度，取值范围0-1，在加载时才有值，否则恒为1
        /// </summary>
        public float LoadingProgress { get; private set; }
        public Scene CurrentScene => SceneManager.GetActiveScene();
        public event Action<Scene> SceneLoaded;
        public event Action<Scene> SceneUnloaded;

        //private readonly int EVENT_START_ID = 10100;        // 10100 - 11000，预留900个场景事件

        ///// <summary>
        ///// 根据id，添加场景加载的事件，无参数
        ///// </summary>
        //public void AddLoadListener(int sceneId, Action listener)
        //{
        //    //MessageManager.Instance.AddEventListener(EVENT_START_ID + sceneId, (i)=> listener((Scene)i[0]), this);
        //    MessageManager.Instance.AddEventListener(GetLoadId(sceneId), listener, this);
        //}

        ///// <summary>
        ///// 根据id，添加场景卸载的事件，无参数
        ///// </summary>
        //public void AddUnloadListener(int sceneId, Action listener)
        //{
        //    MessageManager.Instance.AddEventListener(GetUnloadId(sceneId), listener, this);
        //}

        //private int GetLoadId(int sceneId) => EVENT_START_ID + sceneId * 2;
        //private int GetUnloadId(int sceneId) => EVENT_START_ID + sceneId * 2 + 1;

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneId">场景id，与build settings里的index一致</param>
        /// <param name="callBack">加载之后要做的事情(一次性)</param>
        public void LoadScene(int sceneId, UnityAction callBack = null) => StartCoroutine(IELoadScene(sceneId, callBack));

        private IEnumerator IELoadScene(int sceneId, UnityAction callBack = null)
        {
            //MessageManager.Instance.DispatchEvent(GetUnloadId(CurrentScene.buildIndex));
            SceneManager.LoadScene(sceneId);
            while (SceneManager.GetActiveScene().buildIndex != sceneId)  // 等待场景跳转
                yield return null;
            callBack?.Invoke();
            //MessageManager.Instance.DispatchEvent(GetLoadId(sceneId));
        }

        /// <summary>
        /// 异步加载场景，成员变量LoadingProgress可获取进度值
        /// </summary>
        /// <param name="sceneId">场景id，与build settings里的index一致</param>
        /// <param name="isSmoothLoading">是否平滑加载（设置为true后会平滑加载，可通过this.LoadingProgress读取到光滑的加载进度，适用于大于2秒的场景转换）</param>
        /// <param name="callBack">异步加载之后要做的事情(一次性)</param>
        public void LoadSceneAsyn(int sceneId, bool isSmoothLoading = false, UnityAction callBack = null)
        {
            if (LoadingProgress != 1) 
            {
                LogF8.LogWarning("有场景正在异步加载中，忽略新的加载请求");
                return;
            }
            LoadingProgress = 0;
            StartCoroutine(IELoadSceneAsyn(sceneId, isSmoothLoading, callBack));
        }

        private IEnumerator IELoadSceneAsyn(int sceneId, bool isSmoothLoading, UnityAction callBack)
        {
            yield return null;      // 给一帧的时间优先执行其他异步操作
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneId);
            ao.allowSceneActivation = !isSmoothLoading;

            while (!ao.isDone && ao.progress < 0.9f)    // 如果allowSceneActivation是false，那么这里的ao.progress只会到0.9，不会继续增长
            {
                if (isSmoothLoading)
                {
                    if (LoadingProgress < ao.progress)
                        LoadingProgress += Time.deltaTime;
                    else if (LoadingProgress < 0.3f)
                        LoadingProgress += Time.deltaTime * 0.2f;
                }
                else
                {
                    LoadingProgress = ao.progress;
                }

                yield return null;
            }

            if (isSmoothLoading)
            {
                for (var p = Math.Min(LoadingProgress + Time.deltaTime, 1); p < 1; p += Time.deltaTime * 2f) 
                {
                    LoadingProgress = p;
                    yield return null;
                }
            }

            // 跳转场景
            //MessageManager.Instance.DispatchEvent(GetUnloadId(CurrentScene.buildIndex));
            LoadingProgress = 1;
            ao.allowSceneActivation = true;
            while (SceneManager.GetActiveScene().buildIndex != sceneId)  // 等待场景跳转
                yield return null;
            callBack?.Invoke();
            //MessageManager.Instance.DispatchEvent(GetLoadId(sceneId));
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
            if (mode == LoadSceneMode.Additive) 
            {
                LogF8.LogWarning("框架里的场景管理器不支持Additive模式加载");
                return;
            }
            SceneLoaded?.Invoke(scene);
        }

        private void OnSceneUnload(Scene scene)
        {
            SceneUnloaded?.Invoke(scene);
        }

        public void OnInit(object createParam)
        {
            LoadingProgress = 1;
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }
    }
}

