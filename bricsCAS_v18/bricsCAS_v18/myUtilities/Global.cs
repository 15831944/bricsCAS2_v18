using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace CAS.myUtilities
{
    public sealed class Global
    {
        Assembly _assembly = typeof(CAS.Commands).Assembly;
        private const string _appName = "CAS";
        private const string _Prototypzeichnung = "CASProto.dwg";

        //Attribut Layersuffix
        private const string _layNr = "-B";
        private const string _layHöhe = "-H";
        private const string _layCode = "-Code";
        private const string _layDatum = "-Datum";

        private static readonly Lazy<Global> lazy =
        new Lazy<Global>(() => new Global());

        //Constructor
        private Global() { }

        //Properties
        public string AppName
        {
            get { return _appName; }
        }

        public string PrototypFullPath
        {
            get { return Path.GetDirectoryName(_assembly.Location) + "\\"+_Prototypzeichnung; }
        }

        public string LayNummer
        { get { return _layNr; } }

        public string LayHöhe
        {  get { return _layHöhe; } }

        public string LayDatum
        {  get { return _layDatum; } }

        public string LayCode
        { get { return _layCode; } }

        public static Global Instance
        { get { return lazy.Value; } }

        
    }
}
