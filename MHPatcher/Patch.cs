namespace MHPatcher
{
    public class Patch
    {
        public string Name { get; }
        public bool IsEnabled { get; }
        public string Offset { get; }
        public string Data { get; }

        public Patch(string name, bool isEnabled, string offset, string data)
        {
            Name = name;
            IsEnabled = isEnabled;
            Offset = offset;
            Data = data;
        }
    }
}
