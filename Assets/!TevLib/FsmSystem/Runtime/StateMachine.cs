using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.FsmSystem.Runtime
{
    public class StateMachine
    {
        public AbstractState CurrentState;
        
        private Dictionary<int, AbstractState> _stateDict;
        public IMachineOwner Owner {get; private set;}
        public FsmDebugger FsmDebugger {get; private set;}
        
        public StateMachine(IMachineOwner owner, StateSO[] states)
        {
            _stateDict = new Dictionary<int, AbstractState>();
            
            Owner = owner;
            if (Owner.GameObject.TryGetComponent(out FsmDebugger fsmDebugger))
                FsmDebugger = fsmDebugger;
            
            foreach (StateSO stateData in states)
            {
                Type type = Type.GetType(stateData.className);
                Debug.Assert(type != null,$"Finding Type is null {stateData.className}");
                AbstractState agentState = Activator.CreateInstance(type, owner, stateData) as AbstractState;
                
                _stateDict.Add(stateData.assetIndex , agentState);
            }
        }
        
        public void ChangeState(int newStateIndex)
        {
            AbstractState newState = _stateDict.GetValueOrDefault(newStateIndex);
            
            StateDebug(newState);

            CurrentState?.Exit();
            Debug.Assert(newState != null,$"Finding Type is null {newStateIndex}");
            
            CurrentState = newState;
            CurrentState.Enter();
        }
        
        public void UpdateMachine() => CurrentState?.Update();

        #region ForDebug

        private void StateDebug(AbstractState newState)
        {
            if (FsmDebugger == null) return;
            if (FsmDebugger.DebugMessageData == null) return;
            if (!FsmDebugger.IsMachineDebug) return;
            
            if (FsmDebugger.DebugMessageData.NewState &&
                !FsmDebugger.DebugMessageData.CurrentState)
                Debug.LogFormat(FsmDebugger.DebugMessageData.DebugMessage, newState?.StateName);
            
            else if (FsmDebugger.DebugMessageData.CurrentState &&
                     !FsmDebugger.DebugMessageData.NewState)
                Debug.LogFormat(FsmDebugger.DebugMessageData.DebugMessage, CurrentState?.StateName);
            
            else if (FsmDebugger.DebugMessageData.NewState &&
                     FsmDebugger.DebugMessageData.CurrentState)
                Debug.LogFormat(FsmDebugger.DebugMessageData.DebugMessage, CurrentState?.StateName , newState?.StateName);
        }

        #endregion

    }
}