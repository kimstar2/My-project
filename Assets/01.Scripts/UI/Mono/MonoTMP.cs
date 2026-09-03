using TMPro;
using UnityEngine;

namespace _01.Scripts.UI.Mono
{
    public class MonoTMP : MonoBehaviour
    {
        protected TextMeshPro TMP;
        private void Awake()
        {
            TMP = GetComponent<TextMeshPro>();
        }
    }
}