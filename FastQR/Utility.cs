namespace FastQR
{
    public static class Utility
    {
        public const string StoragePrivilege = "http://tizen.org/privilege/mediastorage";
        public const string LogTag = "FastQR";
        public const string Extension = ".fastqr";

        public static string GetTransformedFile(string file) => file + Extension;
    }
}
