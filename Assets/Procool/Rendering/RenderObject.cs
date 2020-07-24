using UnityEngine.Rendering;

namespace Procool.Rendering
{
    public abstract class RenderObject
    {
        public abstract void Init();
        public abstract void Update();
        public abstract void CleanUp();
    }
}