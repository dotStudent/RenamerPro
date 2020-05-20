using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public static class helper
    {
    public static bool IsNumeric(this string s)
    {
        float output;
        return float.TryParse(s, out output);
    }
    public static bool isDateTime(string dt)
    {
        return DateTime.TryParse(dt, out DateTime d);
    }
}
