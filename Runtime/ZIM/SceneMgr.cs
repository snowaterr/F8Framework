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
        // 0-1，在加载时才有值，否则恒为1
        public float LoadingProgress { get; private set; }
        public Scene CurrentScene => SceneManager.GetActiveScene();


        private readonly int EVENT_START_ID = 10100;        // 10100 - 11000，预留900个场景事件

        /// <summary>
        /// 根据id，添加场景加载完成的事件
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="listener"></param>
        public void AddLoadCompleteListener(int sceneId, Action listener)
        {
            MessageManager.Instance.AddEventListener(EVENT_START_ID + sceneId, listener, this);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneId">场景id，与build settings里的index一致</param>
        /// <param name="callBack">加载之后要做的事情(一次性)</param>
        public void LoadScene(int sceneId, UnityAction callBack = null)
        {
            SceneManager.LoadScene(sceneId);
            callBack?.Invoke();
            MessageManager.Instance.DispatchEvent(EVENT_START_ID + sceneId);
        }

        /// <summary>
        /// 异步加载场景，成员变量LoadingProgress可获取进度值
        /// </summary>
        /// <param name="sceneId">场景id，与build settings里的index一致</param>
        /// <param name="isSmoothLoading">是否平滑加载（适用于大于2秒的加载，加载进度值更光滑）</param>
        /// <param name="callBack">异步加载之后要做的事情(一次性)</param>
        public void LoadSceneAsyn(int sceneId, bool isSmoothLoading, UnityAction callBack = null)
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
            LoadingProgress = 1;
            ao.allowSceneActivation = true;
            callBack?.Invoke();
            MessageManager.Instance.DispatchEvent(EVENT_START_ID + sceneId);
        }

        public void OnInit(object createParam)
        {
            LoadingProgress = 1;
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
            
        }
    }
}

