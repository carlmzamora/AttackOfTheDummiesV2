using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionFieldName;
    public bool expectedValue;

    public ShowIfAttribute(string conditionFieldName, bool expectedValue = true)
    {
        this.conditionFieldName = conditionFieldName;
        this.expectedValue = expectedValue;
    }
}