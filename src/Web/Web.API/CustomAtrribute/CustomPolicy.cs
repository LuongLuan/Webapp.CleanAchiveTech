namespace Web.API.CustomAtrribute
{
    public class CustomPolicy: Attribute
    {
        public string PolicyName { get; set; }

        public CustomPolicy(string policyName)
        {
            PolicyName = policyName;
        }
    }
}
