using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SheetGenerator : MonoBehaviour
{
    public static void Generate(string csvPath, string cssPath, string cssName)
    {
        var code = CSharpClassCodeFromCsvFile(csvPath);

        var classFilePath = string.Format("{0}/{1}.cs", cssPath, cssName);

        FileStream stream = null;

        if (File.Exists(classFilePath))
            File.Delete(classFilePath);
        stream = File.Create(classFilePath);

        var sw = new StreamWriter(stream);

        sw.Write(code);

        sw.Close();
    }

    public static string CSharpClassCodeFromCsvFile(string filePath, string delimiter = ",", string classAttribute = "",
        string propertyAttribute = "")
    {
        if (string.IsNullOrWhiteSpace(propertyAttribute) == false)
            propertyAttribute += "\n\t";
        if (string.IsNullOrWhiteSpace(propertyAttribute) == false)
            classAttribute += "\n";

        var lines = File.ReadAllLines(filePath);
        var columnNames = lines.First().Split(',').Select(str => str.Trim()).ToArray();
        var data = lines.Skip(1).ToArray();

        var className = Path.GetFileNameWithoutExtension(filePath);
        // use StringBuilder for better performance
        var code = string.Format(
            "using FrameWork;\n using System;\nnamespace SheetData\n{{\n {0}public partial class {1} : {2} \n{{ \n",
            classAttribute, className, "DefinitionBase");
        for (var columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
        {
            var columnName = Regex.Replace(columnNames[columnIndex], @"[\s\.]", string.Empty, RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(columnName))
                columnName = "Column" + (columnIndex + 1);

            //한글 주석 확인
            var byteCount = Encoding.Default.GetByteCount(columnName);
            if (byteCount != columnName.Length) continue;

            //Key 컬럼 제외
            if (columnName.Equals("key") || columnName.Equals("Key"))
                continue;

            code += "\t" + GetVariableDeclaration(data, columnIndex, columnName, propertyAttribute) + "\n\n";
        }

        code += "}\n}\n";
        return code;
    }

    public static string GetVariableDeclaration(string[] data, int columnIndex, string columnName,
        string attribute = null)
    {
        var columnValues = data.Select(line => line.Split(',')[columnIndex].Trim()).ToArray();
        string typeAsString;

        if (AllIntValues(columnValues))
            typeAsString = "int";
        else if (AllDoubleValues(columnValues))
            typeAsString = "double";
        else if (AllBoolValues(columnValues))
            typeAsString = "bool";
        else if (AllDateTimeValues(columnValues))
            typeAsString = "DateTime";
        else
            typeAsString = "string";

        var declaration = string.Format("{0}public {1} {2} {{ get; set; }}", attribute, typeAsString, columnName);
        return declaration;
    }

    public static bool AllDoubleValues(string[] values)
    {
        double d;
        return values.All(val => double.TryParse(val, out d));
    }

    public static bool AllIntValues(string[] values)
    {
        int d;
        return values.All(val => int.TryParse(val, out d));
    }

    public static bool AllDateTimeValues(string[] values)
    {
        DateTime d;
        return values.All(val => DateTime.TryParse(val, out d));
    }

    public static bool AllBoolValues(string[] values)
    {
        bool d;
        return values.All(val => bool.TryParse(val, out d));
    }
}