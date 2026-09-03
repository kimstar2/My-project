using System;
using _01.Scripts.UI.Mono;
using _TevLib.HashDataSystem;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace _01.Scripts.UI
{
    public class UIMaterialSetter : MonoBehaviour
    {
        [SerializeField] private MonoImage targetImage;
        [SerializeField] private ShaderHashSO shaderHash;
        [SerializeField] private float duration;
        [SerializeField] private Ease easeType;
        [SerializeField] private Transform idTrm;
        private Material _defaultMaterial;
        private Material _materialInstance;
        private int _id;
        
        private void Awake()
        {
            _id = idTrm.GetHashCode();
        }

        private void Start()
        {
            _defaultMaterial = targetImage.Image.material;
            _materialInstance = new Material(_defaultMaterial);
            targetImage.SetMaterial(_materialInstance);
        }

        public void SetIntensity(float intensity)
        {
            DOTween.Kill(idTrm);

            float crtIntensity = _materialInstance.GetFloat(shaderHash.HashValue);
            DOTween.To(() => crtIntensity, x =>
            {
                crtIntensity = x;
                SetAmount(crtIntensity);
            }, intensity, duration).SetId(_id).
                SetEase(easeType)
                .SetUpdate(true);
        }

        private void SetAmount(float crtIntensity)
        {
            _materialInstance.SetFloat(shaderHash.HashValue, crtIntensity);
            targetImage.Image.SetMaterialDirty();
        }
    }
}
