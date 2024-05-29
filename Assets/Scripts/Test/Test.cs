using System;
using Base;
using Manager;
using Modal.Game.Manager;
using UnityEngine;
using Utils;

namespace Test
{
    public class Test : MonoBehaviour,IObserver
    {
        private void OnEnable()
        {
            //GameManager.RegisterObserver(MessageList.Test,this,null);
            GameManager.RegisterObserver(
                new string[] { MessageList.Test, MessageList.VsTest },
                this,
                null);
        }

        public void NotifyObserver()
        {
            GameManager.NotifyObservers(MessageList.Test,gameObject,"A");
        }
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         GameManager.NotifyObservers(MessageList.Test,this,"A");
        //     }
        // }

        private void OnDisable()
        {
            GameManager.UnregisterObserver(this);
        }

        public void OnNotify(string message, object sender, params object[] args)
        {
            switch (message)
            {
                case MessageList.Test:
                    this.Delay(true, 0.1f, (() =>
                    {
                        Debug.Log("Test触发");
                        Debug.LogWarning((sender as GameObject).name);
                    }));
                    break;
                case MessageList.VsTest:
                    Debug.Log("VSTest Trigger");
                    Debug.LogWarning(args[0]);
                    Debug.Log(GameManager.Instance.gameRoleTaskStateManager.
                            IsState("演示测试","Done"));
                    break;
            }
        }
    }
}