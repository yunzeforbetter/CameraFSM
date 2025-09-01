using System;
using System.Collections.Generic;


namespace CMCameraFramework
{
    public class FSM
    {
        protected Dictionary<Type, FSMState> States = new Dictionary<Type, FSMState>();
        protected Dictionary<Type, FSMEvent> Events = new Dictionary<Type, FSMEvent>();
        public FSMState lastState;
        //        protected FSMState nextState;


        public void AddState<T>(IFSMEntity entity, int StateType) where T : FSMState, new()
        {
            if (States.ContainsKey(typeof(T)) == false)
            {
                FSMState item = new T();
                item.FSMEntity = entity;
                item.StateType = StateType;
                States.Add(typeof(T), item);
            }
        }

        public T GetState<T>() where T : FSMState
        {
            return (T)States[typeof(T)];
        }

        public void AddEvent<T>(int eventType) where T : FSMEvent, new()
        {
            if (Events.ContainsKey(typeof(T)) == false)
            {
                FSMEvent item = new T();
                item.Id = eventType;
                Events.Add(typeof(T), item);
            }
        }

        public T GetEvent<T>() where T : FSMEvent
        {
            var key = typeof(T);
            FSMEvent ret = null;
            if (Events.TryGetValue(key, out ret))
            {
                ((T) ret).Reset();
            }
           
            return (T)ret;
        }

        public FSMEvent GetFSMEvent(Type type)
        {
            FSMEvent fSMEvent = null;
            if (Events.TryGetValue(type, out fSMEvent))
            {
                fSMEvent.Reset();
            }
            return fSMEvent;
        }

        public virtual void InitFSM(IFSMEntity entity, FSMState state)
        {
            ChangeCurrentState(entity, null, state, new ResetEvent());
        }

        public void Tick(IFSMEntity entity, float deltaTime)
        {
            //FiniteStateMachineComponent currentState = entity.GetComponent(StateType) as FiniteStateMachineComponent;
            //var entity = timeScaleComponents[index].EntityBase;
            if (entity != null)
            {
                FSMState currentState = entity.GetFSMState();
                if (currentState != null) TickState(entity, currentState, deltaTime);
            }
            else
            {
                //Logger.Trace("FiniteStateMachineSystem NormalTick Error Index:{0}", index);
            }
        }

        protected void TickState(IFSMEntity entity, FSMState currentState, float deltaTime)
        {
            if (currentState != null)
            {
                //var caller = currentState;
                //var nextState = caller.Tick(deltaTime);
                //if (caller != currentState)
                //{
                //    return;
                //}
                var nextState = currentState.Tick(deltaTime);
                if (nextState != currentState)
                {
                    ChangeCurrentState(entity, currentState, nextState, TickEvent<float>.GetInstance(deltaTime));
                }
            }
        }

        protected void ChangeCurrentState(IFSMEntity entity, FSMState currentState, FSMState nextState, FSMEvent e)
        {
            if (currentState != null)
            {
                currentState.Exit(e, nextState);
                //entity.RemoveComponent(currentState.Identity);
            }

           // LiteCommon.BLog.I($"ChangeCurrentState currentState={currentState} nextState={nextState} frame={Time.frameCount}");
            if (nextState != null)
            {
                //entity.AddComponent(nextState.Identity, nextState);
                lastState = currentState;
                FSMState enterNew = nextState.Enter(e, currentState);
                if (enterNew != null && enterNew != nextState)//Mian State下可能在Enter后直接进入默认子状态
                {
                    ChangeCurrentState(entity, nextState, enterNew, e);
                }
                else
                {
                    entity.SetFSMState(nextState);
                }
            }
        }

        public void Reset(IFSMEntity entity, FSMState startState)
        {
            ChangeCurrentState(entity, null, startState, new ResetEvent());
        }

        public bool Input(FSMEvent e, IFSMEntity entity, FSMState currentState)
        {
            if (currentState == null)
            {
                return false;
            }
            //LiteCommon.BLog.I("currentState = " + currentState.StateType + " Event = " + e);
            var nextState = currentState.OnInputEvent(e);
            if (nextState != currentState)
            {
                ChangeCurrentState(entity, currentState, nextState, e);
                return true;
            }
            return false;
        }
    }
}
