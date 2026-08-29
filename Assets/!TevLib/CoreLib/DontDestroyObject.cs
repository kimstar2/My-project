using UnityEngine;

namespace _TevLib.CoreLib
{
    public class DontDestroyObject : MonoBehaviour
    {
        [field:SerializeField] public bool IsDontDestroyOnLoad { get; private set; }
        private const string DontDestroyOnLoadName = " [ DontDestroyOnLoad ]";
        
        private bool _isSetting;
        
        private void OnEnable()
        {
            if (!IsDontDestroyOnLoad) return;
            if (_isSetting) return;
            _isSetting = true;
            
            NameSet();
            DontDestroyOnLoad(gameObject);
        }

        private void NameSet()
        {
            transform.name += DontDestroyOnLoadName;
            foreach (Transform child in transform)
                child.name += DontDestroyOnLoadName;
        }

        public void SetOption(bool value)
            => IsDontDestroyOnLoad = value;
    }
}
