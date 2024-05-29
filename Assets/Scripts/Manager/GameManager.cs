using System;
using Base;
using Modal.Game.Manager;
using UnityEngine;
using Utils;

namespace Manager
{
    [Serializable]
    public class GameManager
    {
        private static GameManager _instance;

        public Subject ObserverManager = new Subject();
        public GameRoleTaskStateManager gameRoleTaskStateManager;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                    //游戏其他初始化
                    _instance.LoadData();
                    //角色状态储存放Player存档代码中
                    _instance.gameRoleTaskStateManager = new GameRoleTaskStateManager();
                }

                return _instance;
            }
        }

        //加载游戏数据
        private void LoadData()
        {
        
        }
    
    
        //Observer
        public static void RegisterObserver(string message, IObserver observer, object sender = null)
        {
            Instance.ObserverManager.RegisterObserver(message, observer, sender);
        }
    
        public static void RegisterObserver(string[] message, IObserver observer, object sender = null)
        {
            Instance.ObserverManager.RegisterObserver(message, observer, sender);
        }

        public static void UnregisterObserver(string message, IObserver observer)
        {
            Instance.ObserverManager.UnregisterObserver(message, observer);
        }

        public static void UnregisterObserver(IObserver observer)
        {
            Instance.ObserverManager.UnregisterObserver(observer);
        }

        public static void NotifyObservers(string message, object senderObject, params object[] args)
        {
            Instance.ObserverManager.NotifyObservers(message, senderObject, args);
        }
    
        public static void NotifyObserverNextFrame
            (MonoBehaviour mono, string message, object senderObject, params object[] args)
        {
            mono.NextFrame(() =>
            {
                Instance.ObserverManager.NotifyObservers(message, senderObject, args);
            });
        }
    
    }
}