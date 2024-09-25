using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Constants
{
    public class ResourceItemView : MonoBehaviour
    {
        private ColorRenderer _colorRenderer;

        private void Start()
        {
            _colorRenderer = new ColorRenderer(GetComponent<Renderer>());
        }
        
        public void SetColor(Color color)
        {
            _colorRenderer.Color = color;
        }

        #region Factory

        public class Factory : PlaceholderFactory<ResourceItemView>
        {
        }

        #endregion
    }
    
    public class ColorRenderer
    {
        private readonly Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;
        private Color _color;

        public ColorRenderer(Renderer renderer)
        {
            _renderer = renderer;
            _propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propertyBlock);
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            _propertyBlock.SetColor(Constants.Renderer.ResourceColorName, _color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public class ResourceItem
    {
        private readonly ResourceItemView _view;

        public ResourceItem(ResourceItemView.Factory viewFactory)
        {
            _view = viewFactory.Create();
        }
        
        private void Initialise(Settings settings)
        {
            _view.SetColor(settings.Color);
        }

        private void Enable(bool enabled)
        {
            _view.gameObject.SetActive(enabled);
        }

        #region Settings

        [Serializable]
        public struct Settings
        {
            public Color Color;
        }

        #endregion

        #region Pool

        public class Pool : MemoryPool<Settings, ResourceItem>
        {
            protected override void OnCreated(ResourceItem item)
            {
                base.OnCreated(item);
                item.Enable(false);
            }

            protected override void Reinitialize(Settings settings, ResourceItem item)
            {
                item.Initialise(settings);
                item.Enable(true);
            }

            protected override void OnDespawned(ResourceItem item)
            {
                base.OnDespawned(item);
                item.Enable(false);
            }
        }

        #endregion
    }
}