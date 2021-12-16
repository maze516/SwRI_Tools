using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



[Serializable]
public class Exit_AttempteAutomatch : Exception
{
    public Exit_AttempteAutomatch() { }
    public Exit_AttempteAutomatch(string message) : base(message) { }
    public Exit_AttempteAutomatch(string message, Exception inner) : base(message, inner) { }
    protected Exit_AttempteAutomatch(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}