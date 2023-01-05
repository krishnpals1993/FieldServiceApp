namespace LaCafelogy.Models
{
    public class ApiResponseModel
    {
        public string ReturnStatus { get; set; }
        public string ReturnMessage { get; set; }
        
       
    }


    public class LoginApiResponseModel : ApiResponseModel
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }


    }

    public class LoginApiModel : ApiResponseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
       


    }

}