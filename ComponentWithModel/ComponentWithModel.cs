using System;
using Modules.Utils;
using UnityEngine;

namespace Modules.Extensions
{
    public abstract class ComponentModel
    {
    }

    public abstract class ComponentWithModel<TModel> : MonoBehaviour 
        where TModel : ComponentModel
    {
        public TModel Model { get; private set; }
        
        public void Initialize(TModel model, params object[] parameters)
        {
            Model = model;
            OnInitialize(parameters);
        }
        
        public void Release() => Model = null;

        public void UpdateModel(TModel model) => Model = model;

        protected abstract void OnInitialize(params object[] parameters);
    }
    
    public static class ComponentWithModelExtensions
    {
        public static TComponent CreateView<TModel, TComponent>(this TModel model, Func<TComponent,TComponent> create, TComponent prefab, Transform parent, params object[] parameters)
            where TModel : ComponentModel
            where TComponent : ComponentWithModel<TModel>, IPooledGameObject
        {
            var component = create(prefab);
            component.transform.SetParent(parent);
            component.Initialize(model, parameters);
            return component;
        }
        
        public static TComponent CreateView<TModel, TComponent>(this TModel model, Transform parent)
            where TModel : ComponentModel
            where TComponent : ComponentWithModel<TModel>
        {
            var component = new GameObject(typeof(TComponent).Name).AddComponent<TComponent>();
            component.transform.SetParent(parent);
            component.Initialize(model);
            return component;
        }
    }
}