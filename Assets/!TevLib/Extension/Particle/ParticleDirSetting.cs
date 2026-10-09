using System;
using _01.Scripts.Agent.Enemies.BT;
using NaughtyAttributes;
using UnityEngine;

namespace _TevLib.Extension.Particle
{
    [Serializable]
    public struct ParticleDirSetting
    {
        public const string DirTypeVar = "ParticleDirType";
        
        [field:SerializeField] public ParticleDirType ParticleDirType { get; private set; }
        [field: SerializeField] public float Multi { get; private set; } // 테스트용 원래는 이넘값으로 해줄려했는데
        // 그게 안됨
    }
}