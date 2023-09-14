using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneyHeap.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Xml.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MoneyHeap.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController : Controller
    {
        private readonly ISqlDatabase sqlDatabase = null;
        SqlConnection conn = null;
        private IConfiguration _configuration = null;
        public UserController(IConfiguration configuration, ISqlDatabase _sqlDatabase)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            conn = new SqlConnection(_configuration["ConnectionStrings:DatabaseServer"]);
        }

        
        [Authorize]
        [HttpPost("/nextquestions")]
        public IActionResult GetNextQuestions([FromBody] QueResponse queResponse)
        {
            var user = GetCurrentUser();
            QuestionModel? questionModel = null;
            if (user != null)
            {
                conn.Open();
                try
                {   
                    UserRiskAnswer(user,queResponse);
                    string query = "select COUNT(*) from userQueAns WHERE userId = '"+user.Id+"'";
                    Console.WriteLine(query);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int count = (Int32)command.ExecuteScalar();
                        Console.WriteLine(count);
                        if (count < 4)
                        {
                            questionModel = NextQuestions(count + 1);
                        }
                        else
                        {
                            
                            questionModel = new QuestionModel();
                            questionModel.lastQuestion = true;
                            RiskAmount riskCal = GetRiskAmount(user);
                            questionModel.riskAmount = riskCal.riskAmount;
                            return Ok(JsonConvert.SerializeObject(questionModel));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(null);
                }
                conn.Close();
                if (questionModel != null)
                    return Ok(JsonConvert.SerializeObject(questionModel));
                return NotFound(null);
            }
            return BadRequest(null);
        }

        

        [Authorize]
        [HttpGet("/questions")]
        public IActionResult GetQuestions()
        {
            var user = GetCurrentUser();
            QuestionModel? questionModel = null;
            if (user != null)
            {
                conn.Open();
                try
                {
                    string query = "select COUNT(*) from userQueAns WHERE userId = '" + user.Id + "'";
                    Console.WriteLine(query);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int count = (Int32)command.ExecuteScalar();
                        Console.WriteLine(count);
                        if (count < 4)
                        {   
                            questionModel = NextQuestions(count+1);
                        }
                        else
                        {
                            questionModel = new QuestionModel();
                            questionModel.lastQuestion = true;
                            RiskAmount riskCal = GetRiskAmount(user);
                            questionModel.riskAmount = riskCal.riskAmount;

                            return Ok(JsonConvert.SerializeObject(questionModel));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(null);
                }
                conn.Close();
                if (questionModel != null)
                    return Ok(JsonConvert.SerializeObject(questionModel));
                return NotFound(null);
            }
            return BadRequest(null);
        }



        [Authorize]
        [HttpPost("/nextques")]
        public IActionResult GetNextQues([FromBody] QueResponse queResponse)
        {
            var user = GetCurrentUser();
            QuestionModel? questionModel = null;
            if (user != null)
            {
             //   QuestionModel? questionModel = null;
              //  List<QuestionModel> questionList = new List<QuestionModel>();
                //string query = "UPDATE AdminData LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
                string query = "select COUNT(DISTINCT questionId) from userQueAns WHERE userId  = " + user.Id;
                //   query = "SELECT * From Questionss JOIN QuestionOptions ON Questionss.Id = QuestionOptions.questionId";


                conn.Open();
                try
                {
                    UserRiskAns(user, queResponse);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int count = (Int32)command.ExecuteScalar();
                        query = "select COUNT(*) from Questionss";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            int totalQuestion = (Int32)cmd.ExecuteScalar();
                            Console.WriteLine(totalQuestion + "  " + count);

                            if (count < totalQuestion)
                            {
                                questionModel = NextQues(user);
                            }
                            else
                            {
                                questionModel = new QuestionModel();
                                questionModel.lastQuestion = true;
                                RiskAmount riskCal = GetRiskAmount(user);
                                questionModel.riskAmount = riskCal.riskAmount;

                                return Ok(JsonConvert.SerializeObject(questionModel));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(questionModel);
                }
                conn.Close();
                return Ok(JsonConvert.SerializeObject(questionModel));
            }
            return BadRequest(null);
        }

        [Authorize]
        [HttpGet("/ques")]
        public IActionResult CheckQuestion()
        {
            //List<UserRetirementData> retirementData = new List<UserRetirementData>();
            var user = GetCurrentUser();
            if (user != null)
            {

                QuestionModel? questionModel = null;
                List<QuestionModel> questionList = new List<QuestionModel>();
                //string query = "UPDATE AdminData LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
                string query = "select COUNT(DISTINCT questionId) from userQueAns WHERE userId  = " + user.Id;
                //   query = "SELECT * From Questionss JOIN QuestionOptions ON Questionss.Id = QuestionOptions.questionId";


                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int count = (Int32)command.ExecuteScalar();
                        query = "select COUNT(*) from Questionss";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            int totalQuestion = (Int32)cmd.ExecuteScalar();
                            Console.WriteLine(totalQuestion +  "  " + count);
                            if (count < totalQuestion)
                            {
                                questionModel = NextQues(user);
                            }
                            else
                            {
                                questionModel = new QuestionModel();
                                questionModel.lastQuestion = true;
                                RiskAmount riskCal = GetRiskAmount(user);
                                questionModel.riskAmount = riskCal.riskAmount;

                                return Ok(JsonConvert.SerializeObject(questionModel));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(questionModel);
                }
                conn.Close();
                return Ok(JsonConvert.SerializeObject(questionModel));
            }
            return BadRequest();
        }


        [Authorize]
        [HttpPost("/checkInvest")]
        public IActionResult CheckInvestPlan([FromBody] InvestmentForm investmentForm)
        {
            
            var user = GetCurrentUser();
            Console.WriteLine(user.Email);
            if (user != null)
            {  
                InvestmentReturnData investmentReturnData = new InvestmentReturnData();
                conn.Open();
                try
                {
                    int currentAmountt = int.Parse(investmentForm.CurrentAmount);
                   
                   
                    int startdate = Convert.ToInt32(DateTime.Now.Day.ToString());
                    int startMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
                    int startYear = Convert.ToInt32(DateTime.Now.Year.ToString());

                    string[] datetime = investmentForm.DateTime.Split('-');
                    int endDate = Convert.ToInt32(datetime[2]);
                    int endMonth = Convert.ToInt32(datetime[1]);
                    int endYear  = Convert.ToInt32(datetime[0]);
                    Console.WriteLine(endDate + "  " + endMonth+" " + endYear);

                    DateTime startDateTime = new DateTime(startYear, startMonth, startdate);
                    DateTime endDateTime = new DateTime(endYear, endMonth, endDate);

                    int totalMonth = GetMonthDifference(startDateTime, endDateTime);
               
                    int years = totalMonth / 12;

                    RiskAmount riskCal = GetRiskAmount(user);
                    string risk = riskCal.risk;

                    AdminData adminData = GetAdminData();
                   
                    double inflation = adminData.Inflation / (100*1.0);
                    double lowriskreturn = adminData.LowRisk / (100 * 1.0);
                    double midriskreturn = adminData.MidRisk / (100 * 1.0);
                    double highriskreturn = adminData.HighRisk / (100 * 1.0);
                    Console.WriteLine(inflation + "  " + lowriskreturn + "  " + midriskreturn + "  " + highriskreturn + "  lll ");

                    /*                    Console.WriteLine( inflation + " inflatoin " + lowriskreturn + "  " + midriskreturn + " sd " + highriskreturn);
                                        Console.WriteLine(adminData.Inflation + " inflatoin " + adminData.LowRisk + "  " + adminData.MidRisk + " sd " + adminData.HighRisk);
                    */
                    double ret = (risk == "LowRisk" ? lowriskreturn : (risk == "MidRisk" ? midriskreturn : highriskreturn));

                    double adj = ((1 + ret) / ((1 + inflation))*1.0) - 1;

                    Console.WriteLine(adj);
                    double FV = currentAmountt * (Math.Pow((1 + adj), years));
                    double ypmt = FV / (((Math.Pow((1 + adj), years) - 1) / adj) * (1 + adj));
                    double mpmt = ypmt/12;
                    

                    string mn = startMonth < 10 ? "0" + startMonth : startMonth.ToString();
                    string dy = startdate <10 ? "0" + startdate : startdate.ToString() ;
                    string startDT = startYear + "-" + mn + "-" + dy;

                    inflation = inflation * 100;
                    ret = ret * 100;
                /*    investmentReturnData.userId = user.Id;
                    investmentReturnData.email = user.Email;
                    investmentReturnData.currentAmount = currentAmountt;
                    investmentReturnData.startTime = startDT;
                    investmentReturnData.endTime = investmentForm.DateTime;
                    investmentReturnData.risk = risk;
                    investmentReturnData.finalAmount = (int)FV;
                    investmentReturnData.investmentTime = (int) totalMonth;
                    investmentReturnData.monthlyExpenses = (int) mpmt;
                    investmentReturnData.inflation = (int) (inflation);
                    investmentReturnData.returnPercentage = (int)(ret);
                    investmentReturnData.fd = riskCal.FD;
                    investmentReturnData.mf = riskCal.MF;
                    investmentReturnData.equity = riskCal.Equity;*/

                    string query = "INSERT INTO investmentPlansSuggested (userId, currentAmount, startTime, endTime,risk, finalAmount,investmentTime,monthlyExpenses,inflation,returnPercentage,fd,mf,equity) " +
                             " VALUES(" + user.Id + "," + investmentForm.CurrentAmount + ",'" + startDT + "','" + investmentForm.DateTime + "','" + risk + "', " + FV + ","+ totalMonth + ","
                                + mpmt + "," + inflation + "," + ret + "," + riskCal.FD  + "," + riskCal.MF + "," + riskCal.Equity + ")";
                   
                    Console.WriteLine(query);

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT TOP 1 * FROM investmentPlansSuggested where userId = " + user.Id + " ORDER BY Id DESC";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            Console.WriteLine(query);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    investmentReturnData.Id = reader.GetInt32(0);
                                    investmentReturnData.userId = reader.GetInt32(1);
                                    investmentReturnData.email = user.Email; // it is not in database
                                    investmentReturnData.currentAmount = reader.GetInt32(2);
                                    investmentReturnData.startTime = reader.GetString(3);
                                    investmentReturnData.endTime = reader.GetString(4);
                                    investmentReturnData.risk = reader.GetString(5);
                                    investmentReturnData.finalAmount = reader.GetInt32(6);
                                    investmentReturnData.investmentTime = reader.GetInt32(7);
                                    investmentReturnData.monthlyInvestment = reader.GetInt32(8);
                                    investmentReturnData.inflation = reader.GetInt32(9);
                                    investmentReturnData.returnPercentage = reader.GetInt32(10);
                                    investmentReturnData.fd = reader.GetInt32(11);
                                    investmentReturnData.mf = reader.GetInt32(12);
                                    investmentReturnData.equity = reader.GetInt32(13);

                                }
                            }

                        }
                    }
                
                        
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(false);
                }
                conn.Close();
                Console.WriteLine(JsonConvert.SerializeObject(investmentReturnData));
                return Ok(JsonConvert.SerializeObject(investmentReturnData));
            }
            return BadRequest(false);
        }
        private User? GetCurrentUser()
        {
            // var identity = HttpContext.User.Identity as ClaimsIdentity;
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            Console.WriteLine(identity);
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var Name = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Name)?.Value;
                var Email = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Email)?.Value;
                var Role = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Role)?.Value;
                var Id = userClaims.FirstOrDefault(v => v.Type == ClaimTypes.Sid)?.Value;
                Console.WriteLine(Name + " " + Email + " " + Role);
                User usr = new User();
                usr.Name = Name;
                usr.Email = Email;
                usr.Role = Role;
                usr.Id = int.Parse(Id);
                return usr;
            }
            return null;
        }
        public  int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        [Authorize]
        [HttpPost("/submitInvestment")]
        public IActionResult AddInvestmentPlan([FromBody] SubmitInvestment submitInvestment)
        {
            var user = GetCurrentUser();

            if (user != null)
            {
                InvestmentReturnData returnData = new InvestmentReturnData();
                conn.Open();
                try
                {
                    Console.WriteLine("submit investment Call");

                    string query = "SELECT TOP 1 Id,userId FROM investmentPlansSuggested where userId = " + user.Id + " ORDER BY Id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int planId = 0;
                        int userId = 0;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                planId = reader.GetInt32(0);
                                userId = reader.GetInt32(1);


                            }

                        }
                   
                        query = "INSERT INTO InvestPlans (userId, investmentPlanId) VALUES (" + userId + "," + planId +")";
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                       
                    }

                }   
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound("Plan Not Added");
                }
                conn.Close();
                return Ok("Plan Added");
            }
            return BadRequest(false);
        }


        [Authorize]
        [HttpPost("/checkRetirement")]
        public IActionResult CheckRetirementPlan([FromBody] RetirementForm retirementForm)
        {
            var user = GetCurrentUser();
            
            if (user != null)
            {
                RetirementReturnData retirementReturnData = new RetirementReturnData();
                conn.Open();
                try
                {

                    int startdate = Convert.ToInt32(DateTime.Now.Day.ToString());
                    int startMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
                    int startYear = Convert.ToInt32(DateTime.Now.Year.ToString());
                    string mn = startMonth < 10 ? "0" + startMonth : startMonth.ToString();
                    string dy = startdate < 10 ? "0" + startdate : startdate.ToString();
                    string startDT = startYear + "-" + mn + "-" + dy;

                    string[] datetime = retirementForm.RetirementTime.Split('-');
                    int endDate = Convert.ToInt32(datetime[2]);
                    int endMonth = Convert.ToInt32(datetime[1]);
                    int endYear = Convert.ToInt32(datetime[0]);

                    Console.WriteLine("jemllleoeeoe" + retirementForm.MonthlyExpenses + "  " + user.Email);
                    DateTime startDateTime = new DateTime(startYear, startMonth, startdate);
                    DateTime endDateTime = new DateTime(endYear, endMonth, endDate);

                    int totalMonth = GetMonthDifference(startDateTime, endDateTime);
                    // int startYear = Convert.ToInt32(DateTime.Now.Year.ToString());

                    int retirementYear = endYear - startYear;
                    int retirementPeriod = retirementForm.ExpectedPeriod;

                    RiskAmount riskCal = GetRiskAmount(user);
                    string risk = riskCal.risk;

                    //  double inflation = 0.06;

                    //double ret = (risk=="LowRisk" ? 0.08 : (risk=="MidRisk" ? 0.11 : 0.14));

                    AdminData adminData = GetAdminData();

                    double inflation = adminData.Inflation / (100 * 1.0);
                    double lowriskreturn = adminData.LowRisk / (100 * 1.0);
                    double midriskreturn = adminData.MidRisk / (100 * 1.0);
                    double highriskreturn = adminData.HighRisk / (100 * 1.0);

                 
                    double ret = (risk == "LowRisk" ? lowriskreturn : (risk == "MidRisk" ? midriskreturn : highriskreturn));

                    Console.WriteLine(inflation + "  " + lowriskreturn + "  " + midriskreturn + "  " + highriskreturn + "  lll "); 

                    double adj = ((1 + ret) / (1 + inflation) * 1.0) - 1;

                    double FV = retirementForm.MonthlyExpenses * (Math.Pow((1 + inflation), retirementYear));
                    double earexp = FV * 12;

                    double retcorp = earexp * ((1 - (1 / ((Math.Pow((1 + adj), (retirementPeriod - 1)))))) / adj) + earexp;
              
                    double ypmt = retcorp / (((Math.Pow((1 + adj), retirementYear) - 1) / adj) * (1 + adj));
                    Console.WriteLine();
                    Console.WriteLine(ypmt  + "  " + FV + " " + retcorp);
                    Console.WriteLine();
                    double mpmt = ypmt / 12;   

                    inflation = inflation * 100;
                    ret = ret * 100;
                /*    var returnData1 = new
                    {
                        totalAmount = FV,
                        retirementYearLeft = retirementYear,
                        monthlyExpenses = mpmt,
                        inflation = inflation,
                        returnPercentage =ret,
                        riskProfile = risk,
                        fd = riskCal.FD,
                        mf = riskCal.MF,
                        equity = riskCal.Equity,
                        email = user.Email,
                        period = retirementForm.ExpectedPeriod

                    };*/

                    String query = "INSERT INTO retirementPlansSuggested (userId, monthlyExpenses, startTime, endTime, risk, finalAmount,investmentTimeLeft,monthlyInvestment,inflation,returnPercentage,fd,mf,equity) " +
                             " VALUES (" + user.Id + "," + retirementForm.MonthlyExpenses + ",'" + startDT + "','" + retirementForm.RetirementTime + "', '" + risk + "', " + retcorp + ","
                              + totalMonth + "," + mpmt + "," + + inflation + "," + ret + "," + riskCal.FD + "," + riskCal.MF + "," + riskCal.Equity+")";

                    Console.WriteLine(query);

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();

                        query = "SELECT TOP 1 * FROM retirementPlansSuggested where userId = " + user.Id + " ORDER BY Id DESC";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            Console.WriteLine(query);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    retirementReturnData.Id = reader.GetInt32(0);
                                    retirementReturnData.userId = reader.GetInt32(1);
                                    retirementReturnData.email = user.Email; // it is not in database
                                    retirementReturnData.monthlyExpenses = reader.GetInt32(2);
                                    retirementReturnData.startTime = reader.GetString(3);
                                    retirementReturnData.endTime = reader.GetString(4);
                                    retirementReturnData.risk = reader.GetString(5);
                                    retirementReturnData.finalAmount = reader.GetInt32(6);
                                    retirementReturnData.investmentTimeLeft = reader.GetInt32(7);
                                    retirementReturnData.monthlyInvestment = reader.GetInt32(8);
                                    retirementReturnData.inflation = reader.GetInt32(9);
                                    retirementReturnData.returnPercentage = reader.GetInt32(10);
                                    retirementReturnData.fd = reader.GetInt32(11);
                                    retirementReturnData.mf = reader.GetInt32(12);
                                    retirementReturnData.equity = reader.GetInt32(13);

                                }
                            }

                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(false);
                }
                conn.Close();
                return Ok(JsonConvert.SerializeObject(retirementReturnData));
            }
            return BadRequest(false);
        }

        [Authorize]
        [HttpPost("/submitRetirement")]
        public IActionResult AddRetirementPlan([FromBody] SubmitRetirement submitRetirement)
        {
            var user = GetCurrentUser();

            if (user != null)
            {
               // RetirementReturnData returnData = new RetirementReturnData();
                conn.Open();
                try
                {
                   


                    string query = "SELECT TOP 1 Id,userId FROM retirementPlansSuggested where userId = " + user.Id + " ORDER BY Id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        int planId = 0;
                        int userId = 0;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                planId = reader.GetInt32(0);
                                userId = reader.GetInt32(1);
                            }

                        }
                        query = "INSERT INTO RetirePlans (userId, retirementPlanId) VALUES (" + userId + "," + planId + ")";
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.ExecuteNonQuery();
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound("Plan Not Added");
                }
                conn.Close();
                return Ok("Plan Added");
            }
            return BadRequest(false);
        }

        [Authorize]
        [HttpGet("/getAllPlans")]
        public IActionResult GetAllPlans()
        {
            var user = GetCurrentUser();
            List<PlanItems> plans = new List<PlanItems>();
            if (user != null)
            {
                conn.Open();
                try
                {

                    List<UserRetirementData> userRetirementPlans =  getAllRetirementPlans(user);
                    List<UserInvestmentData> userInvestmentPlans =  getAllInvestmentPlans(user);
                    foreach (UserRetirementData plan in userRetirementPlans)
                    {   PlanItems pi = new PlanItems();
                        pi.Id = plan.Id;
                        pi.Name = "Retirement"; ;
                        pi.startTime = plan.startTime.ToString();
                        pi.endTime = plan.endTime.ToString();
                        pi.finalAmount = plan.finalAmount;
                        plans.Add(pi);
                    }
                    foreach (UserInvestmentData plan in userInvestmentPlans)
                    {
                        PlanItems pi = new PlanItems();
                        pi.Id = plan.Id;
                        pi.Name = "Investment";
                        pi.startTime = plan.startTime;
                        pi.endTime = plan.endTime;
                        pi.finalAmount = plan.finalAmount;
                        plans.Add(pi);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(null);
                }
                conn.Close();
                return Ok(plans);
            }
            return BadRequest(null);
        }
        public List<UserRetirementData> getAllRetirementPlans(User user)
        {

            List<UserRetirementData> retirementData = new List<UserRetirementData>();
            string query = "select * from retirementPlansSuggested JOIN RetirePlans ON retirementPlansSuggested.Id = RetirePlans.retirementPlanId where RetirePlans.userId = " + user.Id; 
            Console.WriteLine("query " + query);
            try
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserRetirementData userData = new UserRetirementData();
                            /* Console.WriteLine(reader.GetInt32(0));
                             Console.WriteLine(reader.GetString(1));
                             Console.WriteLine(reader.GetInt32(2));
                             Console.WriteLine(reader.GetInt32(3));
                             Console.WriteLine(reader.GetString(1));
                             Console.WriteLine(reader.GetString(1));
                             Console.WriteLine(reader.GetString(1));
                            CREATE TABLE [dbo].[retirementPlans] (

                             */
                            userData.Id = reader.GetInt32(0);
                            //userData.email = reader.GetString(1);
                            userData.monthlyExpenses = reader.GetInt32(2);
                            userData.startTime = reader.GetString(3);
                            userData.endTime = reader.GetString(4);
                            userData.risk = reader.GetString(5);
                            userData.finalAmount = reader.GetInt32(6);
                            retirementData.Add(userData);

                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message); 
                return new List<UserRetirementData>();
            }

            return retirementData;

        }

        [Authorize]
        [HttpPost("getRetirementById")]
        public IActionResult getRetirementById(SubmitRetirement submitRetirement)
        {

            //List<UserRetirementData> retirementData = new List<UserRetirementData>();
            var user = GetCurrentUser();
            if (user != null)
            {
                string query = "select * from retirementPlansSuggested where id = " + submitRetirement.Id;

                RetirementReturnData retirementReturnData = new RetirementReturnData();
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                             //   RetirementReturnData userData = new RetirementReturnData();

                                retirementReturnData.Id = reader.GetInt32(0);
                                retirementReturnData.userId = reader.GetInt32(1);
                                retirementReturnData.email = user.Email; // it is not in database
                                retirementReturnData.monthlyExpenses = reader.GetInt32(2);
                                retirementReturnData.startTime = reader.GetString(3);
                                retirementReturnData.endTime = reader.GetString(4);
                                retirementReturnData.risk = reader.GetString(5);
                                retirementReturnData.finalAmount = reader.GetInt32(6);
                                retirementReturnData.investmentTimeLeft = reader.GetInt32(7);
                                retirementReturnData.monthlyInvestment = reader.GetInt32(8);
                                retirementReturnData.inflation = reader.GetInt32(9);
                                retirementReturnData.returnPercentage = reader.GetInt32(10);
                                retirementReturnData.fd = reader.GetInt32(11);
                                retirementReturnData.mf = reader.GetInt32(12);
                                retirementReturnData.equity = reader.GetInt32(13);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(retirementReturnData);
                }
                conn.Close();
                return Ok(retirementReturnData);
            }
            return BadRequest();
        }
        public List<UserInvestmentData> getAllInvestmentPlans(User user)
        {

            List<UserInvestmentData> investmentData = new List<UserInvestmentData>();
            string query = "select * from investmentPlansSuggested JOIN InvestPlans ON investmentPlansSuggested.Id = InvestPlans.investmentPlanId where InvestPlans.userId = " + user.Id;
            try
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserInvestmentData userData = new UserInvestmentData();
                            userData.Id = reader.GetInt32(0);
                            userData.currentAmount = reader.GetInt32(2);
                            userData.startTime = reader.GetString(3);
                            userData.endTime = reader.GetString(4);
                            userData.finalAmount = reader.GetInt32(6);
                            investmentData.Add(userData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UserInvestmentData>();
            }

            return investmentData;

        }
        [Authorize]
        [HttpPost("getInvestmentById")]
        public IActionResult getInvestmentById(SubmitInvestment submitInvestment)
        {

            //List<UserRetirementData> retirementData = new List<UserRetirementData>();
            var user = GetCurrentUser();
            if (user != null)
            {
                string query = "select * from investmentPlansSuggested where id = " + submitInvestment.Id;

                InvestmentReturnData investmentReturnData = new InvestmentReturnData();
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                investmentReturnData.Id = reader.GetInt32(0);
                                investmentReturnData.userId = reader.GetInt32(1);
                                investmentReturnData.email = user.Email; // it is not in database
                                investmentReturnData.currentAmount = reader.GetInt32(2);
                                investmentReturnData.startTime = reader.GetString(3);
                                investmentReturnData.endTime = reader.GetString(4);
                                investmentReturnData.risk = reader.GetString(5);
                                investmentReturnData.finalAmount = reader.GetInt32(6);
                                investmentReturnData.investmentTime = reader.GetInt32(7);
                                investmentReturnData.monthlyInvestment = reader.GetInt32(8);
                                investmentReturnData.inflation = reader.GetInt32(9);
                                investmentReturnData.returnPercentage = reader.GetInt32(10);
                                investmentReturnData.fd = reader.GetInt32(11);
                                investmentReturnData.mf = reader.GetInt32(12);
                                investmentReturnData.equity = reader.GetInt32(13);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound(investmentReturnData);
                }
                conn.Close();
                return Ok(investmentReturnData);
            }
            return BadRequest();
        }
        private IActionResult UserRiskAnswer(User user, QueResponse queResponse)
        {

            int mark = 0;
            Console.WriteLine(queResponse.answerId);
            Console.WriteLine(queResponse.answerId.GetType());

            if (queResponse.answerId == 1)
            {
                mark = 0;
            }
            else if (queResponse.answerId == 2)
            {
                mark = 30;
            }
            else if (queResponse.answerId == 3)
            {
                mark = 60;
            }
            else
            {
                mark = 100;
            }

            // Console.WriteLine("Signup :  " + signupUser.name + " " + signupUser.email + "  " + signupUser.password);
            try
            {
                String query = "INSERT INTO userQueAns (userId, questionId,answerMark) " +
                             " VALUES('" + user.Id + "'," + queResponse.questionId + "," + mark + ")";
                Console.WriteLine(query);


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(false);
            }
            return Ok(true);

        }
        public QuestionModel? NextQuestions(int count)
        {
            QuestionModel? questionModel = null;
            /// conn.Open();
            try
            {

                string query = $"select * from Questions where id = {count}";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questionModel = new QuestionModel();
                            questionModel.lastQuestion = false;
                            questionModel.Id = reader.GetInt32(0);
                            questionModel.question = reader.GetString(1);
                            questionModel.option1 = reader.GetString(2);
                            questionModel.option2 = reader.GetString(3);
                            questionModel.option3 = reader.GetString(4);
                            questionModel.option4 = reader.GetString(5);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            if (questionModel != null)
                return questionModel;
            return null;
        }

        private IActionResult UserRiskAns(User user, QueResponse queResponse)
        {



            // Console.WriteLine("Signup :  " + signupUser.name + " " + signupUser.email + "  " + signupUser.password);
            try
            {
                string query = "SELECT SUM(weightage) from QuestionOptions where questionId = " + queResponse.questionId + " AND optionId = " + queResponse.answerId;


                Console.WriteLine(query);


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    int mark = (Int32)command.ExecuteScalar();
                    query = "INSERT INTO userQueAns (userId, questionId,answerMark) " +
                           " VALUES('" + user.Id + "'," + queResponse.questionId + "," + mark + ")";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(false);
            }
            return Ok(true);

        }

        public QuestionModel? NextQues(User user)
        {
            //  QuestionModel? questionModel = null;
            /// conn.Open();
            QuestionModel? questionModl = null;
            try
            {

                string query = "Select * from Questionss JOIN QuestionOptions ON  Questionss.Id = QuestionOptions.questionId WHERE Questionss.Id = (SELECT TOP 1 Id FROM Questionss WHERE Id NOT IN (SELECT Questionss.Id from Questionss JOIN userQueAns ON Questionss.Id = userQueAns.questionId WHERE userId =" + user.Id + "));";
              

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int questionId = -1;

                       
                        while (reader.Read())
                        {
                           
                            int id = reader.GetInt32(0);
                            if (id != questionId)
                            {

                                questionId = id;
                                questionModl = new QuestionModel();
                                questionModl.Id = id;
                                questionModl.question = reader.GetString(1);
                        
                            }
                            if (questionModl.option1 == null)
                            {
                                questionModl.option1 = reader.GetString(5);
                            }
                            else if (questionModl.option2 == null)
                            {
                                questionModl.option2 = reader.GetString(5);
                            }
                            else if (questionModl.option3 == null)
                            {
                                questionModl.option3 = reader.GetString(5);
                            }
                            else
                            {
                                questionModl.option4 = reader.GetString(5);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
           // Console.WriteLine(questionModl.Id  + "  " + questionModl.question + " " + questionModl.option1);
            if (questionModl != null)
                return questionModl;
            return null;
        }


        public AdminData GetAdminData()
        {

            //List<UserRetirementData> retirementData = new List<UserRetirementData>();

            AdminData adminData = new AdminData();
            //string query = "UPDATE AdminData LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
            string query = "SELECT * From AdminData where Id = 0";
            
            try
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            adminData.LowRisk = reader.GetInt32(1);
                            adminData.MidRisk = reader.GetInt32(2);
                            adminData.HighRisk = reader.GetInt32(3);
                            adminData.Inflation = reader.GetInt32(4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return adminData;
            }
            return adminData;
        }
        public RiskAmount GetRiskAmount(User user)
         {
        string query = "select SUM(answerMark) from userQueAns WHERE userId = '" + user.Id + "'";
        Console.WriteLine(query);
            RiskAmount riskAm  =new RiskAmount();
        using (SqlCommand command = new SqlCommand(query, conn))
        {
            int riskAmount = (Int32)command.ExecuteScalar();
            Console.WriteLine("amaount " + riskAmount);
            string risk = ((riskAmount >= 0 && riskAmount < 200) ? "LowRisk" : ((riskAmount >= 200 && riskAmount < 450) ? "MidRisk" : "HighRisk"));
            int fd =  (risk == "LowRisk" ? 50 : (risk == "MidRisk" ?  34 : 20));
            int mf = (risk == "LowRisk" ? 30 : (risk == "MidRisk" ? 33 : 33));
            int equity = (risk == "LowRisk" ? 20 : (risk == "MidRisk" ? 30 : 50));
            riskAm.riskAmount = riskAmount;
                riskAm.risk = risk;
                riskAm.FD = fd;
                riskAm.MF = mf;
                riskAm.Equity = equity;
        }
            return riskAm;
       }


    }
}
