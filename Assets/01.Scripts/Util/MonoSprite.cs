using System;
using UnityEngine;

namespace _01.Scripts.Util
{
    public class MonoSprite : MonoBehaviour
    {
        public SpriteRenderer Sr { get; private set; }

        private void Awake()
        {
            Sr = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            Sr.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            Sr.color = color;
        }
    }
}