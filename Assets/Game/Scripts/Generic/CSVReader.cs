using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader {
    static string SPLIT_RE = ";";
    //static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    //static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static string LINE_SPLIT_RE = @"\n|\r";
    static char[] TRIM_CHARS = { '\"' };
    static string comma = "|";

    public static List<Dictionary<string, string>> Read(TextAsset data) {
        var list = new List<Dictionary<string, string>>();
        if (data == null) {
            return list;
        }
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1)
            return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++) {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++) {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                value = value.Replace(comma, ",");
                entry[header[j]] = value;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, string>> Read(string file) {
        TextAsset data = Resources.Load(file) as TextAsset;
        var list = Read(data);
        return list;
    }

    public static Dictionary<string, List<string>> ReadPro(string file) {
        TextAsset data = Resources.Load(file) as TextAsset;
        var dic = ReadPro(data);
        return dic;
    }
    /// <summary>
    /// Key của các dictionary là header.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> ReadPro(TextAsset data) {
        var dic = new Dictionary<string, List<string>>();
        if (data == null) {
            return dic;
        }
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1)
            return dic;

        var header = Regex.Split(lines[0], SPLIT_RE);
        int length = header.Length;
        for (int i = 0; i < length; i++) {
            var list = new List<string>();
            dic.Add(header[i], list);
        }

        for (var i = 1; i < lines.Length; i++) {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            for (var j = 0; j < header.Length && j < values.Length; j++) {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                value = value.Replace(comma, ",");
                dic[header[j]].Add(value);
            }
        }
        return dic;
    }
}
