namespace E_Commerce.API.Models.Responses
{
    public class ChangePasswordResult
    {
        public bool Succeeded { get; set; }
        public List<string>? Errors { get; set; }
        public string? Message { get; set; }
    }
}
