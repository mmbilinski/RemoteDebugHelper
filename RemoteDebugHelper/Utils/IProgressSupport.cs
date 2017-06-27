namespace RemoteDebugHelper
{
    internal interface IProgressSupport
    {
        void SetupProgress(int max);
        void SetupProgress(int max, string description);

        void ChangeProgress(int current);
        void ChangeProgress(int current, string description);

    }
}