namespace MoneyHeap.Models
{
    public class InvestmentReturnData
    {
        /*    [Id]            INT           IDENTITY (1, 1) NOT NULL,
        [email]         VARCHAR (100) NOT NULL,
        [currentAmount] INT           NOT NULL,
        [startTime]     VARCHAR(100)  NOT NULL,
        [endTime]       VARCHAR(100)  NOT NULL,
        [risk]          VARCHAR (100) NOT NULL,
        [finalAmount]   INT           NOT NULL,
        [investmentTime] int NOT NULL,
        [monthlyExpenses] int NOT NULL,
        [inflation] int NOT NULL,
        [returnPercentage] int NOT NULL,
        [fd] int NOT NULL,
        [mf] int NOT NULL,
        [equity] int NOT NULL*/
        public int Id { get; set; } = 0;
        public int? userId { get; set; } = 0; 
        public string? email { get; set; }
        public int? currentAmount { get;set; } = 0;
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public string? risk{ get; set; }

        public int finalAmount { get; set; }
            public int investmentTime { get; set; }
            public int monthlyInvestment { get; set; }
            public int inflation { get; set; }
            public int returnPercentage { get; set; }
           
            public int fd { get; set; }
            public int mf { get; set; }
            public int equity { get; set; } 
            
    }
}
