using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public class GetManagedReferencesWithMissingTypesInScriptableObjects
{
    enum ReportFormat { Detailed, ClassList }

    [MenuItem("Example/Report ScriptableObject Missing SerializeReference Types")]
    static public void ReportMissingTypes()
    {
        ReportMissingTypesInScriptableObjects(ReportFormat.ClassList);
    }

    [MenuItem("Example/Report ScriptableObject Missing SerializeReference Types - Detailed")]
    static public void ReportMissingTypesDetailed()
    {
        ReportMissingTypesInScriptableObjects(ReportFormat.Detailed);
    }

    static private void ReportMissingTypesInScriptableObjects(ReportFormat reportType)
    {
        var report = new StringBuilder();

        // Find all ScriptableObjects in the project
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (scriptableObject != null)
            {
                ReportReferencesWithMissingTypesOnHost(scriptableObject, ref report, reportType);
            }
        }

        if (report.Length == 0)
            report.Append("No missing types found");

        Debug.Log(report.ToString());
    }

    static private void ReportReferencesWithMissingTypesOnHost(Object host, ref StringBuilder report, ReportFormat reportType)
    {
        // Report the references that have missing types on an individual ScriptableObject
        if (!SerializationUtility.HasManagedReferencesWithMissingTypes(host))
            return;

        var missingTypes = SerializationUtility.GetManagedReferencesWithMissingTypes(host);

        report.Append(reportType == ReportFormat.Detailed ? "Missing references on " : "Missing classes on ");
        ScriptableObjectDescription(host, ref report);

        if (reportType == ReportFormat.Detailed)
        {
            foreach (var missingType in missingTypes)
            {
                report.Append("\t").AppendFormat("{0} - {1}", missingType.referenceId, MissingClassFullName(missingType));
                if (missingType.serializedData.Length > 0)
                    report.Append("\t").AppendFormat("\n\t\t{0}", missingType.serializedData);
                report.AppendLine();
            }
        }
        else
        {
            // Only report each unique class that is missing, rather than all objects using that class
            var missingClasses = new HashSet<string>();
            foreach (var missingType in missingTypes)
            {
                missingClasses.Add(MissingClassFullName(missingType));
            }

            foreach (var missingClass in missingClasses)
            {
                report.Append("\t").Append(missingClass).AppendLine();
            }
        }
    }

    static private void ScriptableObjectDescription(Object host, ref StringBuilder stringBuilder)
    {
        // Identify the object that has missing types
        stringBuilder.AppendFormat("ScriptableObject \"{0}\" (Type: {1}, Instance: {2})",
            host.name,
            host.GetType().FullName,
            host.GetInstanceID()).AppendLine();
    }

    static private string MissingClassFullName(ManagedReferenceMissingType missingType)
    {
        var description = new StringBuilder();
        if (missingType.namespaceName.Length > 0)
            description.Append(missingType.namespaceName).Append(".");
        description.AppendFormat("{0}, {1}", missingType.className, missingType.assemblyName);
        return description.ToString();
    }
}
