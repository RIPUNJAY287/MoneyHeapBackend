namespace MoneyHeap.Models
{
    public class UserRetirementData
    {
        public int Id { get; set; }
        public string? email { get; set; }
        public int monthlyExpenses { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public string? risk { get; set; }
        public int finalAmount { get; set; }
    }
}
