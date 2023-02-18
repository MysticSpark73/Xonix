namespace Xonix.Core
{
    public interface IPoolable
    {
        virtual void Pool() { }

        virtual void Return() { }
    }
}