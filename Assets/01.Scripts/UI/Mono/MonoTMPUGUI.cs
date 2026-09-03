using TMPro;
using UnityEngine;

namespace _01.Scripts.UI.Mono
{
    public class MonoTMPUGUI : MonoBehaviour
    {
        private TextMeshProUGUI _tmp;
        private void Awake()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
        }
        public void SetText(string text)
        {
            _tmp.SetText(text);
        }
    }
}