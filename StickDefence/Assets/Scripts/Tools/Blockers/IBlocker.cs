namespace Tools.Blockers
{
    public interface IBlocker
    {
        void Block(bool useFade = false);
        void Unblock();
    }
}