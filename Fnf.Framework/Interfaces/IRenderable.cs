namespace Fnf.Framework
{
    public interface IRenderable
    {
        bool isRenderable { get; set; }

        void Render();
    }
}