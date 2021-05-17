namespace Bifrost.Common.Core.Domain
{
    public class Field
    {
        public Field(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new Field(Name, Value);
        }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }
}