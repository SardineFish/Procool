using UnityEngine;

namespace Procool.Map.Loader
{
    public abstract class ContentLoaderBase : MonoBehaviour
    {
        public abstract void Load(BlockContent content);
        public abstract void Unload();
    }
    public abstract class ContentLoader<T> : ContentLoaderBase where T : BlockContent
    {
        protected abstract void Load(T content);

        public override void Load(BlockContent content)
        {
            Load(content as T);
        }
        
        
    }
}