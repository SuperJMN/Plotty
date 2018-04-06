namespace Plotty.Model
{
    public class Register
    {
        public int Id { get; }

        public Register(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"R{Id}";
        }

        protected bool Equals(Register other)
        {
            return Id == other.Id;
        }

        public static implicit operator Register(int i)
        {
            return new Register(i);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Register) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}