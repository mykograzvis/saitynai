namespace Ligonine.Auth.Model
{
    public class ForumRoles
    {
        public const string Admin = nameof(Admin);
        public const string HospitalUser = nameof(HospitalUser);
        public const string Doctor = nameof(Doctor);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, HospitalUser, Doctor};
    }
}
