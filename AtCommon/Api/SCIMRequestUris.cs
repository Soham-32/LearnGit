namespace AtCommon.Api
{
    public class ScimRequestUris
    {

        // Groups
        public static string Groups() => "/v1/Groups";
        public static string Groups(string id) => $"/v1/Groups/{id}";

        //Users
        public static string Users() => "/v1/Users";
        public static string Users(string id) => $"/v1/Users/{id}";
    }
}