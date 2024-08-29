using System;
using UnityEngine;

namespace F8Framework.Core
{
    [RequireComponent(typeof(RectTransform))]
    public class BaseView : ComponentBind
    {
        public enum WindowState
        {
            Awake,
            Animating,
            Ready,
            Closed
        }

        public RectTransform rectTransform => transform as RectTransform;
        /// <summary>
        /// UI所属的图层（canvas）
        /// </summary>
        public RectTransform Layer => UIManager.Instance.GetLayer(UIid);

        private int UIid;
        private string Guid;
        private object[] Args;
        private WindowState windowState = WindowState.Closed;
        
        // 消息事件
        private EventDispatcher _eventDispatcher = null;
                
        public EventDispatcher EventDispatcher {
            get
            {
                if (_eventDispatcher == null)
                {
                    _eventDispatcher = new EventDispatcher();
                }
        
                return _eventDispatcher;
            }
        }

        private void Awake()
        {
            windowState = WindowState.Awake;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        public void Added(int uiId, string guid, object[] args = null)
        {
            this.Args = args;
            this.UIid = uiId;
            this.Guid = guid;
            OnAdded(uiId, args);
            windowState = WindowState.Animating;
            OnViewTweenInit();
            OnPlayViewTween();
        }

        protected virtual void OnAdded(int uiId, object[] args = null)
        {
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
        }
        
        protected virtual void OnViewTweenInit()
        {
        }
        
        protected virtual void OnPlayViewTween()
        {
            windowState = WindowState.Ready;
            OnViewOpen();
        }

        protected virtual void OnViewOpen()
        {
        }

        public void AddEscBtn()
        {

        }

        public void OnEscBtn()
        {

        }

        public void Close(bool isDestroy = false)
        {
            windowState = WindowState.Closed;
            UIManager.Instance.Close(this.UIid, isDestroy, this.Guid);
        }

        public void BeforeRemove()
        {
            if (_eventDispatcher != null) {
                _eventDispatcher.Clear();
                _eventDispatcher = null;
            }
            OnBeforeRemove();
        }

        protected virtual void OnBeforeRemove()
        {
        }

        public void Removed()
        {
            OnRemoved();
        }

        protected virtual void OnRemoved()
        {
        }
        
        public void AddEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void AddEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.AddEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void RemoveEventListener<T>(T eventName, Action<object[]> listener) where T : struct, Enum, IConvertible
        {
            EventDispatcher.RemoveEventListener(eventName, listener, this);
        }
            
        public void DispatchEvent<T>(T eventName) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName);
        }
            
        public void DispatchEvent<T>(T eventName, params object[] arg1) where T : struct, Enum, IConvertible
        {
            EventDispatcher.DispatchEvent(eventName, arg1);
        }
        
        public void AddEventListener(int eventId, Action listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void AddEventListener(int eventId, Action<object[]> listener)
        {
            EventDispatcher.AddEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void RemoveEventListener(int eventId, Action<object[]> listener)
        {
            EventDispatcher.RemoveEventListener(eventId, listener, this);
        }
            
        public void DispatchEvent(int eventId)
        {
            EventDispatcher.DispatchEvent(eventId);
        }
            
        public void DispatchEvent(int eventId, params object[] arg1)
        {
            EventDispatcher.DispatchEvent(eventId, arg1);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener<T>(T eventName)
        {
            int tempName = (int)(object)eventName;
            EventDispatcher.RemoveEventListener(tempName);
        }
        
        /// <summary>
        /// 删除此事件所有监听（慎用）
        /// </summary>
        public void RemoveEventListener(int eventId)
        {
            EventDispatcher.RemoveEventListener(eventId);
        }
    }
}
