using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyHeap.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoneyHeap.Controllers
{
    [ApiController]
    [Route("")]
    public class AdminController : Controller
    {
        private readonly ISqlDatabase sqlDatabase = null;
        SqlConnection conn = null;
        private IConfiguration _configuration = null;
        public AdminController(IConfiguration configuration, ISqlDatabase _sqlDatabase)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            conn = new SqlConnection(_configuration["ConnectionStrings:DatabaseServer"]);
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
        [Authorize(Roles = "Admin")]
        [HttpPost("/addQuestion")]
        public IActionResult AddMCQuestion([FromBody] MCQuestionModel mcquestion)
        {

            for (int i = 0; i < mcquestion.options.Length; i++)
            {
                if (mcquestion.weights[i] < 0 || mcquestion.weights[i] > 100)
                    return BadRequest("invalid input data");
            }

                string query = "INSERT INTO Questionss (question) VALUES ('" + mcquestion.question + "')";
                conn.Open();
                try
                {
                    Console.WriteLine(query);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                        query = "SELECT Id FROM Questionss where question = '" + mcquestion.question+"'";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            int questionId = -1;
                            Console.WriteLine(query);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    questionId = reader.GetInt32(0);
                                }

                            }
                        Console.WriteLine(" interger " + questionId); 
                        for (int index = 0;index <mcquestion.options.Length;index++)
                            {   

                                query = "INSERT INTO QuestionOptions (questionId,optionId,optionO,weightage) VALUES ("+ questionId+ ","+ (index+1) +",'" +
                                    mcquestion.options[index] + "'," + mcquestion.weights[index] + ")";
                            Console.WriteLine(query);
                            using (SqlCommand sqlcmd = new SqlCommand(query,conn))
                                {
                                    sqlcmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound("Not Updated, Try Again");
                }
                conn.Close();
                return Ok("Updated");
           /* }
            return BadRequest();*/
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("/deleteQuestion")]
        public IActionResult DeleteMCQuestion([FromBody] IdModel idModel)
        {

             string query = "DELETE FROM QuestionOptions  WHERE  questionId = " + idModel.Id;
            
            conn.Open();
            try
            {
                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();

                    query = "DELETE FROM Questionss WHERE Id = " + idModel.Id;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                        
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                return NotFound("Not Deleted, Try Again");
            }
            conn.Close();
            return Ok("Deleted");
          
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("/updateAdmin")]
        public IActionResult UpdateAdmin([FromBody]  AdminData adminData)
        {

            //List<UserRetirementData> retirementData = new List<UserRetirementData>();
            var user = GetCurrentUser();
            if (user != null)
            {
                if (adminData.LowRisk < 0 || adminData.LowRisk > 100 || adminData.MidRisk < 0 || adminData.MidRisk > 100 || adminData.HighRisk < 0 || adminData.HighRisk > 100)
                    return Ok("invalid input data");


                string query = "UPDATE AdminData SET LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
                conn.Open();
                try
                {
                    Console.WriteLine(query);
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return NotFound("Not Updated, Try Again");
                }
                conn.Close();
                return Ok("Updated");
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getAdminData")]

        public IActionResult GetAdminData()
        {

            //List<UserRetirementData> retirementData = new List<UserRetirementData>();
            var user = GetCurrentUser();
            if (user != null)
            {
                AdminData adminData = new AdminData();
                //string query = "UPDATE AdminData LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
                string query = "SELECT * From AdminData where Id = 0";
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
                    conn.Close();
                    return NotFound("Error while Fetching the data");
                }
                conn.Close();
                return Ok(JsonConvert.SerializeObject(adminData));
            }
            return BadRequest();
        }

    


    [Authorize(Roles = "Admin")]
    [HttpGet("/getAllQuestions")]
    public IActionResult GetAllQuestions()
    {

        //List<UserRetirementData> retirementData = new List<UserRetirementData>();
        var user = GetCurrentUser();
        if (user != null)
        {
         
               // QuestionModel? questionModel = null;
                List<QuestionModel> questionList = new List<QuestionModel>();
                //string query = "UPDATE AdminData LowRiskReturn = " + adminData.LowRisk + ", MidRiskReturn = " + adminData.MidRisk + " , HighRiskReturn = " + adminData.HighRisk + ", Inflation = " + adminData.Inflation + " where Id = 0";
                string query = "SELECT * From Questionss JOIN QuestionOptions ON Questionss.Id = QuestionOptions.questionId" ;
            conn.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    Console.WriteLine(query);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int questionId = -1;
                            QuestionModel? questionModel = null;
                            int flag = -1;
                        while (reader.Read())
                        {

                                
                                int id = reader.GetInt32(0);
                                if(id != questionId) 
                                {
                                    if (flag != -1)
                                    {
                                        questionList.Add(questionModel);
                                    }
                                    questionId = id;
                                    questionModel = new QuestionModel();
                                    questionModel.Id = id;
                                    questionModel.question = reader.GetString(1);
                                   
                                    flag = 0;
                                    

                                }
                                if (questionModel.option1 == null)
                                {
                                    questionModel.option1 = reader.GetString(5);
                                    questionModel.weight1 = reader.GetInt32(6);

                                   
                                }
                                else if (questionModel.option2 == null)
                                {
                                    questionModel.option2 = reader.GetString(5);
                                    questionModel.weight2 = reader.GetInt32(6);

                                }
                                else if (questionModel.option3 == null)
                                {
                                    questionModel.option3 = reader.GetString(5);
                                    questionModel.weight3 = reader.GetInt32(6);

                                }
                                else 
                                {
                                    questionModel.option4 = reader.GetString(5);
                                    questionModel.weight4 = reader.GetInt32(6);

                                }



                            }
                            if (flag != -1)
                            {
                                questionList.Add(questionModel);
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                return NotFound(questionList);
            }
            conn.Close();
            return Ok(JsonConvert.SerializeObject(questionList));
        }
        return BadRequest();
    }

}
}
