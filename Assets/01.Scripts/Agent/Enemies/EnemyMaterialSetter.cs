using System;
using _TevLib.HashDataSystem;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies
{
    public class EnemyMaterialSetter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float tweenTime = 0.15f;
        [SerializeField] private ShaderHashSO voronoiValueHash;
        [SerializeField] private Transform idTrm;
        private int _id;
        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(_mpb);
            _id = idTrm.GetHashCode();
        }

        private void OnDisable()
        {
            DOTween.Kill(_id);
        }

        public void SetValue(float value)
        {
            if (_mpb == null) return;
            DOTween.Kill(_id);
            
            float crtValue = _mpb.GetFloat(voronoiValueHash.HashValue);
            DOTween.To(() => crtValue, x =>
            {
                crtValue = x;
                ApplyValue(crtValue);
            }, value, tweenTime).SetId(_id);
        }

        public void SetValueUsingPer(float numerator, float denominator)
        {
            SetValue(numerator/denominator);
        }

        private void OnEnable()
        {
            DOTween.Kill(_id);
        }

        private void ApplyValue(float crtValue)
        {
            _mpb.SetFloat(voronoiValueHash.HashValue, crtValue);
            targetRenderer.SetPropertyBlock(_mpb);
        }
    }
}
