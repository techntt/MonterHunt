using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader {

    static char[] LINE_CHAR = { '\n', ' ' };
    static char[] CONTENT_CHAR = { ';' };
     
    public static List<Dictionary<string,string>> ReadDataToList(string data)
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
        try
        {
            string[] lines = data.Split(LINE_CHAR);
            if (lines.Length > 1)
            {
                string[] header = lines[0].Split(CONTENT_CHAR) ;
                for(int i = 1; i < lines.Length; i++)
                {
                    string[] contents = lines[i].Split(CONTENT_CHAR);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    for (int j=0;j<contents.Length && j < header.Length; j++)
                    {
                        if (!dict.ContainsKey(header[j].Trim()))
                        {
                            dict.Add(header[j].Trim(), contents[j].Trim());
                        }
                    }
                    list.Add(dict);
                }
                return list;
            }
        }catch(System.Exception ex)
        {
            Debug.Log("[CSVReader] " + ex.Message);
            return null;
        }
        return null;
    }


    public static List<Dictionary<string, string>> ReadDataToListFromFile(string path)
    {
        TextAsset txData = ReadDataFromFile(path);
        if (txData == null)
            return null;
        return ReadDataToList(txData.text);
    }


    public static Dictionary<string,Dictionary<string, string>> ReadDataToDict(string data)
    {
        Dictionary<string,Dictionary<string, string>> list = new Dictionary<string,Dictionary<string, string>>();
        try
        {
            string[] lines = data.Split(LINE_CHAR);
            if (lines.Length > 1)
            {
                string[] header = lines[0].Split(CONTENT_CHAR);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] contents = lines[i].Split(CONTENT_CHAR);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    for (int j = 0; j < contents.Length && j < header.Length; j++)
                    {
                        if (!dict.ContainsKey(header[j].Trim()))
                        {
                            dict.Add(header[j].Trim(), contents[j].Trim());
                        }
                    }
                    list.Add(header[0].Trim(),dict);
                }
                return list;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("[CSVReader] " + ex.Message);
            return null;
        }
        return null;
    }


    public static Dictionary<string, Dictionary<string, string>> ReadDataToDictFromFile(string path)
    {
        TextAsset txData = ReadDataFromFile(path);
        if (txData == null)
            return null;
        return ReadDataToDict(txData.text);
    }

    private static TextAsset ReadDataFromFile(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path) as TextAsset;
        return textAsset;
    }
}
