namespace TextUtils
{
    public class InputRequest
    {
        private readonly string? _text;
        public InputRequestKind Kind { get; }
        public int? InsertionPoint { get; }
        public object? Change { get; }
        public string? Text
        {
            get
            {
                return Kind == InputRequestKind.TextInput ? _text : Kind == InputRequestKind.NewLine ? "\r\n" : null;
            }
        }

        // public int SequenceId { get; set; }

        public InputRequest(InputRequestKind kind, string text, object? textChange)
        {
            // Timestamp = DateTime.Now;
            Kind = kind;
            _text = text;
            Change = textChange;
        }
        public InputRequest(InputRequestKind kind, string text, in int? insertionPoint=null)
        {
            Kind = kind;
            InsertionPoint = insertionPoint;
            _text = text;
            // Timestamp = DateTime.Now;
        }

        public InputRequest(InputRequestKind kind, in int? insertionPoint=null)
        {
            Kind = kind;
            InsertionPoint = insertionPoint;
        }

        public InputRequest(InputRequestKind kind, in char inputChar, in int? insertionPoint) : this(kind,inputChar.ToString(), insertionPoint)
        {
        }

        public override string ToString()
        {
            return $"{Kind} " + (Text != null ? $"({Text}) " : "");// + $"Seq={SequenceId}";
        }
    }
}