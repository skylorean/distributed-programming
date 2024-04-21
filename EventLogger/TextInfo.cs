namespace EventsLogger
{
    internal class TextInfo
    {
        public string Id { get; set; }
        public string Result { get; set; }

        public TextInfo(string id, string result)
        {
            Id = id;
            Result = result;
        }
    }
}