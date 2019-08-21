using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentCore
{
    public class DefaultTimeProvider:ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
    public class FakeTimeProvider:ITimeProvider
    {
        private double addMinutes;
        public FakeTimeProvider(double addMinutes)
        {
            this.addMinutes = addMinutes;
        }
        public DateTime Now()
        {
            DateTime now = DateTime.Now;
            return now.AddMinutes(addMinutes);
        }
    }

    public interface ITimeProvider
    {
        DateTime Now();
    }

}
