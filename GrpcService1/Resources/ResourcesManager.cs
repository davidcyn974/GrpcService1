using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace GrpcService1.Resources
{
    public class ResourcesManager
    {
        #region Public Constructors

        static ResourcesManager()
        {
            TemplateRefusCollecte = GetResourceValueString(@"Resources\refus-collecte.html");
        }

        #endregion Public Constructors

        #region Private Methods

        private static string GetResourceValueString(string resourceName)
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using var stream = fileProvider.GetFileInfo(resourceName).CreateReadStream();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static byte[] GetResourceValueByteArray(string resourceName)
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using var stream = fileProvider.GetFileInfo(resourceName).CreateReadStream();
            using var reader = new StreamReader(stream);
            using MemoryStream ms = new MemoryStream();
            reader.BaseStream.CopyTo(ms);
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        #endregion Private Methods

        #region Public Fields

        public static readonly string TemplateRefusCollecte;

        #endregion Public Fields
    }
}