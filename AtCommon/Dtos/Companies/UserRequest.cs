﻿namespace AtCommon.Dtos.Companies
{
    public class UserRequest : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}