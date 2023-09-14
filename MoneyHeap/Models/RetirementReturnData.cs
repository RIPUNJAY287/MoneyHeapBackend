using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices;

namespace MoneyHeap.Models
{
    public class RetirementReturnData
    {
/*        CREATE TABLE[dbo].[retirementPlans]
        (
    [Id] INT           IDENTITY(1, 1) NOT NULL,
    [email]         VARCHAR(100) NOT NULL,

    [monthlyExpenses] int NOT NULL,
    [startTime] int NOT NULL,
    [endTime] int NOT NULL,
    [risk] VARCHAR(100) NOT NULL,
    [finalAmount]   INT NOT NULL,
	[investmentYearLeft] int NOT NULL,
	[monthlyInvestment] int NOT NULL,
	[inflation] int NOT NULL,
	[returnPercentage] int NOT NULL,
	[fd] int NOT NULL,
	[mf] int NOT NULL,
	[equity] int NOT NULL
    PRIMARY KEY CLUSTERED([Id] ASC)
)*/


        public int Id { get; set; }
        public int userId { get; set; }
        public string? email { get; set; }
        public int monthlyExpenses { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string? risk { get; set; }

        public int finalAmount { get; set; }
        public int investmentTimeLeft { get; set; }
        public int monthlyInvestment { get; set; }
        public int inflation { get; set; }
        public int returnPercentage { get; set; }

        public int fd { get; set; }
        public int mf { get; set; }
        public int equity { get; set; }
    }
}
