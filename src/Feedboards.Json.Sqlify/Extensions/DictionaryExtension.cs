namespace Feedboards.Json.Sqlify.Extensions;

public static class DictionaryExtension
{
    public static void MergeNestedDictionaries(
        this Dictionary<string, Dictionary<string, string>> target,
        Dictionary<string, Dictionary<string, string>> source)
    {
        foreach (var (outerKey, innerDict) in source)
        {
            if (!target.TryGetValue(outerKey, out var targetInnerDict))
            {
                targetInnerDict = new Dictionary<string, string>();
                target[outerKey] = targetInnerDict;
            }

            foreach (var (innerKey, innerValue) in innerDict)
            {
                targetInnerDict[innerKey] = innerValue; // Overwrites by default
            }
        }
    }
}