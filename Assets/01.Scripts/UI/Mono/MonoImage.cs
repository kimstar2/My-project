using System;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.UI.Mono
{
    public class MonoImage : MonoBehaviour
    {
        public Image Image {get; private set;}

        private void Awake()
        {
            Image = GetComponent<Image>();
        }

        public void SetImage(Sprite sprite)
        {
            Image.sprite = sprite;
        }

        public void SetMaterial(Material material)
        {
            Image.material = material;
        }
        
        public void SetEnable(bool enable)
        {
            Image.enabled = enable;
        }
    }
}