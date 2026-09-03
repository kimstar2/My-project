using System;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI.Mono
{
    public class MonoOutLine : MonoBehaviour
    {
        public Outline Outline { get; private set; }

        private void Awake()
        {
            Outline = GetComponent<Outline>();
        }

        public void SetColor(Color color)
        {
            Outline.effectColor = color;
        }
        
        public void SetEnable(bool enable)
        {
            Outline.enabled = enable;
        }
    }
}