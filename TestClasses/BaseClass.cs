
namespace TestClasses
{

    public class BaseClass
    {
        static int s_NextID = 1;

        readonly int _ID = Interlocked.Increment(ref s_NextID);

        public override string ToString()
        {
            return $"{GetType().Name}[{_ID}]";
        }

        public int ID => _ID;

    }

}
