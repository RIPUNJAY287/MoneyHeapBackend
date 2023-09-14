namespace MoneyHeap.Models
{
    public class QuestionModel
    {
        public bool lastQuestion { get; set; }

        public int riskAmount { get; set; }
        public int Id { get; set; }
        public string question { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }

        public int? weight1 { get; set; } 
        public int? weight2 { get; set; }
        public int? weight3 { get; set; }
        public int? weight4 { get; set; }


    }
}
