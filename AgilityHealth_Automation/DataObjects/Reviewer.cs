namespace AgilityHealth_Automation.DataObjects
{
    public class Reviewer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"< FirstName = '{FirstName}', LastName = '{LastName}', Role = '{Role}', Email = '{Email}' >";
        }
    }
}
