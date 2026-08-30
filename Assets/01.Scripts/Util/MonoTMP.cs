using System;
using TMPro;
using UnityEngine;

namespace _01.Scripts.Util
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