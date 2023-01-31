namespace PlexMatchGenerator.Models
{
    public record ProcessingResults
    {
        public bool Success { get; set; }
        public int RecordsProcessed { get; set; }
    }
}
