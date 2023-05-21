namespace CarsharingProject.ViewModels
{
    public class UserWithRoleViewModel
    {
        public string UserName { get; set; }
        public string? Role { get; set; }
        public IEnumerable<string> AllRoles { get; set; }
    }
}
