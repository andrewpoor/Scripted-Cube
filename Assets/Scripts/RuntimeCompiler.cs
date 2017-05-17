using Microsoft.CSharp;
using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;

public class RuntimeCompiler : MonoBehaviour
{
   static CompilerParameters param;
   static bool isInitialised = false;

   private static void Initialise()
   {
      param = new CompilerParameters();

      // Add ALL of the assembly references
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
         param.ReferencedAssemblies.Add(assembly.Location);
      }

      // Add specific assembly references
      //param.ReferencedAssemblies.Add("System.dll");
      //param.ReferencedAssemblies.Add("CSharp.dll");
      //param.ReferencedAssemblies.Add("UnityEngines.dll");

      // Generate a dll in memory
      param.GenerateExecutable = false;
      param.GenerateInMemory = true;

      isInitialised = true;
   }

   public static Assembly Compile(string source)
   {
      //When first called, initialise the assembly references first.
      //This should only occur once, else an exception is thrown.
      if (!isInitialised)
      {
         Initialise();
      }

      var provider = new CSharpCompiler.CodeCompiler();

      // Compile the source
      var result = provider.CompileAssemblyFromSource(param, source);

      //Check for errors
      if (result.Errors.Count > 0)
      {
         Debug.Log (source);

         var msg = new StringBuilder();
         foreach (CompilerError error in result.Errors)
         {
            msg.AppendFormat("Error ({0}): {1}\n",
               error.ErrorNumber, error.ErrorText);
         }

         throw new Exception(msg.ToString());
      }

      // Return the assembly
      return result.CompiledAssembly;
   }

   //Loads a file from storage
   public static string Load(string fileName)
   {
      try
      {
         string line;
         string result = "";
         StreamReader reader = new StreamReader(fileName, Encoding.Default);

         using (reader)
         {
            do
            {
               line = reader.ReadLine();

               if (line != null)
               {
                  result = result + line;
               }
            }
            while (line != null);

            reader.Close();
            return result;
         }
      }
      catch (Exception e)
      {
         throw e;
      }
   }
}
