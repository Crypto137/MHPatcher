namespace MHPatcher
{
    public class Patch
    {
        public string Name { get; }
        public string Offset { get; }
        public string Data { get; }

        public Patch(string name, string offset, string data)
        {
            Name = name;
            Offset = offset;
            Data = data;
        }
    }
}
