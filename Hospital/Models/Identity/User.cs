namespace Hospital.Models.Identity
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string HashPassword { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public byte[] Image { get; set; }
        public Role Role { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
