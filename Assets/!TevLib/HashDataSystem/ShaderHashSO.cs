using UnityEngine;

namespace _TevLib.HashDataSystem
{
    [CreateAssetMenu(fileName = "ShaderHash data", menuName = "TevLib/System/HashData/ShaderHash", order = 0)]
    public class ShaderHashSO : ScriptableObject
    {
        [field:SerializeField] public string HashName {get; private set;}
        [field:SerializeField] public int HashValue {get; private set;}

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(HashName))
            {
                HashValue = 0;
                return;
            }
            
            HashValue = Shader.PropertyToID(HashName);
        }
    }
}