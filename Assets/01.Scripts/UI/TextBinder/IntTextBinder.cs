using _01.Scripts.UI.Mono;
using UnityEngine;

namespace _01.Scripts.UI.TextBinder
{
    public class IntTextBinder : MonoTMP
    {
        [Tooltip("바인딩을 원하는 위치에 {0}을 적어주세요."),SerializeField,TextArea] private string content;
        public void Binding(int value)
        {
            TMP.SetText(string.Format(content, value));
        }
    }
}