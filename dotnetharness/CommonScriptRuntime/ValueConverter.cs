namespace CommonScript.Runtime
{
    public class ValueConverter
    {
        public static string RTValueToReadableString(object rtVal)
        {
            return Internal.FunctionWrapper.PUBLIC_valueToString(rtVal);
        }
    }
}
