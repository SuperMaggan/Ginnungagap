namespace Bifrost.Dto.Dto
{
    public class JobProgressDto
    {
        public int Total { get; set; }
        public int Current { get; set; }

        public long GetProgress() => Current / Total;
    }
}