using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using SoneAutomatedTests.DataModel;

namespace SoneAutomatedTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class FileTestCaseSource : TestCaseSourceAttribute
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public FileTestCaseSource() : base("Custom") { }

        private NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();
        protected NUnitTestCaseBuilder TestCaseBuilder { get { return _builder; } }


        protected string GetDirPath(IMethodInfo method)
        {
            foreach (var path in GetPaths(method, ""))
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException("Unable to locate test case source for method: " + method.Name);
        }

        protected string GetFilePath(IMethodInfo method, string suffix)
        {
            foreach (var path in GetPaths(method, suffix))
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException("Unable to locate test case source for method: " + method.Name);
        }

        protected IList<string> GetPaths(IMethodInfo method, string suffix)
        {
            var values = new List<string>();

            string className = method.MethodInfo.DeclaringType.FullName.Replace(@".", @"\");
            string methodName = method.Name;
            string pathEnds = methodName + "." + suffix;

            System.Diagnostics.Debug.WriteLine(FileTestCaseSource.AssemblyDirectory + @"\..\..\SmokeTests\" + pathEnds);
            string solutionPath = FileTestCaseSource.AssemblyDirectory + @"\..\..\SmokeTests\" + pathEnds;// ConfigurationManager.AppSettings.Get("solutionPath") + pathEnds;
            //string relativePath = className + @"\" + pathEnds;
            //string workdirPath = TestContext.CurrentContext.WorkDirectory + @"\" + relativePath;
            //string absolutePath1 = @"C:\Projects\SoneAutomatedTestsDotNet\" + relativePath;
            //string absolutePath2 = @"D:\Projects\SoneAutomatedTestsDotNet\" + relativePath;

            values.Add(solutionPath);
            //values.Add(relativePath);
            //values.Add(workdirPath);
            //values.Add(absolutePath1);
            //values.Add(absolutePath2);

            return values;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class JsonTestCaseSource : FileTestCaseSource, ITestBuilder, IImplyFixture
    {
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            IParameterInfo[] methodParams = method.GetParameters();
            if (methodParams.Length != 2)
            {
                throw new TargetParameterCountException("JSON data source only supports 1 test methods accepting 1 parameter.");
            }


            var tests = new List<TestMethod>();
            string dirPath = GetDirPath(method);

            string[] jsonFiles = Directory.GetFiles(GetDirPath(method), "*.json", SearchOption.AllDirectories);


            foreach (var jsonFile in jsonFiles)
            {
                object[] args = new object[2];
                args[0] = Path.GetFileName(jsonFile).Replace(".json", "").Replace("_", " ");

                string strJson = File.ReadAllText(jsonFile);
                try
                {
                    Type paramType = methodParams[1].ParameterType;
                    var obj = JsonConvert.DeserializeObject(strJson, paramType);
                    args[1] = obj;

                    tests.Add(TestCaseBuilder.BuildTestMethod(method, suite, new TestCaseParameters(args)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught Exception: " + ex.ToString());
                }
            }

            return tests;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CsvTestCaseSource : FileTestCaseSource, ITestBuilder, IImplyFixture
    {
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            List<TestMethod> tests = new List<TestMethod>();
            List<TestCaseParameters> parameterSet = new List<TestCaseParameters>();
            ParameterInfo[] methodParams = method.MethodInfo.GetParameters();

            string filename = GetFilePath(method, "csv");
            if (File.Exists(filename))
            {
                var csv = new CsvHelper.CsvReader(File.OpenText(filename));
                csv.Configuration.Comment = '#';
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.AllowComments = true;

                while (csv.Read())
                {
                    if (methodParams.Length != csv.CurrentRecord.Length)
                    {
                        throw new TargetParameterCountException("CSV data and method parameters count do not match.");
                    }

                    object[] args = new object[csv.CurrentRecord.Length];
                    for (int i = 0; i < csv.CurrentRecord.Length; i++)
                    {
                        string strValue = csv.CurrentRecord[i].Trim();

                        ParameterInfo paramInfo = methodParams[i];

                        var converter = TypeDescriptor.GetConverter(paramInfo.ParameterType);
                        if (converter != null)
                        {
                            try
                            {
                                var value = converter.ConvertFrom(strValue);
                                args[i] = value;
                            }
                            catch (System.FormatException)
                            {
                                if ("Boolean".Equals(paramInfo.ParameterType))
                                {
                                    // booleans are false by default
                                    args[i] = false;
                                }
                            }
                            catch (Exception e)
                            {
                                throw new TargetException("Unable to convert CSV data to target data type: " + strValue, e);
                            }

                        }
                        else
                        {
                            throw new TargetException("Unable to locate converter for CSV value: " + strValue);
                        }
                    }

                    tests.Add(TestCaseBuilder.BuildTestMethod(method, suite, new TestCaseParameters(args)));
                }
            }
            else
            {
                tests.Add(TestCaseBuilder.BuildTestMethod(method, suite, new TestCaseParameters()));
            }


            return tests;
        }
    }
}
