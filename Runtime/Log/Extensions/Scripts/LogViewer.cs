using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class LogViewer : SingletonMono<LogViewer>
    {
        [Header("游戏运行时自动激活")] public bool autoActivate = true;

        [Header("选择电脑显示面板的触发按键，其中BackQuote 是 ~")] public Viewer.CheakingKey CheakingKey = Viewer.CheakingKey.BackQuote;
        [Header("手机多点长按显示面板，可选3指、4指、5指长按")] public Viewer.GestureTouchCount gestureTouchCount = Viewer.GestureTouchCount.Touch3Fingers;

        [Space(5)] [Header("发送邮件")]
        public MailData mailSetting = null;
        private Viewer viewer = null;

        protected override void Init()
        {
            if (autoActivate) transform.GetChild(0).gameObject.SetActive(true);
            Initialize();
        }
        
        public override void OnQuitGame()
        {
            Clear();
        }

        public void AddCheatKeyCallback(Action<string> callback)
        {
            Function.Instance.AddCheatKeyCallback(callback);
        }

        public void AddCommand(object instance, string methodName)
        {
            Function.Instance.AddCommand(instance, methodName);
        }

        public void Show()
        {
            viewer.Show();
        }

        public string MakeLogWithCategory(string message, string category)
        {
            return Log.Instance.MakeLogMessageWithCategory(message, category);
        }

        private void Initialize()
        {
            Function.Instance.Initialize();

            SetMailData();

            if (viewer == null)
            {
                viewer = transform.GetChild(0).GetComponent<Viewer>();
            }

            viewer.Initialize();
            SetCheakingKey();
            SetGestureTouchCount();
        }

        private void Clear()
        {
            Function.Instance.Clear();
        }

        private void SetCheakingKey()
        {
            viewer.SetCheakingKey(CheakingKey);
        }

        private void SetGestureTouchCount()
        {
            viewer.SetGestureTouchCount(gestureTouchCount);
        }

        private void SetMailData()
        {
            Function.Instance.SetMailData(mailSetting);
        }
    }
}