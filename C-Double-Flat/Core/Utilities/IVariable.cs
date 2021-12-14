using System.Text;
using System.Threading.Tasks;
namespace C_Double_Flat.Core.Utilities
{

    public enum VariableType
    {
        Value,
        Collection
    }

    public interface IVariable
    {
        public VariableType Type();

        public bool AsBool();

        public double AsDouble();
        public string AsString();
    }
}
