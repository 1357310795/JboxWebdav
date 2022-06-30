using System.Reflection;


namespace NutzCode.Libraries.Web
{
    public static class Extensions
    {
        public static void CopyTo(this object s, object d)
        {
            foreach (PropertyInfo pis in s.GetType().GetProperties())
            {
                foreach (PropertyInfo pid in d.GetType().GetProperties())
                {
                    if (pid.Name == pis.Name)
                        (pid.GetSetMethod()).Invoke(d, new [] { pis.GetGetMethod().Invoke(s, null) });
                }
            }
        }

    }
}
