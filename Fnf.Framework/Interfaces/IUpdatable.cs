namespace Fnf.Framework
{
    public interface IUpdatable
    {
        bool isUpdatable { get; set; }

        void Update();
    }
}