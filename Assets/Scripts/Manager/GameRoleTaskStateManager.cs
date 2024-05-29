using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modal.Game.Manager
{
    [Serializable]
    public class GameRoleTaskStateManager
    {
       
        /*————————测试————————*/
        public string 演示测试 { get; private set; } = "Start";
        
        public void SetState(string stateName, string newState, int index = -1)
        {
            //根据名字得到对应的类并操作
            var property = GetType().GetProperty(stateName);
            if (property == null)
            {
                Debug.LogError("没有同名的变量，变量是：" + stateName);
                return;
            }

            if (property.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)property.GetValue(this);
                if (index >= list.Count)
                {
                    list.Add(newState);
                    Debug.Log("添加了新的状态：" + newState);
                }
                else
                    list[index] = newState;
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, newState);
            }
            else
            {
                Debug.LogError("没有找到对应的类型" + stateName);
            }
        }

        public bool IsState(string stateName, string state, int index = -1)
        {
            //根据名字得到对应的类并操作
            var property = GetType().GetProperty(stateName);
            if (property == null)
            {
                Debug.LogError("没有同名的变量，变量是：" + stateName);
                return false;
            }

            if (property.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)property.GetValue(this);
                if (index < list.Count)
                    return list[index] == state;
                else
                {
                    Debug.LogError("超索引");
                    return false;
                }

                return false;
            }
            else if (property.PropertyType == typeof(string))
            {
                return (string)property.GetValue(this) == state;
            }
            else
            {
                Debug.LogError("没有找到对应的类型" + stateName);
                return false;
            }
        }

        public string GetState(string stateName, int index = -1)
        {
            //根据名字得到对应的类并操作
            var property = GetType().GetProperty(stateName);
            if (property == null)
            {
                Debug.LogError("没有同名的变量，变量是：" + stateName);
                return null;
            }

            if (property.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)property.GetValue(this);
                return index < list.Count ? list[index] : stateName + "：超索引";
            }
            else if (property.PropertyType == typeof(string))
            {
                return (string)property.GetValue(this);
            }
            else
            {
                Debug.LogError("没有找到对应的类型" + stateName);
                return null;
            }
        }
    }
}