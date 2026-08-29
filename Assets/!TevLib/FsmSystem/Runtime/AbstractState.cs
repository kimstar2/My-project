namespace _TevLib.FsmSystem.Runtime
{
    public abstract class AbstractState
    {
        public string StateName => StateSo?.stateName;

        protected IMachineOwner Owner;
        protected readonly StateSO StateSo;
        
        protected AbstractState(IMachineOwner owner, StateSO stateData)
        {
            Owner = owner;
            StateSo = stateData;
        }

        public virtual void Enter() {}
        public void Update() => OnUpdate();
        // 실패 시 추가적인 실행 방지를 위한 훅
        protected virtual bool OnUpdate() => false; // 훅
        public virtual void Exit() {}
    }
}