namespace Bifrost.Core.Connectors.States
{
    public enum JobState
    {
        NotDefined,
        Error,
        Discovering,
        InitialCrawling,
        IncrementalCrawling,
        ForeignTable,
        Paused
    }
}