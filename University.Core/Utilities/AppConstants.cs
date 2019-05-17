using System;
using System.Collections.Generic;
using System.Text;

namespace University.Core.Utilities
{
    public class AppConstants
    {
        public static class UserRoleConstants
        {
            public const int Administrator = 1;
            public const int Student = 2;
        }

        public static class StatusConstants
        {
            public const int Pending = 1;
            public const int Active = 2;
            public const int Deleted = 3;
        }
    }
}
