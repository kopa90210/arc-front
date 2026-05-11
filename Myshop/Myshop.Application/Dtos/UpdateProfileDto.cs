namespace Myshop.Application.Dtos
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }

        // تغيير الباسورد (اختياري)
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
