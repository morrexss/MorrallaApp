using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Validations
{
    public interface IComparisonRule<T>
    {
        string ValidationMessage { get; set; }
        bool Compare(T value);
    }
}
