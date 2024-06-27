namespace OpenAPI.Ordering
{
    public class ComputationResult
    {
        public int Id { get; set; }
        public string TaskId { get; set; }
        public decimal? Result { get; set; }
        public ComputationResultStatus Status { get; set; }
    }
}
