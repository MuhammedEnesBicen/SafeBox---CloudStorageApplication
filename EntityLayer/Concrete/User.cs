namespace EntityLayer.Concrete
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public float TotalSpaceUsed { get; set; } // in MB
    }
}
