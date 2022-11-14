namespace API.Models
{
    //UserDataTransferObject
    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IpAdress { get; set; } = string.Empty;
        public DateTime timeStamp { get; set; }

    }
}
