using Example.Models;

namespace Example
{
    public static class ValidationExtensions
    {
        public static (bool isValid, string reason) IsValid(this MessageBoard request)
        {
            if (request == null)
            {
                return (false, "request cannot be null");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                return (false, "name cannot be null");
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                return (false, "content cannot be null");
            }

            return (true, string.Empty);
        }
    }
}
