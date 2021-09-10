namespace EmailRequestsRouter.Domain
{
    public class MailHeader
    {
        public MailHeader(string name = null, string value = null)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
