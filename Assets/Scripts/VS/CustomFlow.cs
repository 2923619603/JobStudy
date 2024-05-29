using System;
using System.Collections.Generic;
using Base;
using Manager;
using Unity.VisualScripting;
using UnityEngine;


namespace VS
{

    [UnitCategory("流程")]
    public class 注册游戏中事件 : Unit, IGraphElementWithData
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;
        [DoNotSerialize] public ValueInput EventName;
        [DoNotSerialize] public ValueInput Sender;
        [DoNotSerialize] public ControlOutput EventFlow;
        [DoNotSerialize] public List<ValueOutput> EventArgs = new List<ValueOutput>();

        [SerializeAs(nameof(ArgumentCount))] private int _argumentCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int ArgumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 10);
        }

        public class EventData : IGraphElementData, IObserver
        {
            public WeakReference<GraphReference> Instance;
            public readonly List<object> Args = new List<object>();

            public Action<Flow, string, object, object[]> OnNotifyAction;

            public void OnNotify(string message, object sender, params object[] args)
            {
                if (Instance.TryGetTarget(out var instance))
                {
                    var flow = Flow.New(instance);
                    Args.Clear();
                    foreach (var arg in args)
                    {
                        Args.Add(arg);
                    }

                    OnNotifyAction?.Invoke(flow, message, sender, args);
                }
                else
                {
                    Debug.LogError(message + "Sender————" + sender + args);
                }
            }

        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            var data = instance.GetElementData<EventData>(this);
            data.Instance = new WeakReference<GraphReference>(instance);
        }

        public IGraphElementData CreateData()
        {
            return new EventData();
        }

        protected override void Definition()
        {
            InputTrigger = ControlInput("注册", Register);
            OutputTrigger = ControlOutput("结束");
            EventName = ValueInput<string>("事件名", "");
            Sender = ValueInput<object>("发送者", null);
            EventFlow = ControlOutput("处理事件"); // 定义新的Flow

            EventArgs.Clear();
            for (var i = 0; i < ArgumentCount; i++)
            {
                var localI = i;
                var args = ValueOutput<object>("参数" + i, (flow) =>
                {
                    var data = flow.stack.GetElementData<EventData>(this);
                    return localI < data.Args.Count ? data.Args[localI] : null;
                });
                EventArgs.Add(args);

                Requirement(EventName, EventArgs[i]);
            }

            Requirement(EventName, InputTrigger);
            Succession(InputTrigger, OutputTrigger);
            Succession(InputTrigger, EventFlow);
        }

        private ControlOutput Register(Flow arg)
        {
            var message = arg.GetValue<string>(EventName);
            var sender = arg.GetValue<object>(Sender);
            var data = arg.stack.GetElementData<EventData>(this);

            GameManager.RegisterObserver(message, data, sender as object);
            data.OnNotifyAction = (flow, s, o, objects) =>
            {
                if (s == message)
                {
                    flow.Invoke(EventFlow);
                }
            };

            return OutputTrigger;
        }
    }


    [UnitCategory("流程")]
    public class 发送游戏事件 : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;
        [DoNotSerialize] public ValueInput EventName;

        [SerializeAs(nameof(ArgumentCount))] private int _argumentCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int ArgumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 10);
        }

        [DoNotSerialize] public List<ValueInput> ArgumentPorts { get; } = new List<ValueInput>();

        protected override void Definition()
        {
            InputTrigger = ControlInput("发送", Send);
            OutputTrigger = ControlOutput("结束");
            Succession(InputTrigger, OutputTrigger);
            EventName = ValueInput<string>("事件名", "");
            // Target = ValueInput<GameObject>("自身", null).NullMeansSelf();
            ArgumentPorts.Clear();

            for (var i = 0; i < ArgumentCount; i++)
            {
                ArgumentPorts.Add(ValueInput<object>("参数" + i));
            }

            Requirement(EventName, InputTrigger);
        }

        private ControlOutput Send(Flow flow)
        {
            var message = flow.GetValue<string>(EventName);
            var args = new object[ArgumentCount];

            for (var i = 0; i < ArgumentCount; i++)
            {
                args[i] = flow.GetValue(ArgumentPorts[i]);
            }

            // GameManager.NotifyObservers(message, flow.stack.self, args);
            GameManager.NotifyObservers(message, flow.stack.self, args);
            return OutputTrigger;
        }
    }

    [UnitCategory("流程")]
    public class 设置角色任务状态信息 : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;
        [DoNotSerialize] public ValueInput StateName;
        [DoNotSerialize] public ValueInput NewState;
        [DoNotSerialize] public ValueInput Index;

        protected override void Definition()
        {
            InputTrigger = ControlInput("开始", flow =>
            {
                var stateName = flow.GetValue<string>(StateName);
                var newState = flow.GetValue<string>(NewState);
                var index = flow.GetValue<int>(Index);
                var character = GameManager.Instance.gameRoleTaskStateManager;
                character.SetState(stateName, newState, index);
                return OutputTrigger;
            });
            OutputTrigger = ControlOutput("结束");
            StateName = ValueInput<string>("任务名", "");
            NewState = ValueInput<string>("新状态", "");
            Index = ValueInput<int>("索引", -1);
            Succession(InputTrigger, OutputTrigger);
        }
    }
    
    [UnitCategory("流程")]
    public class 获取角色任务状态信息 : Unit
    {
        [DoNotSerialize] public ValueInput StateName;
        [DoNotSerialize] public ValueOutput State;
        [DoNotSerialize] public ValueInput Index;

        protected override void Definition()
        {
            StateName = ValueInput<string>("任务名", "");
            Index = ValueInput<int>("索引", -1);
            State = ValueOutput<string>("状态",
                flow =>
                {
                    var stateName = flow.GetValue<string>(StateName);
                    var index = flow.GetValue<int>(Index);
                    var stateManager = GameManager.Instance.gameRoleTaskStateManager;

                    return stateManager.GetState(stateName, index);
                });
            Requirement(StateName, State);
        }
    }

    [UnitCategory("流程")]
    public class 角色当前任务状态是否是 : Unit
    {
        [DoNotSerialize] public ValueInput StateName;
        [DoNotSerialize] public ValueInput State;
        [DoNotSerialize] public ValueInput Index;
        [DoNotSerialize] public ValueOutput IsOrNot;
        protected override void Definition()
        {
            IsOrNot = ValueOutput<bool>("是否是", flow =>
            {
                var stateName = flow.GetValue<string>(StateName);
                var state = flow.GetValue<string>(State);
                var index = flow.GetValue<int>(Index);
                var stateManager = GameManager.Instance.gameRoleTaskStateManager;
                return stateManager.IsState(stateName, state, index);
            });
            StateName = ValueInput<string>("任务名", "");
            State = ValueInput<string>("状态", "");
            Index = ValueInput<int>("索引", -1);
            Requirement(StateName, IsOrNot);
        }
    }
}
