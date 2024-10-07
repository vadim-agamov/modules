using UnityEngine;

namespace Modules.Extensions
{
    public abstract class ComponentWithModel<TModel> : MonoBehaviour
    {
        protected TModel Model { get; private set; }
        public void Initialize(TModel model) => Model = model;
        protected abstract void OnInitialize();
    }
    
    public static class GameObjectInstantiator
    {
        public static TComponent InstantiateComponentWithModel<TModel, TComponent>(this TComponent prefab, TModel model, Transform parent)
            where TComponent : ComponentWithModel<TModel>
        {
            var component = Object.Instantiate(prefab, parent);
            component.Initialize(model);
            return component;
        }
    }
}